using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingCollapser : MonoBehaviour {
	[SerializeField] private GameObject collapsingPrefab;
	[SerializeField] private float collapsingTime = 3;

	private float height;
	[SerializeField, ReadOnly] private GameObject clone;

	private void Start() {
		height = transform.localScale.y;
		clone = Instantiate(collapsingPrefab, null);
		clone.transform.position = transform.position;
		clone.transform.rotation = transform.rotation;
		clone.transform.localScale = transform.localScale;
		collapsingTime = transform.localScale.y;
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
		clone.transform.DOMoveY(-height, collapsingTime).OnComplete(() => { Destroy(clone.gameObject); });
		gameObject.SetActive(false);
	}
}