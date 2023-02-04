using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
	public class RootGenerator : MonoBehaviour
	{
		public Material material;
		[SerializeField] [Range(0, 5)] public float timeInterval = 1;
		[SerializeField] [Range(0, 1)] public float branchDeathProbability = 0.5f;
		[SerializeField] [Range(0, 1)] private float directionRandomizer = 0.5f;
		[SerializeField] [Range(0, 0.5f)] public float maxNodeLength = 0.25f;
		[SerializeField] public Gradient color;
		private List<Root> _roots = new();
		public AnimationCurve startingWidthGrowth;

		private void Start()
		{
			CreateBranch();
			StartCoroutine(Tick());
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				var randomWaitTime = Random.value;
				yield return new WaitForSeconds(randomWaitTime);

				var randomBranch = _roots[Random.Range(0, _roots.Count)];
				CreateBranch(randomBranch);
			}
		}

		public void CreateBranch(Root parent = null)
		{
			var child = new GameObject() { name = "Branch" };
			child.transform.SetParent(parent?.transform ?? transform);
			child.transform.localPosition = Vector3.zero;
            if (parent != null)
			{
				child.transform.position = parent.transform.TransformPoint(parent[parent.PositionCount - 1]);
			}

			var root = child.AddComponent<Root>();
			root.Init(this);
			_roots.Add(root);
		}
	}
}