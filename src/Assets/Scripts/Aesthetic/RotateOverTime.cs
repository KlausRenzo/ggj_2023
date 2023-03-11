using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RotateOverTime : MonoBehaviour {
	public float speed;
	public float offset;
	public float randomOffset;
	public Vector3 axis;

	private void OnValidate() {
		axis = axis.normalized;
	}

	private void Awake() {
		randomOffset = Random.Range(-randomOffset, randomOffset);
	}

	private void Start() {
		transform.Rotate(axis, offset + randomOffset);
	}

	private void Update() {
		transform.Rotate(axis, speed * Time.deltaTime);
	}
}