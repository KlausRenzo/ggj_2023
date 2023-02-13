using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	public class EnemyController : MonoBehaviour {
		public float damage = 1f;
		private PlayerController _player;
		private SpriteRenderer _spriteRenderer;
		private NavMeshAgent _agent;
		private Camera _camera;
		public event Action<EnemyController> OnKill;

		private void Awake() {
			_player = FindObjectOfType<PlayerController>();
			_spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
			_agent = this.GetComponent<NavMeshAgent>();
			_camera = _player._camera;
		}

		private void Start() {
			StartCoroutine(FollowPlayer());
		}

		private IEnumerator FollowPlayer() {
			while (true) {
				yield return new WaitForSeconds(.5f);

				_agent.SetDestination(_player.transform.position);
			}
		}

		private void Update() {
			_spriteRenderer.transform.LookAt(_player.transform);
		}

		public void SetSprite(Sprite sprite) {
			_spriteRenderer.sprite = sprite;
		}

		[SerializeField] private GameObject[] _particlePrefabs;
		[SerializeField] private AudioClip[] _deathClips;

		public void Kill() {
			OnKill?.Invoke(this);

			var randomPrefab = _particlePrefabs[Random.Range(0, _particlePrefabs.Length)];
			var instance = Instantiate(randomPrefab, this.transform.position, Quaternion.identity);
			AudioSource audioSource = instance.AddComponent<AudioSource>();
			audioSource.clip = _deathClips[Random.Range(0, _deathClips.Length)];
			audioSource.pitch = Random.Range(0.75f, 1.25f);
			audioSource.Play();

			Destroy(gameObject);
			Destroy(instance, 3);
		}
	}
}