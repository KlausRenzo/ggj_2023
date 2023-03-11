using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Aesthetic;
using UnityEngine;

public class PharmacyManager : MonoBehaviour {
	[SerializeField] private LayerMask playerLayer;
	private AudioSource audioSource;
	[SerializeField] private AudioClip[] pharmacyClips;
	[SerializeField] private Transform pharmacyDoorTransform;
	[SerializeField] private float pharmacyDoorCloseScale = 9;
	[SerializeField] private Material crossMaterialOff;
	[SerializeField] private MeshRenderer[] crossesMeshRenderers;
	[SerializeField] private GameObject[] stuffToDeactivateOnClose;

	private int currentClipIndex;
	public float healPerSecond = 1f;
	public float openTime = 100f;
	private float currentOpenTime;
	private Vector3 doorClosedLocalScale;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
		currentOpenTime = openTime;
		doorClosedLocalScale = new Vector3(1, pharmacyDoorCloseScale, 1);
	}

	// private void OnTriggerEnter(Collider other) {
	// 	if (playerLayer == (playerLayer | (1 << other.gameObject.layer))) { }
	// }

	private void PlayNextAudio() {
		audioSource.PlayOneShot(pharmacyClips[currentClipIndex]);
		currentClipIndex = (currentClipIndex + 1) % pharmacyClips.Length;
	}

	private void CloseUpPharmacy() {
		foreach (MeshRenderer crossesMeshRenderer in crossesMeshRenderers) {
			crossesMeshRenderer.material = crossMaterialOff;
		}
		foreach (GameObject o in stuffToDeactivateOnClose) {
			o.SetActive(false);
		}
	}

	private void OnTriggerStay(Collider other) {
		if (playerLayer == (playerLayer | (1 << other.gameObject.layer)) && currentOpenTime > 0) {
			other.GetComponent<PlayerController>().Health(healPerSecond * Time.deltaTime);
			currentOpenTime -= Time.deltaTime;
			pharmacyDoorTransform.localScale = Vector3.Lerp(Vector3.one, doorClosedLocalScale, (1f - currentOpenTime / openTime));
			if (!audioSource.isPlaying)
				PlayNextAudio();
			if (currentOpenTime <= 0) {
				CloseUpPharmacy();
			}
		}
	}
}