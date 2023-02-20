using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Aesthetic {
	public class PlayerGun : MonoBehaviour {
		[SerializeField] private PlayerController playerController;
		[SerializeField] private Transform _muzzle;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private GameObject _bigBullet;
		[SerializeField] private AudioClip[] _shootClips;
		private AudioSource _audioSource;

		private void Awake() {
			_audioSource = GetComponent<AudioSource>();
		}

		private void Start() {
			playerController.OnFire += OnFire;
			playerController.OnBigFire += OnBigFire;
		}

		private void OnBigFire() {
			var instance = Instantiate(_bigBullet, _muzzle.position, _muzzle.rotation);
		}

		private void OnFire() {
			_audioSource.clip = _shootClips[Random.Range(0, _shootClips.Length)];
			_audioSource.pitch = Random.Range(0.9f, 1.1f);
			_audioSource.Play();
			var instance = Instantiate(_prefab, _muzzle.position, _muzzle.rotation);
		}
	}
}