using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

namespace Assets.Scripts.Aestetic
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour
	{
		private Rigidbody _rigidBody;
		[SerializeField] private float speedMultiplier;
		[SerializeField] private float viewMultiplier;
		[SerializeField] private Transform _head;

		private PlayerInput _playerInput;
		[SerializeField]private PlayerGun _playerGun;

		void Awake()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_rigidBody = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
		}

		// Update is called once per frame
		void Update()
		{
			Movement();
			Visual();
			Fire();
		}

		private void Fire()
		{
			if (!_playerInput.fire)
				return;
			
			_playerGun.Shoot();
		}

		private Vector3 maxRotation = new Vector3(90, 360, 360);
		private Vector3 minRotation = new Vector3(-90, -360, -360);
		private float xRotation = 0;

		private void Visual()
		{
			transform.Rotate(transform.up, _playerInput.viewDelta.x * viewMultiplier * 0.1f);

			var xDelta = _playerInput.viewDelta.y * viewMultiplier * 0.1f;
			xRotation = Mathf.Clamp(xRotation + xDelta, -90, 90);
			_head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		}


		private void Movement()
		{
			var desiredDirection = transform.rotation * _playerInput.movementDirection * speedMultiplier;
			_rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, desiredDirection, Time.deltaTime);
		}
	}
}