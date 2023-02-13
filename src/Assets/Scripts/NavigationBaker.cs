using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour {
	public NavMeshSurface[] surfaces;

	[Button("Build")]
	public void Build() {
		for (int i = 0; i < surfaces.Length; i++) {
			surfaces[i].BuildNavMesh();
		}
	}
}