using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingBuilding : MonoBehaviour, IShakeable {
	public event Action<bool> OnShake;

	private IEnumerator Start() {
		yield return null;
		OnShake?.Invoke(true);
	}
}