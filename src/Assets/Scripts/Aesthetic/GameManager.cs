using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Aestetic {
	public class GameManager : MonoBehaviour {
		public int killCounter;

		[FormerlySerializedAs("_text")] [SerializeField]
		private TextMeshProUGUI _killLabel;
		private EnemySpawner enemySpawner;

		public void OnKill() {
			killCounter++;
			_killLabel.text = killCounter.ToString("000");
		}
	}
}