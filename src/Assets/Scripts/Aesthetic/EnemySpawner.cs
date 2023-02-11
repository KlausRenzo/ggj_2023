using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aestetic {
	public class EnemySpawner : MonoBehaviour {
		[FormerlySerializedAs("_parent")] [SerializeField]
		private Transform _enemyContainer;
		[SerializeField] [Range(0, 2)] private float _spawnDelay;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private float _radius;
		public event Action<EnemyController> OnSpawn;

		private void Start() {
			StartCoroutine(SpawnEnemy());
		}

		private IEnumerator SpawnEnemy() {
			while (true) {
				yield return new WaitForSeconds(_spawnDelay);
				Spawn();
			}
		}

		private void Spawn() {
			var randomDistance = Random.insideUnitSphere;
			var randomPosition = (randomDistance * _radius) + transform.position;
			randomPosition.y = 0;
			var instance = Instantiate(_prefab, randomPosition, Quaternion.identity, _enemyContainer);

			instance.GetComponent<EnemyController>().SetSprite(_sprites[Random.Range(0, _sprites.Length)]);

			OnSpawn?.Invoke(instance.GetComponent<EnemyController>());
		}
	}
}