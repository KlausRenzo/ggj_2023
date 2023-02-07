using UnityEngine;

namespace Assets.Scripts.Aestetic
{
	public class PlayerInput : MonoBehaviour
	{
		public bool fire;
		public bool run;
		public bool jump;
		public Vector3 movementDirection;
		public Vector3 viewDelta;

		private void Update()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical = Input.GetAxis("Vertical");
			movementDirection = new Vector3(horizontal, 0, vertical);
			jump = Input.GetButtonDown("Jump");
			fire = Input.GetButtonDown("Fire1");
			run = Input.GetButton("Run");
			viewDelta = new Vector3(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
		}
    }
}