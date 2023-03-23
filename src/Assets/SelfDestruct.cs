using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {
	public float lifeTime;

	private void Update() {
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			Destroy(gameObject);
		}
	}
}