using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Aesthetic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Aesthetic {
	public class VendingMachineManager : MonoBehaviour {
		public float health;
		[FormerlySerializedAs("spawnTrans")] [FormerlySerializedAs("spawnPoint")] [SerializeField]
		private Transform spawnTransform;
		public GameObject sambucaBottlePrefab;
		public LayerMask playerLayer;
		public LayerMask bulletLayer;
		public GameObject workingMesh;
		public GameObject brokenMesh;
		[SerializeField] private AudioClip explosionClip;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private Collider trigger;

		private void Awake() {
			brokenMesh.SetActive(false);
			audioSource.DOFade(0, .1f);
		}

		private void OnTriggerEnter(Collider other) {
			if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
				audioSource.DOKill();
				audioSource.DOFade(1, 1);
			}
		}

		private void OnTriggerExit(Collider other) {
			if ((playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
				audioSource.DOKill();
				audioSource.DOFade(0, 1);
			}
		}

		private void OnCollisionEnter(Collision collision) {
			if (health > 0 && (bulletLayer == (bulletLayer | (1 << collision.gameObject.layer)))) {
				health += collision.gameObject.GetComponent<Bullet>().GetDamage();
				SpawnBottle();
				if (health <= 0) {
					DestroyMachine();
				}
			}
		}

		private void SpawnBottle() {
			var newBottle = Instantiate(sambucaBottlePrefab, spawnTransform);
			newBottle.transform.parent = null;
			newBottle.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(100, 200), spawnTransform.position - spawnTransform.forward, 5, 0);
		}

		private void DestroyMachine() {
			trigger.enabled = false;
			audioSource.Stop();
			audioSource.loop = false;
			audioSource.PlayOneShot(explosionClip);
			workingMesh.SetActive(false);
			brokenMesh.SetActive(true);
		}
	}
}