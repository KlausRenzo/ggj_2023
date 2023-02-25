using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Aesthetic {
	public class GameManager : MonoBehaviour {
		[SerializeField] private MeshRenderer cityHealthRenderer;
		[SerializeField] private EnemySpawner enemySpawner;
		[SerializeField] private PlayerController playerController;
		[SerializeField] private float consumePerEnemy = 1f;
		[SerializeField] private int stopSpawnAfter = 500;
		private Material consumeMaterial;
		private int currentActiveEnemies;
		private int spawnedEnemies;
		private int killedEnemies;

		public event Action<float> OnRadicalizationUpdate;
		public event Action<Vector3, bool> OnShake;

		private void Start() {
			consumePerEnemy /= 100f;
			consumeMaterial = new Material(cityHealthRenderer.material);
			cityHealthRenderer.material = consumeMaterial;
			cityHealthRenderer.material.SetFloat("_Roots_Percentage", 0);

			playerController = FindObjectOfType<PlayerController>();
			enemySpawner.spawnTransform = playerController.transform;

			enemySpawner.OnSpawn += OnEnemySpawn;
			enemySpawner.OnEnemyCountUpdated += OnEnemyCountUpdated;
			enemySpawner.OnKill += OnEnemyKilled;
			enemySpawner.StartSpawn();
		}

		private void OnEnemyCountUpdated(int activeEnemies) {
			this.currentActiveEnemies = activeEnemies;
		}

		private void OnEnemySpawn(EnemyController newEnemy) {
			spawnedEnemies++;
		}

		private void OnEnemyKilled(EnemyController killedEnemy, int totalKills) {
			killedEnemies = totalKills;
			if (killedEnemies >= stopSpawnAfter) {
				enemySpawner.StopSpawn();
			}
		}

		private void Update() {
			var newFloat = cityHealthRenderer.material.GetFloat("_Roots_Percentage") + consumePerEnemy * Time.deltaTime * currentActiveEnemies;
			OnRadicalizationUpdate?.Invoke(newFloat);
			//Debug.Log(newFloat);
			cityHealthRenderer.material.SetFloat("_Roots_Percentage", newFloat);
		}

		[Button("Shake")]
		public void Shake(Vector3 origin, bool mustBeOnGround = false) {
			OnShake?.Invoke(origin, mustBeOnGround);
		}

		public void ShakeGround(Vector3 origin) {
			Shake(origin, true);
		}
	}
}