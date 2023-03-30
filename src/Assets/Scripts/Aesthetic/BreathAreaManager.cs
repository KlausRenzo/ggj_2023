using Assets.Scripts.Aesthetic;
using UnityEngine;

namespace Aesthetic {
	public class BreathAreaManager : MonoBehaviour {
		[SerializeField] private float damagePerSecond;

		private void OnTriggerStay(Collider other) {
			var playerController = other.GetComponent<PlayerController>();
			if (playerController != null) {
				playerController.Health(damagePerSecond * Time.deltaTime);
			}
		}
	}
}