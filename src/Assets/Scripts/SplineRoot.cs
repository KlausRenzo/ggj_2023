using System.Collections;
using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

namespace Assets.Scripts
{
	public class SplineRoot : MonoBehaviour
	{
		public Spline spline;

		private RootSplineGenerator _rootGenerator;
		private bool _stop;

		private void ContinueBranch()
		{
			var lastPoint = spline.nodes[^1];
			var previousPoint = spline.nodes.Count > 1 ? spline.nodes[^2] : lastPoint;

			var direction = (lastPoint.Position - previousPoint.Position).normalized;
			direction += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.2f;
			var length = Random.Range(0, _rootGenerator.maxNodeLength);

			var position = lastPoint.Position + direction * length;
			var node = new SplineNode(position, direction);

			spline.AddNode(node);
			spline.nodes.Add(node);

			if(spline.nodes.Count > 1)  
				_splineExtruder.SetToUpdate();
		}


		private void Awake()
		{
		}


		private IEnumerator Timer()
		{
			while (!_stop)
			{
				yield return new WaitForSeconds(_rootGenerator.timeInterval);

				OnTick();
			}
		}


		private float startTime;
		[SerializeField] private float _timeToGrow = 10;
		private SplineExtrusion _splineExtruder;

		private void OnTick()
		{
			var decision = Random.Range(0f, 1f);
			if (decision < _rootGenerator.branchDeathProbability)
				_stop = true;
			else
				ContinueBranch();

			//_spline.startWidth = _rootGenerator.startingWidthGrowth.Evaluate((Time.time - startTime) / _timeToGrow);
		}


		public void Init(RootSplineGenerator rootGenerator)
		{
			startTime = Time.time;
			_rootGenerator = rootGenerator;
			spline = gameObject.AddComponent<Spline>();
			_splineExtruder = gameObject.AddComponent<SplineExtrusion>();
			_splineExtruder.material = rootGenerator.material;
			//_spline.startWidth = 0f;
			//_spline.endWidth = 0;
			//_spline.numCornerVertices = 1;
			//_spline.colorGradient = rootGenerator.color;
			//_spline.useWorldSpace = false;

			//_spline.SetPosition(1, Vector3.up);
			spline.AddNode(new SplineNode(Vector3.zero, Vector3.zero));

			StartCoroutine(Timer());
		}
	}
}