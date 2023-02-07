using Assets.Scripts.Aestetic;
using UnityEngine;
using UnityEngine.UI;

public class UiRunTime : MonoBehaviour {
	[SerializeField] private Image image;
	private PlayerController playerController;

	private void Start() {
		playerController = FindObjectOfType<PlayerController>();
		playerController.OnRun += OnRun;
	}

	private void OnRun(float percentage) {
		image.fillAmount = percentage;
	}
}