using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingCollapser : MonoBehaviour {
	[SerializeField] private GameObject collapsingPrefab;

	private float height;
	[SerializeField, ReadOnly] private GameObject clone;

	private void Start() {
		height = transform.localScale.y;
		clone = Instantiate(collapsingPrefab, null);
		clone.transform.position = transform.position;
		clone.transform.rotation = transform.rotation;
		clone.transform.localScale = transform.localScale;

		clone.SetActive(false);
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("BossFoot")) {
			Collapse();
		}
	}

	[Button]
	private void Collapse() {
		clone.SetActive(true);
		clone.transform.DOMoveY(-height, 3).OnComplete(() => { Destroy(clone.gameObject); });
		gameObject.SetActive(false);
	}
}