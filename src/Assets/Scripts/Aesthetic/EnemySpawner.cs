using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aestetic {
	public class EnemySpawner : MonoBehaviour {
		[SerializeField] private int maxEnemySpawned = 100;
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

		private void Awake() {
			enemies = new List<EnemyController>();
		}

		private void Start() {
			StartCoroutine(SpawnEnemy());
		}

		private IEnumerator SpawnEnemy() {
			while (true) {
				yield return new WaitForSeconds(_spawnDelay);
				if (enemies.Count < maxEnemySpawned) {
					Spawn();
				}
			}
		}

		private void Spawn() {
			var randomDistance = Random.insideUnitSphere;
			var randomPosition = (randomDistance * _radius) + transform.position;
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
	}
}