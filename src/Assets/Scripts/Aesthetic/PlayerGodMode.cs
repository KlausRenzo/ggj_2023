using UnityEngine;

namespace Assets.Scripts.Aestetic {
	public class PlayerGodMode : MonoBehaviour {
		private PlayerController playerController;
		[SerializeField] private string log = "";

		private void Awake() {
			playerController = GetComponent<PlayerController>();
		}

		private void Update() {
			for (int i = 0; i < 256; i++) {
				if (Input.GetKeyDown((KeyCode)i)) {
					log += (KeyCode)i;
				}
			}
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.G)) {
				playerController.godMode = !playerController.godMode;
			}
#endif
		}
	}
}