using UnityEngine;

namespace Assets.Scripts.Aestetic
{
	public class Killer : MonoBehaviour
	{
		public void OnTriggerEnter(Collider other)
		{
			Destroy(other.gameObject);
		}
	}
}