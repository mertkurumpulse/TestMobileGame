using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolManager
{
	private readonly List<GameObject> _units = new List<GameObject>();
        
	private readonly List<GameObject> _objectsInPool = new List<GameObject>();

	public PoolManager()
	{
		for (var index = 0; index < _objectsInPool.Count; index++)
		{
			_objectsInPool[index].tag = index.ToString();
		}
	}

	public void SetUnits(List<GameObject> units)
	{
		_units.Clear();
		foreach (var unit in units)
		{
			_units.Add(unit);
		}
	}
        
	public void AddObjectToPool(GameObject gameObject)
	{
		gameObject.SetActive(false);
		_objectsInPool.Add(gameObject);
	}

	public GameObject GetObjectFromPool(int index)
	{ 
		foreach (var item in _objectsInPool)
		{
			if (item.name.Equals(_units[index].name))
			{
				item.SetActive(true);
				_objectsInPool.Remove(item);
				return item;
			}
		}
		
		var gameObject = Object.Instantiate(_units[index]);
		gameObject.name = _units[index].name;
		gameObject.SetActive(true);
		return gameObject;
	}

	public void ClearPool()
	{
		foreach (var gameObject in _objectsInPool)
		{
			Object.Destroy(gameObject);
		}
		
		_objectsInPool.Clear();
	}

	public int GetObjectCount()
	{
		return _units.Count;
	}
}