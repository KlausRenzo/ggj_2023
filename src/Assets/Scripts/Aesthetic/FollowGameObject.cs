using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowGameObject : MonoBehaviour {
	public Transform target;
	public float spring = 0.5f;

	private void Awake() {
		if (target == null) {
			Destroy(this);
		}
	}

	private void Update() {
		transform.position = Vector3.Lerp(transform.position, target.transform.position, spring);
	}
}