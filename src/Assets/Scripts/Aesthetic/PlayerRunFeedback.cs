using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

namespace Assets.Scripts.Aesthetic {
	public class PlayerRunFeedback : MonoBehaviour {
		private PlayerController playerController;
		[SerializeField] private Camera playerCamera;
		[Space] [SerializeField] private AudioClip runClip;
		[SerializeField] private AudioSource _runAudioSource;
		[SerializeField] private float fovDelta;
		[SerializeField] private PostProcessVolume _feedbacksPostProcess;
		private float defaultFov;

		private void Awake() {
			playerController = GetComponent<PlayerController>();
			playerController.OnRun += OnRun;
			playerController.OnRunState += OnRunState;
			defaultFov = playerCamera.fieldOfView;
		}

		private void OnRunState(bool startStopRun) {
			if (startStopRun) {
				_runAudioSource.clip = runClip;
				_runAudioSource.pitch = 1f + Random.Range(-0.1f, 0.1f);
				_runAudioSource.Play();
				float fov = playerCamera.fieldOfView;
				DOTween.To(() => fov, x => fov = x, defaultFov + fovDelta, 0.1f)
					.OnUpdate(() => { playerCamera.fieldOfView = fov; });

				float intensity = 1;
				float endIntensity = 0;
				DOTween.To(() => intensity, x => intensity = x, endIntensity, 0.25f)
					.OnUpdate(() => { _feedbacksPostProcess.weight = intensity; });
			}
			else {
				float fov = playerCamera.fieldOfView;
				DOTween.To(() => fov, x => fov = x, defaultFov, 0.5f).OnUpdate(() => { playerCamera.fieldOfView = fov; });
			}
		}

		private void OnRun(float runPercentage) { }
	}
}