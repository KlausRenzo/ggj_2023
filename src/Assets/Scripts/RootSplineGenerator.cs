using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
	public class RootSplineGenerator : MonoBehaviour
	{
		public Material material;
		[SerializeField] [Range(0, 5)] public float timeInterval = 1;
		[SerializeField] [Range(0, 1)] public float branchDeathProbability = 0.5f;
		[SerializeField] [Range(0, 1)] private float directionRandomizer = 0.5f;
		[SerializeField] [Range(0, 0.5f)] public float maxNodeLength = 0.25f;
		[SerializeField] public Gradient color;
		private List<SplineRoot> _roots = new();
		public AnimationCurve startingWidthGrowth;

		private void Start()
		{
			CreateBranch();
			StartCoroutine(Tick());
		}

		private IEnumerator Tick()
		{yield break;

			while (true)
			{
				var randomWaitTime = Random.value;
				yield return new WaitForSeconds(randomWaitTime);

				var randomBranch = _roots[Random.Range(0, _roots.Count)];
				CreateBranch(randomBranch);
			}
		}

		public void CreateBranch(SplineRoot parent = null)
		{
			var child = new GameObject() { name = "Branch" };
			child.transform.SetParent(parent?.transform ?? transform);
			child.transform.localPosition = Vector3.zero;
			if (parent != null && parent.spline.nodes.Any())
			{
				child.transform.position = parent.transform.TransformPoint(parent.spline.nodes.Last().Position);
			}

			var root = child.AddComponent<SplineRoot>();
			root.Init(this);
			_roots.Add(root);
		}
	}
}