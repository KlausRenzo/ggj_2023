using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class Root : MonoBehaviour
	{
		private LineRenderer _lineRenderer;

		private RootGenerator _rootGenerator;
		private bool _stop;

		private void ContinueBranch()
		{
			var lastPoint = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
			var previousPoint = _lineRenderer.positionCount > 1 ? _lineRenderer.GetPosition(_lineRenderer.positionCount - 2) : lastPoint;

			var direction = (lastPoint - previousPoint).normalized;
			direction += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.2f;
			var length = Random.Range(0, _rootGenerator.maxNodeLength);

			var newPoint = lastPoint + direction * length;
			_lineRenderer.positionCount++;
			_lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newPoint);
		}

		public int PositionCount => _lineRenderer.positionCount;

		private void Awake()
		{
		}

		public Vector3 this[int i]
		{
			get => _lineRenderer.GetPosition(i);
			set => _lineRenderer.SetPosition(i, value);
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

		private void OnTick()
		{
			var decision = Random.Range(0f, 1f);
			if (decision < _rootGenerator.branchDeathProbability)
				_stop = true;
			else
				ContinueBranch();

			_lineRenderer.startWidth = _rootGenerator.startingWidthGrowth.Evaluate((Time.time - startTime) / _timeToGrow);
		}


		public void Init(RootGenerator rootGenerator)
		{
			startTime = Time.time;
			_rootGenerator = rootGenerator;
			_lineRenderer = gameObject.AddComponent<LineRenderer>();
			_lineRenderer.material = rootGenerator.material;
			_lineRenderer.startWidth = 0f;
			_lineRenderer.endWidth = 0;
			_lineRenderer.numCornerVertices = 1;
			_lineRenderer.colorGradient = rootGenerator.color;
			_lineRenderer.useWorldSpace = false;

			_lineRenderer.SetPosition(1, Vector3.up);

			StartCoroutine(Timer());
		}
	}
}