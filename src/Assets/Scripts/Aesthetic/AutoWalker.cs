using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AutoWalker : MonoBehaviour {
	public Transform restTransform;
	public GameObject iKTarget;
	public LayerMask terrainLayer;
	public float minDistance = 10f;
	[FormerlySerializedAs("moveDistance")] public float maxDistance = 50f;
	[FormerlySerializedAs("moveTime")] [FormerlySerializedAs("moveSpeed")]
	public float movingTime = 3f;
	public float yAltitude;
	public AnimationCurve yCurve;
	public bool isOnGround;
	[ShowInInspector, ReadOnly] private Vector3 previousPosition;
	[ShowInInspector, ReadOnly] private Vector3 currentPosition;
	[ShowInInspector, ReadOnly] private Vector3 targetPositionOnFloor;
	[ShowInInspector, ReadOnly] private Vector3 movingRestPosition;
	[ShowInInspector, ReadOnly] private Vector3 bodyPositionOnFloor;

	private float movingTimer;
	public event Action<Vector3> OnFootDown;
	public event Action<Vector3> OnFootUp;
	public UnityEvent<Vector3> onFootDown;
	public UnityEvent<Vector3> onFootUp;

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(targetPositionOnFloor, 15);

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(movingRestPosition, maxDistance);
		Gizmos.DrawWireSphere(movingRestPosition, minDistance);

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(currentPosition, 20);
	}

	private void Awake() {
		targetPositionOnFloor = restTransform.position;
		targetPositionOnFloor.y = 0;
		bodyPositionOnFloor = transform.position;
		bodyPositionOnFloor.y = 0;
		currentPosition = iKTarget.transform.position;
		currentPosition.y = 0;
		isOnGround = true;
	}

	private void Update() {
		var distanceToRestPosition = Vector3.Distance(movingRestPosition, currentPosition);
		var distanceToTargetPosition = Vector3.Distance(targetPositionOnFloor, currentPosition);

		Debug.DrawLine(movingRestPosition, currentPosition, Color.red);
		Debug.DrawLine(targetPositionOnFloor, currentPosition, Color.green);

		//Debug.Log($"dist to rest:{distanceToRestPosition} - dist to target {distanceToTargetPosition}");

		if (isOnGround && distanceToRestPosition > maxDistance) {
			isOnGround = false;
			//Debug.LogWarning($"Move!");
			previousPosition = currentPosition;
			targetPositionOnFloor = movingRestPosition;
			movingTimer = 0;
			OnFootUp?.Invoke(currentPosition);
			onFootUp.Invoke(currentPosition);
		}

		if (!isOnGround) {
			float t = movingTimer / movingTime;
			currentPosition = Vector3.Lerp(previousPosition, targetPositionOnFloor, t);
			currentPosition.y += yCurve.Evaluate(t) * yAltitude;
			movingTimer += Time.deltaTime;
		}

		if (movingTimer >= movingTime) {
			isOnGround = true;
			OnFootDown?.Invoke(currentPosition);
			onFootDown.Invoke(currentPosition);
			movingTimer = 0;
		}
		iKTarget.transform.position = currentPosition;
	}

	private void FixedUpdate() {
		RaycastHit hit;
		if (Physics.Raycast(restTransform.position, Vector3.down, out hit, 500, terrainLayer)) {
			Debug.DrawRay(restTransform.position, Vector3.down * 200, Color.yellow);
			//Debug.Log(hit.collider.gameObject.name);
			movingRestPosition = hit.point;
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