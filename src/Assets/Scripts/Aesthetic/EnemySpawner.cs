using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	public class EnemySpawner : MonoBehaviour {
		[FormerlySerializedAs("maxEnemySpawned")] [SerializeField]
		private int maxEnemiesActive = 100;
		[FormerlySerializedAs("spawnCenter")] public Transform spawnTransform;
		[SerializeField] private Transform _enemyContainer;
		[SerializeField] [Range(0, 2)] private float _spawnDelay;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private float _radius;
		private List<EnemyController> enemies;
		public event Action<int> OnEnemyCountUpdated;
		public event Action<EnemyController> OnSpawn;
		public event Action<EnemyController, int> OnKill;
		private int kills;
		[ShowInInspector, ReadOnly] private bool isSpawning;

		private void Awake() {
			enemies = new List<EnemyController>();
		}

		private void Start() {
			StartCoroutine(SpawnEnemy());
		}

		private IEnumerator SpawnEnemy() {
			while (true) {
				yield return new WaitForSeconds(_spawnDelay);
				if (isSpawning && enemies.Count < maxEnemiesActive) {
					Spawn();
				}
			}
		}

		public void Spawn() {
			var randomDistance = Random.insideUnitSphere;
			var randomPosition = (randomDistance * _radius) + spawnTransform.position;
			randomPosition.y = 0;
			var instance = Instantiate(_prefab, randomPosition, Quaternion.identity, _enemyContainer);
			var newEnemyController = instance.GetComponent<EnemyController>();
			newEnemyController.SetSprite(_sprites[Random.Range(0, _sprites.Length)]);
			newEnemyController.OnKill += OnEnemyKilled;
			enemies.Add(newEnemyController);
			OnEnemyCountUpdated?.Invoke(enemies.Count);
			OnSpawn?.Invoke(instance.GetComponent<EnemyController>());
		}

		private void OnEnemyKilled(EnemyController enemyController) {
			kills++;
			OnKill?.Invoke(enemyController, kills);
			enemies.Remove(enemyController);
		}

		public void StartSpawn() {
			isSpawning = true;
		}

		public void StopSpawn() {
			isSpawning = false;
		}
	}
}