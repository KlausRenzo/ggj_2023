using System.Collections;
using System.Linq;
using SplineMesh;
using Unity.VisualScripting;
using UnityEngine;
using ExtrusionSegment = SplineMesh.ExtrusionSegment;

namespace Assets.Scripts
{
	public class SplineGenerator : MonoBehaviour
	{
		private Spline spline;
		private SplineExtrusion _splineExtruder;

		private void Awake()
		{
			spline = this.GetComponent<Spline>();
			_splineExtruder = this.GetComponent<SplineExtrusion>();
		}

		private IEnumerator Start()
		{
			spline.AddNode(new SplineNode(Vector3.zero, Vector3.up));
			var lastPoint = Vector3.zero;
			var previousPoint = Vector3.one;

            for (int i = 0; i < 15; i++)
			{
				var direction = (lastPoint - previousPoint).normalized;
				direction += new Vector3(Random.value, Random.value, Random.value) * 0.2f;
				var length = Random.Range(0, 5f);

				var position = lastPoint + direction * length;
				var node = new SplineNode(position, (position - lastPoint).normalized * 0.01f);

				spline.AddNode(node);
				previousPoint = lastPoint;
				lastPoint = position;
			}


			yield return null;
			spline.RefreshCurves();


			yield return null;
			_splineExtruder.SetToUpdate();
		}
	}
}