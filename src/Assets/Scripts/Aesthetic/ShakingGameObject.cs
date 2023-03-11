using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IShakeable {
	public event Action<bool> OnShake;
}

public class ShakingGameObject : MonoBehaviour {
	[SerializeField] private float intensity;
	[SerializeField] private GameObject shakingPart;
	[SerializeField] private IShakeable shakeable;
	private Vector3 defaultLocalPosition;
	private Quaternion defaultLocalRotation;

	private void Awake() {
		shakeable ??= GetComponent<IShakeable>();
	}

	private void Start() {
		shakeable.OnShake += OnShake;
		defaultLocalPosition = shakingPart.transform.localPosition;
		defaultLocalRotation = shakingPart.transform.localRotation;
	}

	public void SetIntensity(float f) {
		intensity = f;
	}

	[Button("Test Shake")]
	private void OnShake(bool startShake) {
		if (startShake) {
			StartCoroutine(Shaking());
		}
		else {
			StopAllCoroutines();
			shakingPart.transform.localRotation = defaultLocalRotation;
			shakingPart.transform.localPosition = defaultLocalPosition;
		}
	}

	private IEnumerator Shaking() {
		while (true) {
			shakingPart.transform.localRotation = Quaternion.Euler(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity)) * defaultLocalRotation;
			yield return null;
		}
	}
}