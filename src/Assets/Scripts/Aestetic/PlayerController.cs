using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	private bool _fire;
	private bool _jump;
	private Vector3 _movementDirection;
	private Rigidbody _rigidBody;
	[SerializeField] private float speedMultiplier;
	[SerializeField] private float viewMultiplier;
	[SerializeField] private Transform _head;
	private Vector3 _previousMousePosition;
	private Vector3 _viewDelta;

	void Awake()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		_rigidBody = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		Inputs();
		Movement();
		Visual();
	}

	private Vector3 maxRotation = new Vector3(90, 360, 360);
	private Vector3 minRotation = new Vector3(-90, -360, -360);
	private float xRotation = 0;

	private void Visual()
	{
		transform.Rotate(transform.up, _viewDelta.x * viewMultiplier * 0.1f);

		var xDelta = _viewDelta.y * viewMultiplier * 0.1f;
		xRotation = Mathf.Clamp(xRotation + xDelta, -90, 90);
		_head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
	}

	private void Inputs()
	{
		var horizontal = Input.GetAxis("Horizontal");
		var vertical = Input.GetAxis("Vertical");
		_movementDirection = new Vector3(horizontal, 0, vertical);
		_jump = Input.GetButtonDown("Jump");
		_fire = Input.GetButtonDown("Fire1");
		_viewDelta = new Vector3(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
	}

	private void Movement()
	{
		var desiredDirection = transform.rotation * _movementDirection * speedMultiplier;
		_rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, desiredDirection, Time.deltaTime);
	}
}

public static class Extensions
{
	public static Vector3 ClampX(this Quaternion vector, float min, float max)
	{
		Vector3 eulerAngles = vector.eulerAngles;


		var x = eulerAngles.x;
		var y = eulerAngles.y;
		var z = eulerAngles.z;

		var result = new Vector3(x, y, z);

		Debug.Log(eulerAngles + "  -  " + result);

		return result;
	}
}