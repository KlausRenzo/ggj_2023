using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aestetic {
	public class EnemySpawner : MonoBehaviour {
		[SerializeField] private Transform _parent;
		[SerializeField] [Range(0, 2)] private float _spawnDelay;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private float _radius;

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
			randomDistance.y = 0;

			var randomPosition = (randomDistance * _radius) + transform.position;
			var randomSprite = _sprites[Random.Range(0, _sprites.Length)];


			var instance = Instantiate(_prefab, _parent);
			instance.transform.position = randomPosition;
			instance.GetComponent<EnemyController>().SetSprite(randomSprite);
		}
	}
}