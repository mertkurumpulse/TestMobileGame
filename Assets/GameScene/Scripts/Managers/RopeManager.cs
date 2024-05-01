using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{

	public GameObject RopeSegment;
	private List<GameObject> _ropeSegments;

	private PoolManager _ropeSegmentPool;

	private Vector2 _from, _to;

	private float _ropeSegmentWidth;

	private float _createdRopeSegmentCount;
	private bool _ropeIsCreating;
	private float _createRopeStartTime;
	
	public bool RopeIsActive;

	private void Start()
	{
		_ropeSegmentWidth = GetRopeSegmentWidth();

		_ropeSegmentPool = new PoolManager();
		_ropeSegmentPool.SetUnits(new List<GameObject> {RopeSegment});

		_ropeSegments = new List<GameObject>();
	}

	public void SetMode(Modes mode)
	{
		if(_ropeSegmentPool != null) _ropeSegmentPool.ClearPool();

		RopeSegment.GetComponent<SpriteRenderer>().sprite = GamePlayManager.Instance.Track.RopeSprites[(int) mode];
	}
	
	public void CreateRope()
	{
		DestroyRope();
		
		_from = GamePlayManager.Instance.Car.transform.position;
		_to = Vector2.Lerp(transform.position, _from, .15f / Vector2.Distance(transform.position, _from));

		var ropeCount = Mathf.CeilToInt(Vector2.Distance(_from, _to) /
		                                _ropeSegmentWidth);

		var rotation = Quaternion.FromToRotation(Vector3.right, _from - _to);
		
		for (var i = 0; i < ropeCount; i++)
		{
			var ropeSegment = _ropeSegmentPool.GetObjectFromPool(0);
			ropeSegment.transform.position = Vector3.Lerp(_from, _to,
				(float) i / ropeCount);

			ropeSegment.transform.rotation = rotation;
			
			ropeSegment.transform.SetParent(transform);
			_ropeSegments.Add(ropeSegment);
		}
		
		if (!RopeIsActive)
		{
			if (!_ropeIsCreating)
			{
				_createdRopeSegmentCount = 0;
				_ropeIsCreating = true;

				_createRopeStartTime = Time.time;
			}
			

			for (var i = 0; i < _ropeSegments.Count; i++)
			{
				_ropeSegments[i].SetActive(Time.time - _createRopeStartTime > i*.007f);
			}

			if (Time.time - _createRopeStartTime > _ropeSegments.Count*.007f)
			{
				RopeIsActive = true;
				_ropeIsCreating = false;
			}

		}	
	}

	public void DestroyRope()
	{	
		foreach (var ropeSegment in _ropeSegments)
		{
			_ropeSegmentPool.AddObjectToPool(ropeSegment);
		}
		_ropeSegments.Clear();
	}

	private float GetRopeSegmentWidth()
	{
		Vector2 spriteSize = RopeSegment.GetComponent<SpriteRenderer>().sprite.rect.size;
		Vector2 localSpriteSize = spriteSize / RopeSegment.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
		Vector3 worldSize = localSpriteSize;
		worldSize.x *= gameObject.transform.lossyScale.x;
		return worldSize.x;
	}
}
