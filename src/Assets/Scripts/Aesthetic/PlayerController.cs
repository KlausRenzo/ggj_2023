using System;
using Aesthetic;
using DG.Tweening;
using Sirenix.OdinInspector;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {
		#region Fields

		[SerializeField] private Volume _postProcessVolume;
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

		[SerializeField] private Volume _feedbacksPostProcess;

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
		[Header("Big Bullets")] [SerializeField]
		private int bigBullets;
		[SerializeField] private int enemiesKilledForBigBullet;
		private int bigBulletCharger;
		public int BigBullets {
			get => bigBullets;
			set {
				bigBullets = value;
				OnBigBulletsUpdated?.Invoke(bigBullets);
			}
		}
		[Space] private GameManager gameManager;
		private EnemySpawner enemySpawner;
		private int killCounter;
		public bool isOnGround;

		public event Action<float> OnDrunkness;
		public event Action OnSoberUp;
		public event Action<float> OnHealth;
		public event Action OnHurt;
		public event Action OnHeal;
		public event Action<bool> OnRunState;
		public event Action<float> OnRun;
		public event Action<float> OnJump;
		public event Action<bool> OnGround;
		public event Action OnFire;
		public event Action OnBigFire;
		public event Action OnDeath;
		public event Action<int> OnBigBulletsUpdated;
		public event Action<float> OnBigBulletPartialUpdated;

		#endregion

		private void Awake() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			_rigidBody = GetComponent<Rigidbody>();
			_playerInput = GetComponent<PlayerInput>();
			_camera = this.GetComponentInChildren<Camera>();

			_jumpAudioSource = gameObject.AddComponent<AudioSource>();
			_jumpAudioSource.clip = _jumpAudioClip;
			_reactionAudioSource = gameObject.AddComponent<AudioSource>();
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
			BigBullets = bigBullets;
			OnBigBulletPartialUpdated?.Invoke(0);
		}

		private void OnEnemySpawn(EnemyController newEnemy) {
			newEnemy.OnKill += OnEnemyKilled;
		}

		private void OnEnemyKilled(EnemyController enemyController) {
			killCounter++;
			bigBulletCharger++;
			if (bigBulletCharger >= enemiesKilledForBigBullet) {
				bigBulletCharger = 0;
				BigBullets++;
			}
			OnBigBulletPartialUpdated?.Invoke((float)bigBulletCharger / enemiesKilledForBigBullet);
			if (killCounter % _reactionFrequency == 0 && !_reactionAudioSource.isPlaying) {
				_reactionAudioSource.clip = reactionClips[Random.Range(0, reactionClips.Length)];
				_reactionAudioSource.Play();
			}
		}

		private void Update() {
			Visual();
			if (!IsDead) {
				if (_playerInput.fire)
					Fire();
				if (_playerInput.bigFire) {
					if (BigBullets > 0) {
						BigFire();
					}
					else {
						BigFireDry();
					}
				}

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
		public void Health(float delta = -1) {
			currentHealth += godMode ? 100 : delta;
			if (currentHealth <= 0) {
				Vignette vignette;
				if (_feedbacksPostProcess.profile.TryGet<Vignette>(out vignette)) {
					vignette.color.value = Color.red;
				}

				OnDeath?.Invoke();
				_head.GetChild(0).transform.Rotate(_head.forward, 75f);
				currentHealth = 0;
				float intensity = 0f;
				float endIntensity = 1;
				DOTween.To(() => intensity, x => intensity = x, endIntensity, 1f)
					.OnUpdate(() => { _feedbacksPostProcess.weight = intensity; });
			}
			else {
				float weight = .5f;
				float endWeight = 0;
				UnityEngine.Rendering.Universal.Vignette v;
				if (delta < 0) {
					if (_feedbacksPostProcess.profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out v)) {
						v.color.value = Color.red;
					}
					DOTween.To(() => weight, x => weight = x, endWeight, 0.25f).OnUpdate(() => { _feedbacksPostProcess.weight = weight; });
					OnHurt?.Invoke();
				}
				if (delta > 0) {
					if (_feedbacksPostProcess.profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out v)) {
						v.color.value = Color.green;
					}
					DOTween.To(() => weight, x => weight = x, endWeight, 0.25f).OnUpdate(() => { _feedbacksPostProcess.weight = weight; });
					OnHeal?.Invoke();
				}
			}
			OnHealth?.Invoke(currentHealth / maxHealth);
		}

		private void UpdateDof() {
			var hasHit = Physics.Raycast(this.transform.position + transform.forward * 1, transform.forward, out var hit);
			if (!hasHit)
				return;
			DepthOfField dof;
			if (_postProcessVolume.profile.TryGet<DepthOfField>(out dof)) {
				dof.focusDistance.value = (this.transform.position - hit.point).magnitude;
			}
		}

		private void CheckGround() {
			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, 2.5f, floorMask.value)) {
				if (!isOnGround) {
					OnGround?.Invoke(true);
				}
				isOnGround = true;
				consecutiveJumps = 0;
				_jumpCount = _maxJumps;
				_jumpAudioSource.pitch = 1f;
				OnJump?.Invoke(1f);
			}
			else {
				if (isOnGround) {
					OnGround?.Invoke(false);
				}
				isOnGround = false;
			}
		}

		private void Fire() {
			OnFire?.Invoke();
			float intensity = 5;
			DOTween.To(() => intensity, x => intensity = x, 0, 0.2f)
				.OnUpdate(() => {
					ColorAdjustments c;
					if (_postProcessVolume.profile.TryGet<ColorAdjustments>(out c))
						c.postExposure.value = intensity;
				});
		}

		private void BigFire() {
			OnBigFire?.Invoke();
			BigBullets--;
			float intensity = 15;
			DOTween.To(() => intensity, x => intensity = x, 0, 1f)
				.OnUpdate(() => {
					ColorAdjustments c;
					if (_postProcessVolume.profile.TryGet<ColorAdjustments>(out c))
						c.postExposure.value = intensity;
				});
		}

		private void BigFireDry() {
			BigBullets = bigBullets;
		}

		private void Visual() {
			transform.Rotate(transform.up, _playerInput.viewDelta.x * viewMultiplier * 0.1f);

			float xDelta = _playerInput.viewDelta.y * viewMultiplier * 0.1f;
			xRotation = Mathf.Clamp(xRotation + xDelta, -90, 90);
			_head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
		}

		private void Movement() {
			if (isRunning ^ _playerInput.run) {
				//cambio fra capito bro, run e walk
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
				var ec = collision.gameObject.GetComponent<HealthModifier>();
				if (ec != null) {
					Health(-ec.damage);
				}
			}
		}
	}
}