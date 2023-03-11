using System;
using UnityEngine;

public class FollowPosition : MonoBehaviour {
	public Transform targetTransform;
	private Vector3 positionOffset;
	public Vector3 axisWeight;
	public bool relative;
	private Quaternion startingLocalRotation;

	private void OnDrawGizmos() {
		if (targetTransform != null) {
			Gizmos.color = Color.grey;
			Gizmos.DrawLine(transform.position, targetTransform.position);
		}
	}

	private void Awake() {
		if (targetTransform == null) {
			Destroy(this);
		}
		startingLocalRotation = transform.rotation;
		var position = targetTransform.position;
		positionOffset = transform.position - new Vector3(axisWeight.x * position.x, axisWeight.y * position.y, axisWeight.z * position.z);
	}

	private void Update() {
		var position = targetTransform.position;
		if (relative) {
			transform.position = Vector3.zero;
			transform.rotation = startingLocalRotation * targetTransform.rotation;
		}
		transform.position = new Vector3(axisWeight.x * position.x, axisWeight.y * position.y, axisWeight.z * position.z) + positionOffset;
	}
}