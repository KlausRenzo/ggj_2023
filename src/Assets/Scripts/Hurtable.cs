using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Aesthetic {
	public class Hurtable : MonoBehaviour {
		public UnityEvent<float> onHurt;

		public void Hurt(float healthModifier) {
			onHurt.Invoke(healthModifier);
		}
	}
}