using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aestetic {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {
		#region Fields

		[SerializeField] private Transform _head;

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
		[Header("Reactions")] [SerializeField] private int _reactionFrequency = 2;
		private AudioSource _reactionAudioSource;
		[FormerlySerializedAs("clips")] [SerializeField] private AudioClip[] reactionClips;

		[Header("Jump")] [FormerlySerializedAs("_audioClip")] [SerializeField]
		AudioClip _jumpAudioClip;
		[SerializeField] private float _jumpPower = 10;
		private int _jumpCount = 10;
		private AudioSource _jumpAudioSource;

		[Header("Run")] [SerializeField] private float runTime = 5f;
		[SerializeField] private float runMultiplier = 1.5f;
		[SerializeField] private float runRechargePerTick = 0.1f;
		private float runningTime;
		[SerializeField] private bool isRunning;
		
		private GameManager gameManager;
		private EnemySpawner enemySpawner;
		private int killCounter;
		
		public event Action<float> OnRun;

		#endregion

		private void Awake() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_rigidBody = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			camera = this.GetComponentInChildren<Camera>();

			_jumpAudioSource = this.AddComponent<AudioSource>();
			_jumpAudioSource.clip = _jumpAudioClip;
			_reactionAudioSource = this.AddComponent<AudioSource>();
		}

		private void Start() {
			gameManager = FindObjectOfType<GameManager>();
			enemySpawner = FindObjectOfType<EnemySpawner>();
			enemySpawner.OnSpawn += OnEnemySpawn;
			OnRun?.Invoke(1);
		}

		private void OnEnemySpawn(EnemyController newEnemy) {
			newEnemy.OnKill += OnEnemyKilled;
		}

		private void OnEnemyKilled() {
			killCounter++;
			if (killCounter % _reactionFrequency == 0 && !_reactionAudioSource.isPlaying) {
				_reactionAudioSource.clip = reactionClips[Random.Range(0, reactionClips.Length)];
				_reactionAudioSource.Play();
			}
		}

		private void Update() {
			Visual();
			Fire();

			CheckGround();
			UpdateDof();
		}

		private void FixedUpdate() {
			Movement();
		}

		[SerializeField] private PostProcessVolume _postProcessVolume;

		private void UpdateDof() {
			var hasHit = Physics.Raycast(this.transform.position + transform.forward * 1, transform.forward, out var hit);

			if (!hasHit)
				return;
			_postProcessVolume.profile.GetSetting<DepthOfField>().focusDistance.value = (this.transform.position - hit.point).magnitude;
		}

		private void CheckGround() {
			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, 2.5f, floorMask.value)) {
				_jumpCount = 10;
				_jumpAudioSource.pitch = 1;
			}
		}

		[SerializeField] private AudioSource _shootAudioSource;
		[SerializeField] private AudioClip[] _shootClips;

		private void Fire() {
			if (!_playerInput.fire)
				return;

			_playerGun.Shoot();
			_shootAudioSource.clip = _shootClips[Random.Range(0, _shootClips.Length)];
			_shootAudioSource.pitch = Random.Range(0.9f, 1.1f);
			_shootAudioSource.Play();

			StartCoroutine(ShootLight());
		}

		private IEnumerator ShootLight() {
			_postProcessVolume.profile.GetSetting<ColorGrading>().postExposure.value = 5;
			yield return null;
			yield return null;
			_postProcessVolume.profile.GetSetting<ColorGrading>().postExposure.value = 0;
		}

		private void Visual() {
			transform.Rotate(transform.up, _playerInput.viewDelta.x * viewMultiplier * 0.1f);

			float xDelta = _playerInput.viewDelta.y * viewMultiplier * 0.1f;
			xRotation = Mathf.Clamp(xRotation + xDelta, -90, 90);
			_head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		}

		private void Movement() {
			if (isRunning ^ _playerInput.run) {
				//cambio fra run e walk
				if (_playerInput.run) {
					Debug.Log($"Start Run");
				}
				else {
					Debug.Log($"Stop Run");
				}
			}
			isRunning = _playerInput.run;
			var runMul = runMultiplier;
			if (_playerInput.run) {
				runningTime += Time.fixedDeltaTime;
				if (runningTime >= runTime) {
					runMul = .9f;
					Debug.Log($"Tired Run");
				}
			}
			else {
				runningTime -= runRechargePerTick;
			}
			runningTime = Mathf.Clamp(runningTime, 0, runTime);
			OnRun?.Invoke(1f - runningTime / runTime);
			Vector3 desiredDirection = transform.rotation * _playerInput.movementDirection * (speedMultiplier * runMul);
			_rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, desiredDirection, Time.fixedDeltaTime);

			if (_playerInput.jump && _jumpCount > 0) {
				_jumpCount--;
				_jumpAudioSource.pitch = Mathf.Lerp(1, 2, (10f - _jumpCount) / 10f);
				_jumpAudioSource.Play();
				_rigidBody.AddForce(Vector3.up * _jumpPower);
			}
		}
	}
}