using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour {
	public float speed;
	public float offset;
	public Vector3 axis;

	private void OnValidate() {
		axis = axis.normalized;
	}

	private void Start() {
		transform.Rotate(axis, offset);
	}

	private void Update() {
		transform.Rotate(axis, speed * Time.deltaTime);
	}
}