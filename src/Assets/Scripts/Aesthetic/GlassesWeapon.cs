using System;
using System.Collections;
using System.Collections.Generic;
using Aesthetic;
using Assets.Scripts.Aesthetic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GlassesWeapon : MonoBehaviour {
	[SerializeField] private GlassesWeapon otherGlassesWeapon;
	[SerializeField] private float health;
	[SerializeField] private GameObject explosion;
	[SerializeField] private GameObject smallExplosionPrefab;
	public LayerMask playerLayer;
	public float timeBeforePushing;
	private float currentTimer;
	[SerializeField] private GameObject laser;
	[SerializeField] private float pushingForce;
	[SerializeField] private float cooldown;
	[SerializeField] private ParticleSystem particleSystem;
	[SerializeField] private AudioClip chargingClip;
	[SerializeField] private AudioClip firingClip;
	[SerializeField] private AudioClip hitClip;
	private AudioSource audioSource;
	private bool firing;
	private Hurtable hurtable;

	private void Awake() {
		currentTimer = 0;
		laser.SetActive(false);
		audioSource = GetComponent<AudioSource>();
		hurtable = gameObject.AddComponent<Hurtable>();

		hurtable.onHurt = new UnityEvent<float>();
		hurtable.onHurt.AddListener(Hurt);
	}

	public void ParticleActivation() {
		particleSystem.Play();
		if (!audioSource.isPlaying)
			audioSource.PlayOneShot(chargingClip);
	}

	private void OnTriggerEnter(Collider other) {
		if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
			ParticleActivation();
			if (otherGlassesWeapon != null) {
				otherGlassesWeapon.ParticleActivation();
			}
		}
	}

	public void ParticleDeactivation() {
		currentTimer = 0;
		particleSystem.Stop();
	}

	private void OnTriggerExit(Collider other) {
		if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
			ParticleDeactivation();
			if (otherGlassesWeapon != null) {
				otherGlassesWeapon.ParticleDeactivation();
			}
		}
	}

	private IEnumerator TurningOffLaser() {
		yield return new WaitForSeconds(cooldown);
		laser.SetActive(false);
		firing = false;
	}

	public void Hurt(float damage) {
		health += damage;
	}

	private void OnCollisionEnter(Collision collision) {
		var bullet = collision.gameObject.GetComponent<Bullet>();
		if (bullet != null) {
			audioSource.PlayOneShot(hitClip);
			Hurt(bullet.GetDamage());
			if (health <= 0) {
				Die();
			}
			else {
				var newExplosion = Instantiate(smallExplosionPrefab, null);
				newExplosion.transform.position = collision.contacts[0].point;
			}
		}
	}

	private void Die() {
		explosion.transform.parent = transform.parent;
		explosion.SetActive(true);
		Destroy(explosion, 3);
		Destroy(gameObject, .3f);
	}

	public void FireLaser() {
		laser.SetActive(true);
		StartCoroutine(TurningOffLaser());
		firing = true;
	}

	public void Activate() {
		currentTimer += Time.deltaTime;
	}

	private void OnTriggerStay(Collider other) {
		if (!firing && (playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
			if (otherGlassesWeapon != null) {
				otherGlassesWeapon.Activate();
			}
			currentTimer += Time.deltaTime;
			if (currentTimer >= timeBeforePushing) {
				other.GetComponent<Rigidbody>().AddForce(transform.forward * pushingForce);
				FireLaser();
				if (otherGlassesWeapon != null) {
					otherGlassesWeapon.FireLaser();
				}
			}
		}
	}
}