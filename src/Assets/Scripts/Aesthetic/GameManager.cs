using TMPro;
using UnityEngine;

namespace Assets.Scripts.Aestetic
{
	public class GameManager : MonoBehaviour
	{
		public int killCounter;

		[SerializeField] private int _reactionFrequency = 2;
		[SerializeField] private TextMeshProUGUI _text;

		public void OnKill()
		{
			killCounter++;
			_text.text = killCounter.ToString("000");

			if (killCounter % _reactionFrequency == 0)
			{
				PlayAudio();
			}
		}

		[SerializeField] private AudioSource audioSource;
		[SerializeField] private AudioClip[] clips;

		private void PlayAudio()
		{
			var randomClip = clips[Random.Range(0, clips.Length)];
			audioSource.clip = randomClip;

			audioSource.Play();
		}
	}
}