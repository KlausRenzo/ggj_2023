using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Aesthetic;
using UnityEngine;

public class GlassesWeapon : MonoBehaviour {
	[SerializeField] private float health;
	public LayerMask playerLayer;
	public float timeBeforePushing;
	private float currentTimer;
	[SerializeField] private GameObject laser;
	[SerializeField] private float pushingForce;
	[SerializeField] private float cooldown;
	[SerializeField] private ParticleSystem particleSystem;
	[SerializeField] private AudioClip chargingClip;
	[SerializeField] private AudioClip firingClip;
	private AudioSource audioSource;
	private bool firing;

	private void Awake() {
		currentTimer = 0;
		laser.SetActive(false);
		audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other) {
		if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
			particleSystem.Play();
			if (!audioSource.isPlaying)
				audioSource.PlayOneShot(chargingClip);
		}
	}

	private void OnTriggerExit(Collider other) {
		if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
			currentTimer = 0;
			particleSystem.Stop();
		}
	}

	private IEnumerator TurningOffLaser() {
		yield return new WaitForSeconds(cooldown);
		laser.SetActive(false);
		firing = false;
	}

	private void OnCollisionEnter(Collision collision) {
		audioSource.PlayOneShot(firingClip);
		var bullet = collision.gameObject.GetComponent<Bullet>();
		if (bullet != null) {
			health += bullet.GetDamage();
		}
	}

	private void OnTriggerStay(Collider other) {
		if (!firing && (playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
			currentTimer += Time.deltaTime;
			if (currentTimer >= timeBeforePushing) {
				other.GetComponent<Rigidbody>().AddForce(transform.forward * pushingForce);
				laser.SetActive(true);
				StartCoroutine(TurningOffLaser());
				firing = true;
			}
		}
	}
}