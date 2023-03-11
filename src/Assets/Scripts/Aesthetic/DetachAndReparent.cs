using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachAndReparent : MonoBehaviour {
	public Transform newParent;

	private void Awake() {
		transform.parent = newParent;
	}
}