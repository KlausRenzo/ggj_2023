using System;
using UnityEngine;

public class FollowPosition : MonoBehaviour {
	public Transform targetTransform;
	private Vector3 positionOffset;

	private void OnDrawGizmos() {
		if (targetTransform != null) {
			Gizmos.color = Color.grey;
			Gizmos.DrawLine(transform.position, targetTransform.position);
		}
	}

	private void Start() {
		if (targetTransform == null) {
			Destroy(this);
		}
		positionOffset = transform.position - targetTransform.position;
	}

	private void Update() {
		transform.position = targetTransform.position + positionOffset;
	}
}