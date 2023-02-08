using Assets.Scripts.Aestetic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiRunTime : MonoBehaviour {
	[SerializeField] private RectTransform charIcon;
	[Space] [SerializeField] private Image healthImage;
	[SerializeField] private AudioSource healthAudio;
	[SerializeField] private AudioClip hurtClip;
	[SerializeField] private AudioClip deathClip;
	[Space]
	[SerializeField] private Image runImage;
	[Space]
	[SerializeField] private Image jumpImage;

	private PlayerController playerController;
	private float previousHealth = 1;

	private void Start() {
		playerController = FindObjectOfType<PlayerController>();
		playerController.OnRun += OnRun;
		playerController.OnJump += OnJump;
		playerController.OnHealth += OnHealth;
		playerController.OnHurt += OnHurt;
		playerController.OnDeath += OnDeath;
	}

	private void OnDeath() {
		healthAudio.clip = deathClip;
		healthAudio.volume = Random.Range(0.9f, 1f);
		healthAudio.Play();
	}

	private void OnHurt() {
		charIcon.DOKill();
		charIcon.DORotate(Vector3.zero, 0);
		charIcon.GetComponent<Image>().DOColor(Color.red, 0);
		charIcon.GetComponent<Image>().DOColor(Color.white, 1);
		charIcon.DOShakeRotation(1f, 45, 30, 45);
		healthAudio.volume = Random.Range(0.8f, 1f);
		healthAudio.clip = hurtClip;
		healthAudio.Play();
	}

	private void OnHealth(float percentage) {
		healthImage.fillAmount = percentage;
		previousHealth = percentage;
	}

	private void OnJump(float percentage) {
		jumpImage.fillAmount = percentage;
	}

	private void OnRun(float percentage) {
		runImage.fillAmount = percentage;
	}
}