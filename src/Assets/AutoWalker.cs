using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoWalker : MonoBehaviour {
	[FormerlySerializedAs("restPosition")] public Transform restTransform;
	public GameObject iKTarget;
	public LayerMask terrainLayer;
	public float minDistance = 10f;
	public float moveDistance = 50f;
	public float moveSpeed = 40f;
	[ShowInInspector, ReadOnly] private Vector3 currentPosition;
	[ShowInInspector, ReadOnly] private Vector3 targetPositionOnFloor;
	[ShowInInspector, ReadOnly] private Vector3 currentRestPosition;

	[ShowInInspector, ReadOnly] private Vector3 bodyPositionOnFloor;

	private void OnDrawGizmos() {
		// Gizmos.color = Color.cyan;
		// Gizmos.DrawRay(restTransform.position, Vector3.down * 500);

		Gizmos.color = Color.green;
		targetPositionOnFloor = iKTarget.transform.position;
		Gizmos.DrawWireSphere(targetPositionOnFloor, 15);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(currentRestPosition, 20);

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(bodyPositionOnFloor, 10);
	}

	private void Awake() {
		targetPositionOnFloor = restTransform.position;
		targetPositionOnFloor.y = 0;
		bodyPositionOnFloor = transform.position;
		bodyPositionOnFloor.y = 0;
	}

	private void Update() {
		var distance = Vector3.Distance(currentRestPosition, currentPosition);
		Debug.DrawLine(currentRestPosition, currentPosition);
		if (distance > moveDistance) {
			Debug.Log($"Move!");
			targetPositionOnFloor = currentRestPosition;
		}
		if (distance > minDistance) {
			currentPosition = Vector3.Lerp(currentPosition, targetPositionOnFloor, 0.9f * moveSpeed * Time.deltaTime);
		}

		iKTarget.transform.position = currentPosition;
	}

	private void FixedUpdate() {
		RaycastHit hit;
		if (Physics.Raycast(restTransform.position, Vector3.down, out hit, 500, terrainLayer)) {
			Debug.DrawRay(restTransform.position, Vector3.down * 200, Color.yellow);
			Debug.Log(hit.collider.gameObject.name);
			currentRestPosition = hit.point;
		}
		bodyPositionOnFloor = transform.position;
		bodyPositionOnFloor.y = 0;
		// if (Physics.Raycast(transform.position + Vector3.up * 500, Vector3.down, out hit, Mathf.Infinity, terrainLayer)) {
		// 	Debug.DrawRay(transform.position + Vector3.up * 500, Vector3.down * 200, Color.white);
		// 	bodyPositionOnFloor = hit.transform.position;
		// 	Debug.Log($"{transform.position} {bodyPositionOnFloor}");
		// }
	}
}