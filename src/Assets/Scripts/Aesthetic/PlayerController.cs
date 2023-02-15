using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {
		#region Fields

		[SerializeField] private PostProcessVolume _postProcessVolume;
		[Space] [SerializeField] private Transform _head;
		[SerializeField] private PlayerGun _playerGun;
		private PlayerInput _playerInput;
		private Rigidbody _rigidBody;

		[SerializeField] private LayerMask floorMask;

		[SerializeField] private float speedMultiplier;
		[SerializeField] private float viewMultiplier;
		private float xRotation = 90;
		public Camera _camera;
		[Header("Reactions")] [SerializeField] private int _reactionFrequency = 2;
		private AudioSource _reactionAudioSource;
		[SerializeField] private AudioClip[] reactionClips;

		[Header("Drunkness")] [SerializeField] private float maxDrunkness;
		[SerializeField] private float drunknessConsumedPerSecond;
		private float drunkness;
		public float Drunkness {
			get => drunkness;
			private set {
				drunkness = value;
				if (drunkness < 0) {
					drunkness = 0;
					if (!godMode)
						OnSoberUp?.Invoke();
				}
				else {
					if (drunkness > maxDrunkness) drunkness = maxDrunkness;
					OnDrunkness?.Invoke(drunkness / maxDrunkness);
				}
				_postProcessVolume.weight = drunkness / maxDrunkness;
			}
		}

		public bool godMode;

		[Header("Health")] [SerializeField] private float maxHealth;
		[SerializeField] private float currentHealth;
		private bool IsDead => currentHealth <= 0;

		[SerializeField] private PostProcessVolume _feedbacksPostProcess;

		[Header("Jump")] [SerializeField] AudioClip _jumpAudioClip;
		[SerializeField] private float _jumpPower = 10;
		[SerializeField] private int _maxJumps = 10;
		private int _jumpCount = 10;
		private int consecutiveJumps;
		private AudioSource _jumpAudioSource;

		[Header("Run")] [SerializeField] private float runTime = 5f;
		[SerializeField] private float runMultiplier = 1.5f;
		[SerializeField] private float runRechargePerTick = 0.1f;
		private float runningTime;
		[SerializeField] private bool isRunning;

		[Space] private GameManager gameManager;
		private EnemySpawner enemySpawner;
		private int killCounter;

		[SerializeField] private AudioSource _shootAudioSource;
		[SerializeField] private AudioClip[] _shootClips;

		public event Action<float> OnDrunkness;
		public event Action OnSoberUp;
		public event Action<float> OnHealth;
		public event Action OnHurt;
		public event Action<bool> OnRunState;
		public event Action<float> OnRun;
		public event Action<float> OnJump;
		public event Action OnDeath;

		#endregion

		private void Awake() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_rigidBody = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			_camera = this.GetComponentInChildren<Camera>();

			_jumpAudioSource = this.AddComponent<AudioSource>();
			_jumpAudioSource.clip = _jumpAudioClip;
			_reactionAudioSource = this.AddComponent<AudioSource>();
			currentHealth = maxHealth;
			Drunkness = maxDrunkness;
		}

		private void Start() {
			gameManager = FindObjectOfType<GameManager>();
			enemySpawner = FindObjectOfType<EnemySpawner>();
			enemySpawner.OnSpawn += OnEnemySpawn;
			OnRun?.Invoke(1);
			OnJump?.Invoke(1);
			OnHealth?.Invoke(currentHealth / maxHealth);
		}

		private void OnEnemySpawn(EnemyController newEnemy) {
			newEnemy.OnKill += OnEnemyKilled;
		}

		private void OnEnemyKilled(EnemyController enemyController) {
			killCounter++;
			if (killCounter % _reactionFrequency == 0 && !_reactionAudioSource.isPlaying) {
				_reactionAudioSource.clip = reactionClips[Random.Range(0, reactionClips.Length)];
				_reactionAudioSource.Play();
			}
		}

		private void Update() {
			Visual();
			if (!IsDead) {
				Fire();
				Jump();
			}
			CheckGround();
			//UpdateDof();
		}

		private void FixedUpdate() {
			if (!IsDead) {
				Movement();
			}
		}

		[Button("Health")]
		private void Health(float delta = -1) {
			currentHealth += godMode ? 100 : delta;
			if (currentHealth <= 0) {
				_feedbacksPostProcess.profile.GetSetting<Vignette>().color.value = Color.red;
				OnDeath?.Invoke();

				currentHealth = 0;
				float intensity = 0f;
				float endIntensity = 1;
				DOTween.To(() => intensity, x => intensity = x, endIntensity, 1f)
					.OnUpdate(() => { _feedbacksPostProcess.weight = intensity; });
			}
			else {
				float intensity = .5f;
				float endIntensity = 0;
				if (delta < 0) {
					_feedbacksPostProcess.profile.GetSetting<Vignette>().color.value = Color.red;
					DOTween.To(() => intensity, x => intensity = x, endIntensity, 0.25f).OnUpdate(() => { _feedbacksPostProcess.weight = intensity; });
				}
				if (delta > 0) {
					_feedbacksPostProcess.profile.GetSetting<Vignette>().color.value = Color.green;
					DOTween.To(() => intensity, x => intensity = x, endIntensity, 0.25f).OnUpdate(() => { _feedbacksPostProcess.weight = intensity; });
				}
			}
			OnHealth?.Invoke(currentHealth / maxHealth);
		}

		private void UpdateDof() {
			var hasHit = Physics.Raycast(this.transform.position + transform.forward * 1, transform.forward, out var hit);

			if (!hasHit)
				return;
			_postProcessVolume.profile.GetSetting<DepthOfField>().focusDistance.value = (this.transform.position - hit.point).magnitude;
		}

		private void CheckGround() {
			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, 2.5f, floorMask.value)) {
				consecutiveJumps = 0;
				_jumpCount = _maxJumps;
				_jumpAudioSource.pitch = 1f;
				OnJump?.Invoke(1f);
			}
		}

		private void Fire() {
			if (!_playerInput.fire)
				return;

			_playerGun.Shoot();
			_shootAudioSource.clip = _shootClips[Random.Range(0, _shootClips.Length)];
			_shootAudioSource.pitch = Random.Range(0.9f, 1.1f);
			_shootAudioSource.Play();

			float intensity = 5;
			DOTween.To(() => intensity, x => intensity = x, 0, 0.2f)
				.OnUpdate(() => { _postProcessVolume.profile.GetSetting<ColorGrading>().postExposure.value = intensity; });
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
					OnRunState?.Invoke(true);
				}
				else {
					OnRunState?.Invoke(false);
					Debug.Log($"Stop Run");
				}
			}
			isRunning = _playerInput.run;
			var runMul = runMultiplier;
			if (_playerInput.run) {
				runningTime += Time.fixedDeltaTime;
				if (runningTime >= runTime) {
					runMul = runMultiplier * .9f;
					Drunkness -= drunknessConsumedPerSecond * Time.deltaTime;
					OnRunState?.Invoke(false);
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
		}

		private void Jump() {
			if (_playerInput.jump && _jumpCount > 0) {
				consecutiveJumps++;
				_jumpCount--;
				if (_jumpCount <= _maxJumps / 2) {
					Drunkness -= drunknessConsumedPerSecond;
					//_jumpCount++;
				}

				_rigidBody.AddForce(Vector3.up * _jumpPower);

				float j = (float)(_maxJumps - _jumpCount) / _maxJumps;
				_jumpAudioSource.pitch = Mathf.Lerp(1, 2, j);
				_jumpAudioSource.Play();

				OnJump?.Invoke(1 - j);
			}
		}

		private void OnCollisionEnter(Collision collision) {
			if (!IsDead) {
				var ec = collision.gameObject.GetComponent<EnemyController>();
				if (ec != null) {
					Health(-ec.damage);
				}
			}
		}
	}
}