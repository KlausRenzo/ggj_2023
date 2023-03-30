using System;
using System.Collections;
using Aesthetic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	public class EnemyController : MonoBehaviour {
		public float maxLifeTime = 30f;
		[SerializeField] private float health = 2;
		[Space] public float maxDistanceFromPlayer = 100f;
		public float maxTimeFarFromPlayer = 10;
		[SerializeField] private GameObject[] _particlePrefabs;
		[SerializeField] private AudioClip[] _deathClips;

		private const float AfarTimeTick = 0.5f;
		private PlayerController _player;
		private SpriteRenderer _spriteRenderer;
		private NavMeshAgent _agent;
		private Hurtable hurtable;
		private float lifeTimer = 0;

		public event Action<EnemyController> OnKill;
		public event Action<EnemyController> OnRemove;

		private void Awake() {
			_player = FindObjectOfType<PlayerController>();
			_spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
			_agent = this.GetComponent<NavMeshAgent>();
			hurtable = gameObject.AddComponent<Hurtable>();
		}

		private void Start() {
			if (hurtable.onHurt == null)
				hurtable.onHurt = new UnityEvent<float>();
			StartCoroutine(FollowingPlayer());
			StartCoroutine(CheckingPlayerDistance());

			hurtable.onHurt.AddListener(Hurt);
		}

		private void OnDestroy() {
			hurtable.onHurt.RemoveListener(Hurt);
		}

		private IEnumerator FollowingPlayer() {
			while (true) {
				yield return new WaitForSeconds(.5f);
				if (_agent.isOnNavMesh)
					_agent.SetDestination(_player.transform.position);
			}
		}

		private IEnumerator CheckingPlayerDistance() {
			float timeFar = 0;
			while (true) {
				yield return new WaitForSeconds(AfarTimeTick);
				if (Vector3.Distance(_player.transform.position, transform.position) >= maxDistanceFromPlayer) {
					timeFar += AfarTimeTick;
					if (timeFar >= maxTimeFarFromPlayer) {
						DeactivateEnemy();
					}
				}
				else {
					timeFar = 0;
				}
			}
		}

		private void Update() {
			_spriteRenderer.transform.LookAt(_player.transform);
			lifeTimer += Time.deltaTime;
			if (lifeTimer >= maxLifeTime) {
				DeactivateEnemy();
			}
		}

		private void DeactivateEnemy() {
			OnRemove?.Invoke(this);
			Destroy(gameObject);
		}

		public void InitEnemy(Sprite sprite) {
			_spriteRenderer.sprite = sprite;
			name += $"-{sprite.name}";
		}

		public void Hurt(float damage) {
			health += damage;
			if (health <= 0) {
				Kill();
			}
		}

		public void Kill() {
			OnKill?.Invoke(this);
			var randomPrefab = _particlePrefabs[Random.Range(0, _particlePrefabs.Length)];
			var instance = Instantiate(randomPrefab, this.transform.position, Quaternion.identity);
			AudioSource audioSource = instance.AddComponent<AudioSource>();
			audioSource.clip = _deathClips[Random.Range(0, _deathClips.Length)];
			audioSource.pitch = Random.Range(0.75f, 1.25f);
			audioSource.Play();
			Destroy(instance, 3);
			Destroy(gameObject);
		}
	}
}