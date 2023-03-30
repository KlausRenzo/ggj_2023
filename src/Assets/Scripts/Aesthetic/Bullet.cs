using System;
using System.Collections;
using Aesthetic;
using UnityEngine;

namespace Assets.Scripts.Aesthetic {
	public class Bullet : MonoBehaviour {
		public float GetDamage() => damage;

		[SerializeField] private float areaDamage = 0;
		[SerializeField] private float damageRadius = 100f;
		[SerializeField] private float damage = 1;
		[SerializeField] [Range(0, 100)] private float _initialForce = 10;
		[SerializeField] [Range(0, 10)] private float _bulletLifeTime = 3;
		[SerializeField] private GameObject explosionPrefab;

		private Rigidbody _rigidbody;

		private void Awake() {
			_rigidbody = this.GetComponent<Rigidbody>();
		}

		private void Start() {
			_rigidbody.AddForce(transform.forward * _initialForce * 100);
			StartCoroutine(DestroyCountdown());
		}

		private IEnumerator DestroyCountdown() {
			float timer = 0;
			while (timer < _bulletLifeTime) {
				timer += Time.deltaTime;
				yield return null;
			}
			DestroyBullet();
		}

		private void DestroyBullet() {
			if (explosionPrefab != null) {
				var explosion = Instantiate(explosionPrefab, transform);
				explosion.transform.parent = null;
				Destroy(explosion, 15);
			}
			if (damageRadius > 0) {
				ExplosionDamage(transform.position);
			}
			Destroy(gameObject);
		}

		private void ExplosionDamage(Vector3 center) {
			var hurtables = FindObjectsByType<Hurtable>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
			foreach (Hurtable hurtable in hurtables) {
				var distance = Vector3.Distance(center, hurtable.transform.position);
				hurtable.onHurt.Invoke((1 - Mathf.InverseLerp(0, damageRadius, distance)) * areaDamage);
			}
		}

		private void OnCollisionEnter(Collision collision) {
			var enemy = collision.gameObject.GetComponent<EnemyController>();
			if (enemy != null) {
				enemy.Hurt(damage);
				//enemy.Kill();
			}
			_bulletLifeTime -= 1;
		}
	}
}