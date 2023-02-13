using System;
using UnityEngine;

namespace Assets.Scripts.Aesthetic
{
	public class PlayerGun : MonoBehaviour
	{
		[SerializeField] private Transform _muzzle;
		[SerializeField] private GameObject _prefab;
		private AudioSource _audioSource;

		private void Awake() {
			_audioSource = GetComponent<AudioSource>();
		}

		public void Shoot()
		{
			var instance = Instantiate(_prefab, _muzzle.position, _muzzle.rotation);
		}
	}
}