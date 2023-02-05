using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Aestetic
{
	public class EnemyController : MonoBehaviour
	{
		private PlayerController _player;
		private SpriteRenderer _spriteRenderer;
		private NavMeshAgent _agent;
		private Camera _camera;

		private void Awake()
		{
			_player = FindObjectOfType<PlayerController>();
			_spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
			_agent = this.GetComponent<NavMeshAgent>();
			_camera = _player.camera;
		}

		private void Start()
		{
			StartCoroutine(FollowPlayer());
		}

		private IEnumerator FollowPlayer()
		{
			while (true)
			{
				yield return new WaitForSeconds(.5f);
				_agent.SetDestination(_player.transform.position);
			}
		}

		private void Update()
		{
			_spriteRenderer.transform.LookAt(_player.transform);
		}

		public void SetSprite(Sprite sprite)
		{
			this._spriteRenderer.sprite = sprite;
		}

		[SerializeField] private GameObject[] _particlePrefabs;
		[SerializeField] private AudioClip[] _deathClips;

		public void Kill()
		{
			FindObjectOfType<GameManager>().OnKill();

			var randomPrefab = _particlePrefabs[Random.Range(0, _particlePrefabs.Length)];
			var instance = Instantiate(randomPrefab, this.transform.position, Quaternion.identity);
			AudioSource audio = instance.AddComponent<AudioSource>();
			audio.clip = _deathClips[Random.Range(0, _deathClips.Length)];
			audio.pitch = Random.Range(0.75f, 1.25f);
			audio.Play();


			Destroy(gameObject);
			Destroy(instance, 3);
		}
	}
}