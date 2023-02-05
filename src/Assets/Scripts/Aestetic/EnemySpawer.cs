using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aestetic
{
	public class EnemySpawer : MonoBehaviour
	{
		[SerializeField] private Transform _parent;

		[SerializeField] private GameObject _prefab;
		[SerializeField] private Sprite[] _sprites;
		[SerializeField] private float _radius;

		private void Start()
		{
			StartCoroutine(SpawnEnemy());
		}

		private IEnumerator SpawnEnemy()
		{
			while (true)
			{
				yield return new WaitForSeconds(_spawnDelay);
				Spawn();
			}
		}

		[SerializeField] [Range(0, 2)] private float _spawnDelay;

		private void Spawn()
		{
			var randomAngle = Random.insideUnitSphere;
			randomAngle.y = 0;

			var randomPosition = (randomAngle * _radius) + this.transform.position;
			var randomSprite = _sprites[Random.Range(0, _sprites.Length)];


			var instance = Instantiate(_prefab, _parent);
			instance.transform.position = randomPosition;
			instance.GetComponent<EnemyController>().SetSprite(randomSprite);
		}
	}
}