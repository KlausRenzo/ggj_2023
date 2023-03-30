using Assets.Scripts.Aesthetic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiRunTime : MonoBehaviour {
	[SerializeField] private RectTransform charIcon;

	[Space] [SerializeField] private Image radicalizationImage;

	[Space] [SerializeField] private Image drunknessImage;
	[Space] [SerializeField] private Image healthImage;
	[Space] [SerializeField] private RectTransform bigBulletsCounterContainer;
	[Space] [SerializeField] private Image bigBulletsCounterIcon;
	[Space] [SerializeField] private TMP_Text bigBulletsCounterLabel;
	[SerializeField] private AudioSource healthAudio;
	[SerializeField] private AudioClip hurtClip;
	[SerializeField] private AudioClip healClip;
	[SerializeField] private AudioClip deathClip;
	[Space] [SerializeField] private Image runImage;
	[Space] [SerializeField] private Image jumpImage;
	[Space, Header("Enemies")] [SerializeField]
	private TMP_Text enemiesCount;
	[SerializeField] private TMP_Text enemiesKilledCount;
	private GameManager gameManager;
	private PlayerController playerController;
	private EnemySpawner enemySpawner;
	private float previousHealth = 1;

	private void Start() {
		gameManager = FindObjectOfType<GameManager>();
		gameManager.OnRadicalizationUpdate += OnRadicalizationUpdate;
		playerController = FindObjectOfType<PlayerController>();
		playerController.OnRun += OnRun;
		playerController.OnJump += OnJump;
		playerController.OnHealth += OnHealth;
		playerController.OnHurt += OnHurt;
		playerController.OnHeal += OnHeal;
		playerController.OnDeath += OnDeath;
		playerController.OnDrunkness += OnDrunkness;
		playerController.OnBigBulletsUpdated += OnBigBulletsUpdated;
		playerController.OnBigBulletPartialUpdated += OnBigBulletsChargeUpdated;
		enemySpawner = FindObjectOfType<EnemySpawner>();
		enemySpawner.OnEnemyCountUpdated += OnEnemyCountUpdated;
		enemySpawner.OnKill += OnEnemyKill;
	}

	private void OnBigBulletsChargeUpdated(float percentage) {
		bigBulletsCounterIcon.DOFillAmount(percentage, .3f);
	}

	private void OnBigBulletsUpdated(int newBigBulletsAmount) {
		bigBulletsCounterLabel.text = newBigBulletsAmount.ToString();
		ResetBigBulletImage();
		bigBulletsCounterContainer.DOPunchScale(Vector3.one * 1.5f, .5f);
	}

	private void OnRadicalizationUpdate(float percentage) {
		radicalizationImage.fillAmount = percentage;
	}

	private void ResetBigBulletImage() {
		bigBulletsCounterContainer.DOKill();
		bigBulletsCounterContainer.rotation = Quaternion.identity;
		bigBulletsCounterContainer.localScale = Vector3.one;
	}

	private void OnEnemyKill(EnemyController killedEnemy, int i) {
		enemiesKilledCount.text = $"{i:000}";
		ResetBigBulletImage();
		bigBulletsCounterContainer.DOPunchRotation(Vector3.forward * 10, .5f);
	}

	private void OnEnemyCountUpdated(int enemiesPresent) {
		enemiesCount.text = $"{enemiesPresent:000}";
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

	private void OnHeal() {
		charIcon.GetComponent<Image>().DOColor(Color.green, 0);
		charIcon.GetComponent<Image>().DOColor(Color.white, 1);
		healthAudio.volume = Random.Range(0.8f, 1f);
		healthAudio.clip = healClip;
		healthAudio.Play();
	}

	private void OnDrunkness(float percentage) {
		drunknessImage.fillAmount = percentage;
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