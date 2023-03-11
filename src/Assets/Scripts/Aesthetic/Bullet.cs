using UnityEngine;

namespace Assets.Scripts.Aesthetic {
	public class Bullet : MonoBehaviour {
		public float GetDamage() => damage;

		[SerializeField] private float damage = 1;
		[SerializeField] [Range(0, 100)] private float _initialForce = 10;
		[SerializeField] [Range(0, 10)] private float _bulletLifeTime = 3;

		private Rigidbody _rigidbody;

		private void Awake() {
			_rigidbody = this.GetComponent<Rigidbody>();
		}

		private void Start() {
			Destroy(gameObject, _bulletLifeTime);
			_rigidbody.AddForce(transform.forward * _initialForce * 100);
		}

		private void OnCollisionEnter(Collision collision) {
			var enemy = collision.gameObject.GetComponent<EnemyController>();
			if (enemy != null) {
				enemy.Kill();
			}
		}
	}
}