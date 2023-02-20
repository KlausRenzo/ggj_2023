using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	public class ObstacleSpawner : MonoBehaviour {
		[SerializeField] private GameObject[] _prefabs;
		[SerializeField] private float _density = 100;
		[SerializeField] private int _maxInstances = 1000;
		[SerializeField] private Vector3 _minScale = Vector3.one;
		[SerializeField] private Vector3 _maxScale = Vector3.one;
		[SerializeField] private Collider _collider;
		[SerializeField] private Vector3 _rotation;
		[SerializeField] private AnimationCurve falloff;
		[SerializeField] private bool generateNavmesh;
		[SerializeField] private NavigationBaker navigationBaker;

		private void OnValidate() {
			if (navigationBaker == null) navigationBaker = GetComponent<NavigationBaker>();
		}

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

			Debug.Log($"{(max - min).magnitude * _density}");
			var count = Mathf.Min(_maxInstances, (max - min).magnitude * _density);
			Debug.Log($"Generating {count} instances");
			for (int i = 0; i < count; i++) {
				var position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
				var distanceFromCenter = Vector3.Distance(position, colliderPosition) / _collider.bounds.extents.x * 2;

				var randomVector = new Vector3(Random.Range(-_rotation.x, _rotation.x), Random.Range(-_rotation.y, _rotation.y), Random.Range(-_rotation.z, _rotation.z));
				//randomVector.y *= falloff.Evaluate(distanceFromCenter);
				//Debug.Log($"Distance = {distanceFromCenter} falloff = {falloff.Evaluate(distanceFromCenter)}");
				var scale = VectorMultipliedMysteriously(_maxScale - _minScale, new Vector3(Random.value, Random.value * falloff.Evaluate(distanceFromCenter), Random.value));
				scale += _minScale;
				var chosenPrefab = _prefabs[Random.Range(0, _prefabs.Length)];
#if UNITY_EDITOR
				var instance = (GameObject)PrefabUtility.InstantiatePrefab(chosenPrefab, null);
#else
				var instance = Instantiate(chosenPrefab, null);
#endif
				instance.name = chosenPrefab.name + $" {i + 1}";
				instance.transform.position = position; // + Vector3.up * scale.y / 2;
				instance.transform.rotation = Quaternion.Euler(randomVector);
				instance.transform.localScale = scale;
				instance.transform.parent = transform;
				instance.isStatic = true;
			}
			if (generateNavmesh)
				navigationBaker.Build();
		}

		private Vector3 VectorMultipliedMysteriously(Vector3 a, Vector3 b) {
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
	}
}