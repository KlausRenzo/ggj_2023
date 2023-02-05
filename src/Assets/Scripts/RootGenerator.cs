using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		private float _counter;

		private void Start()
		{
			CreateBranch();
			StartCoroutine(Tick());
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				if (!_roots.Any())
				{
					yield return new WaitForSeconds(0.1f);
					continue;
				}

				var randomWaitTime = Random.value;

				var randomBranch = _roots[Random.Range(0, _roots.Count)];
				CreateBranch(randomBranch);

				yield return new WaitForSeconds(randomWaitTime + (0.25f * _counter));
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
			root.Dead += () => _roots.Remove(root);
			root.Init(this);
			_roots.Add(root);
			_counter++;
		}
	}
}