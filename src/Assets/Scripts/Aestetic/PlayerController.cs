using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Assets.Scripts.Aestetic
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour
	{
#region Fields

		[SerializeField] private Transform _head;
		private int _jumpCount = 10;
		private AudioSource _audioSource;
		[SerializeField] AudioClip _audioClip;
		[SerializeField] private float _jumpPower = 10;
		[SerializeField] private PlayerGun _playerGun;

		private PlayerInput _playerInput;
		private Rigidbody _rigidBody;

		[SerializeField] private LayerMask floorMask;

		private Vector3 maxRotation = new(90, 360, 360);
		private Vector3 minRotation = new(-90, -360, -360);
		[SerializeField] private float speedMultiplier;
		[SerializeField] private float viewMultiplier;
		private float xRotation = 90;
		public new Camera camera;

#endregion

		private void Awake()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_rigidBody = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			camera = this.GetComponentInChildren<Camera>();

			_audioSource = this.AddComponent<AudioSource>();
			_audioSource.clip = _audioClip;
		}

		// Update is called once per frame
		private void Update()
		{
			Movement();
			Visual();
			Fire();

			CheckGround();
			UpdateDof();
		}

		[SerializeField] private PostProcessVolume _postProcessVolume;

		private void UpdateDof()
		{
			var hasHit = Physics.Raycast(this.transform.position + transform.forward * 1, transform.forward, out var hit);

			if (!hasHit)
				return;
			_postProcessVolume.profile.GetSetting<DepthOfField>().focusDistance.value = (this.transform.position - hit.point).magnitude;
		}

		private void CheckGround()
		{
			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, 2.5f, floorMask.value))
			{
				_jumpCount = 10;
				_audioSource.pitch = 1;
			}
		}

		[SerializeField] private AudioSource _shootAudioSource;
		[SerializeField] private AudioClip[] _shootClips;

		private void Fire()
		{
			if (!_playerInput.fire)
				return;

			_playerGun.Shoot();
			_shootAudioSource.clip = _shootClips[Random.Range(0, _shootClips.Length)];
			_shootAudioSource.pitch = Random.Range(0.9f, 1.1f);
			_shootAudioSource.Play();

			StartCoroutine(ShootLight());
		}

		private IEnumerator ShootLight()
		{
			_postProcessVolume.profile.GetSetting<ColorGrading>().postExposure.value = 5;
			yield return null;
			yield return null;
			_postProcessVolume.profile.GetSetting<ColorGrading>().postExposure.value = 0;
		}

		private void Visual()
		{
			transform.Rotate(transform.up, _playerInput.viewDelta.x * viewMultiplier * 0.1f);

			float xDelta = _playerInput.viewDelta.y * viewMultiplier * 0.1f;
			xRotation = Mathf.Clamp(xRotation + xDelta, -90, 90);
			_head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		}


		private void Movement()
		{
			Vector3 desiredDirection = transform.rotation * _playerInput.movementDirection * speedMultiplier;
			_rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, desiredDirection, Time.deltaTime);

			if (_playerInput.jump && _jumpCount > 0)
			{
				_jumpCount--;
				_audioSource.pitch = Mathf.Lerp(1, 2, (10f - _jumpCount) / 10f);
				_audioSource.Play();
				_rigidBody.AddForce(Vector3.up * _jumpPower);
			}
		}
	}
}