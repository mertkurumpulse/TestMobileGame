using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftTrailTimer : MonoBehaviour
{

	private LineRenderer _lineRenderer;
	
	private float _driftTrailEffectTime = 2;
	private float _time;
	
	private void Start ()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		_time += Time.deltaTime;

		_lineRenderer.startColor = Color.Lerp(_lineRenderer.startColor, new Color(20/255f, 20/255f, 20/255f, 0), _time/_driftTrailEffectTime);
		_lineRenderer.endColor = Color.Lerp(_lineRenderer.endColor, new Color(0, 0, 0, 0), _time/(_driftTrailEffectTime*3));

		if (_lineRenderer.endColor.a < .05)
		{
			Destroy(gameObject);
		}
	}
}
