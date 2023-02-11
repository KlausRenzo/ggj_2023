using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour {
	public float speed;
	public Vector3 axis;

	private void OnValidate() {
		axis = axis.normalized;
	}

	private void Update() {
		transform.RotateAround(axis, speed * Time.deltaTime);
	}
}