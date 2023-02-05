using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
	public GameObject prefab;
	private Camera _camera;
	private void Awake()
	{
		_camera = Camera.main;
	}
    void FixedUpdate()
    {
		if (Input.GetMouseButton(0))
		{
			for (int i = 0; i < 5; i++)
			{
				var position = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15));
			Instantiate(prefab, position, Quaternion.identity);
			}
			
		}

	}
}
