using UnityEngine;

namespace Assets.Scripts
{
	public class RootSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject _prefab;
		private Camera _camera;

		private void Awake()
		{
			_camera = this.GetComponent<Camera>();
		}

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
				return;

			var ray = _camera.ScreenPointToRay(Input.mousePosition);

			if (!Physics.Raycast(ray, out var hitInfo))
				return;

			var instance = Instantiate(_prefab);
			instance.transform.position = hitInfo.point;
		}
	}
}