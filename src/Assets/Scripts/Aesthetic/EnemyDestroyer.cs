using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Aesthetic;
using UnityEngine;

public class EnemyDestroyer : MonoBehaviour {
	public LayerMask layermask;

	private void OnTriggerEnter(Collider other) {
		var ec = other.GetComponent<EnemyController>();
		if (ec!=null) {
			ec.Kill();
		}
	}

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }
}