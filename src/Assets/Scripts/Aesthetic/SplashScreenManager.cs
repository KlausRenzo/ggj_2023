using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour {
	private bool isLoading;
	[SerializeField] private CanvasGroup fadeoutImage;
	private AudioSource audioSource;

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	public void LoadGame() {
		if (!isLoading) {
			isLoading = true;
			StartCoroutine(LoadYourAsyncScene());
		}
	}

	IEnumerator LoadYourAsyncScene() {
		fadeoutImage.DOFade(1, 0.5f);
		audioSource.Play();
		while (audioSource.isPlaying) {
			yield return null;
		}
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
		while (!asyncLoad.isDone) {
			yield return null;
		}
	}
}