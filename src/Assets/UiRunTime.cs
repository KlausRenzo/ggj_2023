using Assets.Scripts.Aestetic;
using UnityEngine;
using UnityEngine.UI;

public class UiRunTime : MonoBehaviour {
	[SerializeField] private Image runImage;
	[SerializeField] private Image jumpImage;
	private PlayerController playerController;

	private void Start() {
		playerController = FindObjectOfType<PlayerController>();
		playerController.OnRun += OnRun;
		playerController.OnJump += OnJump;
	}

	private void OnJump(float percentage) {
		jumpImage.fillAmount = percentage;
	}

	private void OnRun(float percentage) {
		runImage.fillAmount = percentage;
	}
}