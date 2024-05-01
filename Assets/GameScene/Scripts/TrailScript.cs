using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrailScript : MonoBehaviour
{

	private LineRenderer _lineRenderer;

	private Vector3[] _lineRendererPositions;
	private LinkedList<float> _creationTimes;

	private Vector3 _lastPosition;

	private float _lifeTime;

	private void Reset()
	{
		if (GetComponent<LineRenderer>() == null)
		{
			gameObject.AddComponent<LineRenderer>();
		}
	}

	private void Awake()
	{
		_lastPosition = transform.position;
		_creationTimes = new LinkedList<float>();
		_lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		_lineRendererPositions = new Vector3[_lineRenderer.positionCount];
		_lineRenderer.GetPositions(_lineRendererPositions);

		if (_creationTimes.Count > 0 && Time.time - _creationTimes.Last.Value > _lifeTime)
		{
			while (_creationTimes.Count > 0 && Time.time - _creationTimes.Last.Value > _lifeTime)
			{
				var lineRendererPositions = new LinkedList<Vector3>(_lineRendererPositions);
				_creationTimes.RemoveLast();
				lineRendererPositions.RemoveLast();
				_lineRendererPositions = lineRendererPositions.ToArray();
			}
			_lineRenderer.positionCount = _lineRendererPositions.Length;
			_lineRenderer.SetPositions(_lineRendererPositions);
		}
		
		if (_lifeTime > 0 && Vector2.SqrMagnitude(_lastPosition - transform.position) > Mathf.Epsilon)
		{
			_creationTimes.AddFirst(Time.time);
			var lineRendererPositions = new LinkedList<Vector3>(_lineRendererPositions);
			lineRendererPositions.AddFirst(transform.position);
			_lineRenderer.positionCount++;
			_lineRenderer.SetPositions(lineRendererPositions.ToArray());

			_lastPosition = transform.position;
		}
		
	}

	public void PauseTrail()
	{
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", _lifeTime},
			{"to", 0},
			{"time", 1},
			{"onupdatetarget", gameObject},
			{"onupdate", "SetLifeTime"},
			{"easetype", iTween.EaseType.easeOutSine}
		});
		
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", _lineRenderer.startWidth},
			{"to", 0},
			{"time", 1},
			{"onupdatetarget", gameObject},
			{"onupdate", "SetWidth"},
			{"easetype", iTween.EaseType.easeOutSine}
		});
	}

	public void ResumeTrail()
	{
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", _lifeTime},
			{"to", .2f},
			{"time", 1},
			{"onupdatetarget", gameObject},
			{"onupdate", "SetLifeTime"},
			{"easetype", iTween.EaseType.easeInSine}
		});
		
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", _lineRenderer.startWidth},
			{"to", .45f},
			{"time", .1f},
			{"onupdatetarget", gameObject},
			{"onupdate", "SetWidth"},
			{"easetype", iTween.EaseType.easeInSine}
		});
	}

	public void StopTrail()
	{
		_creationTimes = new LinkedList<float>();
		_lineRenderer.positionCount = 0;
	}

	private void SetLifeTime(float lifeTime)
	{
		_lifeTime = lifeTime;
	}

	private void SetWidth(float width)
	{
		return;
		_lineRenderer.startWidth = width;
		_lineRenderer.endWidth = width;
	}
}
