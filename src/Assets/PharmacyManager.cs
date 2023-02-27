using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Aesthetic;
using UnityEngine;

public class PharmacyManager : MonoBehaviour {
	[SerializeField] private LayerMask playerLayer;
	private AudioSource audioSource;
	[SerializeField] private AudioClip[] pharmacyClips;

	private int currentClipIndex;
	public float healPerSecond = 1f;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log($"{other.gameObject.layer} {playerLayer.value}");
		if (playerLayer == (playerLayer | (1 << other.gameObject.layer))) { }
	}

	private void PlayNextAudio() {
		if (!audioSource.isPlaying) {
			audioSource.PlayOneShot(pharmacyClips[currentClipIndex]);
			currentClipIndex = (currentClipIndex + 1) % pharmacyClips.Length;
		}
	}

	private void OnTriggerStay(Collider other) {
		if (playerLayer == (playerLayer | (1 << other.gameObject.layer))) {
			other.GetComponent<PlayerController>().Health(healPerSecond * Time.deltaTime);
			PlayNextAudio();
		}
	}
}