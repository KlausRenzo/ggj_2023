using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

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
		[Button("Clear")]
		private void Clear() {
			for (int i = transform.childCount - 1; i >= 0; i--) {
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}

		public void Start() {
			_collider.enabled = false;
		}

		[ContextMenu("Generate")]
		[Button("Generate")]
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

				var instance = (GameObject)PrefabUtility.InstantiatePrefab(_prefab, transform);
				instance.transform.position = position + Vector3.up * scale.y / 2;
				instance.transform.rotation = transform.rotation;
				instance.transform.localScale = scale;
				instance.isStatic = true;
			}
		}

		private Vector3 CosaCheDovrebbeEsserci(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
	}
}