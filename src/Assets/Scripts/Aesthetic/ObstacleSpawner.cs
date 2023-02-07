using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Aestetic {
	public class ObstacleSpawner : MonoBehaviour {
		[SerializeField] private GameObject _prefab;
		[SerializeField] private float _density = 100;
		[SerializeField] private Vector3 _maxScale;
		[SerializeField] private Collider _collider;

		private void Awake() {
			_collider = GetComponent<Collider>();
		}

		[ContextMenu("Clear")]
		private void Clear() {
			for (int i = transform.childCount - 1; i >= 0; i--) {
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}

		public void Start() {
			_collider.enabled = false;
			//Generate();
		}

		[ContextMenu("Generate")]
		private void Generate() {
			var min = _collider.bounds.min;
			var max = _collider.bounds.max;
			var colliderPosition = _collider.transform.position;

			var count = (max - min).sqrMagnitude * _density;

			for (int i = 0; i < count; i++) {
				var position = colliderPosition + new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

				var randomVector = new Vector3(Random.value, Random.value, Random.value).normalized;
				var rotation = Quaternion.Euler(randomVector);
				var scale = CosaCheDovrebbeEsserci(_maxScale, new Vector3(Random.value, Random.value, Random.value));

				var instance = Instantiate(_prefab, position + Vector3.up * scale.y / 2, rotation, this.transform);
				instance.transform.localScale = scale;
			}
		}

		private Vector3 CosaCheDovrebbeEsserci(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
	}
}