using UnityEngine;

namespace Assets.Scripts.Aestetic
{
	public class ObstacleSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject _prefab;
		[SerializeField] private float _density = 100;
		[SerializeField] private float _maxScale = 2;
		private Collider _collider;

		private void Awake()
		{
			_collider = GetComponent<Collider>();
		}

		private void Start()
		{
			var min = _collider.bounds.min;
			var max = _collider.bounds.max;
			var colliderPosition = _collider.transform.position;

			var count = (max - min).sqrMagnitude * _density;

			for (int i = 0; i < count; i++)
			{
				var position = colliderPosition + new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

				var randomVector = new Vector3(Random.value, Random.value, Random.value).normalized;
				var rotation = Quaternion.Euler(randomVector);
				var scale = new Vector3(Random.value, Random.value, Random.value) * _maxScale;

				var instance = Instantiate(_prefab, position, rotation, this.transform );
				instance.transform.localScale = scale;
			}
		}
	}
}