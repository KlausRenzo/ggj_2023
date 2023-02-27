using System;
using Assets.Scripts.Aesthetic;
using UnityEngine;

namespace Aesthetic {
	public class HealthModifier : MonoBehaviour {
		[Space] public float damage = 1f;
		[Space] public bool destroyOnCollision;
		public LayerMask playerLayer;

		private void OnCollisionEnter(Collision other) {
			if (destroyOnCollision && (playerLayer == (playerLayer | (1 << other.gameObject.layer)))) {
				Destroy(gameObject, Time.deltaTime);
			}
		}

		public void Consume() { }
	}
}