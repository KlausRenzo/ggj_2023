using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	public class EnemyController : MonoBehaviour {
		public float maxLifeTime = 30f;
		[Space] public float maxDistanceFromPlayer = 100f;
		public float maxTimeFarFromPlayer = 10;
		[Space] public float damage = 1f;
		[SerializeField] private GameObject[] _particlePrefabs;
		[SerializeField] private AudioClip[] _deathClips;

		private const float AfarTimeTick = 0.5f;
		private PlayerController _player;
		private SpriteRenderer _spriteRenderer;
		private NavMeshAgent _agent;
		private float lifeTimer = 0;

		public event Action<EnemyController> OnKill;
		public event Action<EnemyController> OnRemove;

		private void Awake() {
			_player = FindObjectOfType<PlayerController>();
			_spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
			_agent = this.GetComponent<NavMeshAgent>();
		}

		private void Start() {
			StartCoroutine(FollowingPlayer());
			StartCoroutine(CheckingPlayerDistance());
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