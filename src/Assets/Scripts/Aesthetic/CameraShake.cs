using System;
using System.Collections;
using Assets.Scripts.Aesthetic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour {
	[SerializeField] private PlayerController playerController;
	public GameObject actualCamera;
	[SerializeField] private Vector3 axis;
	[SerializeField] private float duration;
	public float intensity;
	[SerializeField] private float maxIntensity;
	[SerializeField] private float maxDistance;

	private float appliedIntensity;

	public AnimationCurve falloff;

	private float currentDuration;
	private GameManager gameManager;
	private bool isShaking;
	private Vector3 defaultCameraPosition;
	private Quaternion defaultCameraRotation;

	private void Awake() {
		gameManager = FindObjectOfType<GameManager>();
		gameManager.OnShake += OnShake;
		defaultCameraPosition = actualCamera.transform.localPosition;
		defaultCameraRotation = actualCamera.transform.localRotation;
	}

	private void OnShake(Vector3 origin, bool mustBeOnGround) {
		if (mustBeOnGround && !playerController.isOnGround) return;

		var distance = Vector3.Distance(transform.position, origin);
		if (!(distance > maxDistance)) {
			float damp = falloff.Evaluate(distance / maxDistance);
			appliedIntensity = Mathf.Clamp(appliedIntensity + damp * intensity, 0, maxIntensity);
			currentDuration += damp * duration;
			if (!isShaking) {
				isShaking = true;
				StartCoroutine(Shaking());
			}
		}
	}

	private IEnumerator Shaking() {
		float timer = 0;
		while (timer <= currentDuration) {
			float t = timer / currentDuration;
			//actualCamera.transform.localPosition = defaultCameraPosition + (Vector3.one * (Random.Range(-0.5f, 0.5f) * appliedIntensity * t));

			actualCamera.transform.localRotation = Quaternion.AngleAxis(Random.Range(-0.5f, 0.5f) * appliedIntensity * t, axis) * defaultCameraRotation;
			timer += Time.deltaTime;
			yield return null;
		}
		//actualCamera.transform.localPosition = defaultCameraPosition;
		actualCamera.transform.localRotation = defaultCameraRotation;
		currentDuration = 0;
		appliedIntensity = 0;
		isShaking = false;
	}
}