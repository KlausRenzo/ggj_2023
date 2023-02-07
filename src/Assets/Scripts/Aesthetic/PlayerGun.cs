using UnityEngine;

namespace Assets.Scripts.Aestetic
{
	public class PlayerGun : MonoBehaviour
	{
		[SerializeField] private GameObject _prefab;

		public void Shoot()
		{
			var instance = Instantiate(_prefab, transform.position, transform.rotation);
		}
	}
}