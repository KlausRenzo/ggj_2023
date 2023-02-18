using System;
using System.Collections;
using Assets.Scripts.Aesthetic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour {
	public GameObject actualCamera;
	public float duration;
	public float intensity;
	public float maxIntensity;
	public float maxDistance;

	private float appliedIntensity;

	public AnimationCurve falloff;

	private float currentDuration;
	private GameManager gameManager;
	private bool isShaking;
	private Vector3 defaultCameraPosition;

	private void Awake() {
		gameManager = FindObjectOfType<GameManager>();
		gameManager.OnShake += OnShake;
		defaultCameraPosition = actualCamera.transform.localPosition;
	}

	private void OnShake(Vector3 origin) {
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
			actualCamera.transform.localPosition = defaultCameraPosition + (Vector3.one * (Random.Range(-0.5f, 0.5f) * appliedIntensity * t));
			timer += Time.deltaTime;
			yield return null;
		}
		actualCamera.transform.localPosition = defaultCameraPosition;
		currentDuration = 0;
		appliedIntensity = 0;
		isShaking = false;
	}
}