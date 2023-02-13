using UnityEngine;

namespace Assets.Scripts.Aesthetic
{
	public class Killer : MonoBehaviour
	{
		public void OnTriggerEnter(Collider other)
		{
			Destroy(other.gameObject);
		}
	}
}