using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
	public Rect ScrollArea;
	
	[Range(0.1f, 1)] public float ScrollSpeed = .2f;

	private float _screenWidth;
	private float _screenHeight;
	[HideInInspector] public float GapBetweenItems;
	[HideInInspector] public bool OneItemInScreen;

	[Range(.1f, 1)] public float ScaleFactor = 1;

	public Transform ItemsParentTransform;

	public List<GameObject> ScrollList;
	private float _currentMaxX = -1;

	private bool _isScrolling;
	private float _scrollStartTime;
	private float _scrollStartPosX;
	private float _prevTouchPosX;
		
	private float _offset;

	private Transform transform
	{
		get{
			if (GetComponent<RectTransform>())
			{
				return GetComponent<RectTransform>();
			}
			else
			{
				return base.transform;
			}
		}
	}
	
	public int SelectedObjectIndex { get; private set; }
		
	private float _destPositionX;

	private GameObject _firstItem, _lastItem;
	private List<GameObject> _itemsInScroll;

	private Vector2 _initialPosition;
	
	public void Setup()
	{
		_screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
		_screenHeight = Camera.main.orthographicSize * 2;
		
		_itemsInScroll = new List<GameObject>();
		_initialPosition = transform.position;

		if (OneItemInScreen)
		{
			GapBetweenItems = _screenWidth - (ScrollList.Count > 0 ? ScrollList[0].GetBoundingBoxOfUiCanvas().width : 0);
		}

		if (ScrollArea.width <= 0 + Mathf.Epsilon)
		{
			ScrollArea.xMin = -_screenWidth * .5f;
			ScrollArea.width = _screenWidth;
		}
		if (ScrollArea.height <= 0 + Mathf.Epsilon)
		{
			ScrollArea.yMin = -_screenHeight * .5f;
			ScrollArea.height = _screenHeight;
		}
		
		ScrollList.ForEach(AddItemToScroll);
	}

	private void AddItemToScroll(GameObject scrollItem)
	{
		var item = scrollItem.activeInHierarchy ? scrollItem : Instantiate(scrollItem);
		item.transform.SetParent(ItemsParentTransform);

		var widthOfItem = scrollItem.GetBoundingBoxOfUiCanvas().width;
		
		if (_currentMaxX < 0)
		{
			_currentMaxX = -GapBetweenItems - widthOfItem * .5f;
		}
        
		item.transform.localPosition = new Vector3(_currentMaxX + GapBetweenItems + widthOfItem*.5f, 0, 0);

		_currentMaxX += GapBetweenItems + widthOfItem;

		if (_firstItem == null) _firstItem = item;
		_lastItem = item;
		
		_itemsInScroll.Add(item);
	}

	public void SetSelectedItem(int selectedIndex)
	{
		if (selectedIndex < 0 || selectedIndex > ScrollList.Count-1) return;
		
		SelectedObjectIndex = selectedIndex;
		_destPositionX = -ScrollList[selectedIndex].transform.localPosition.x;
	}

	private void Update()
	{

		if (_lastItem == null || _firstItem == null)
		{
			return;
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (ScrollArea.Contains(mousePosition) && !_isScrolling)
			{	
				_isScrolling = true;
				_scrollStartTime = Time.time;
				_scrollStartPosX = Camera.main.WorldToScreenPoint(transform.position).x;
					
				_offset = gameObject.transform.position.x - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).x;

				_prevTouchPosX = mousePosition.x;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (_isScrolling)
			{
				_isScrolling = false;

				var deltaTime = Time.time - _scrollStartTime;
				var deltaPosX = (Camera.main.WorldToScreenPoint(transform.position).x - _scrollStartPosX) / Screen.dpi;

				var velocity = deltaPosX / deltaTime;

				if (velocity > 1.2f && transform.GetChild(SelectedObjectIndex).position.x > 0)
				{
					SelectedObjectIndex--;
				}
				else if (velocity < -1.2f && transform.GetChild(SelectedObjectIndex).position.x < 0)
				{
					SelectedObjectIndex++;
				}
				else
				{
					var closestDistanceToCenter = 1000f ;
					
					for (var i = 0; i < transform.childCount; i++)
					{
						if (Mathf.Abs(transform.GetChild(i).position.x - _initialPosition.x) < closestDistanceToCenter)
						{
							closestDistanceToCenter = Mathf.Abs(transform.GetChild(i).position.x - _initialPosition.x);
							SelectedObjectIndex = i;
						}
					}
					
					SelectedObjectIndex = Mathf.Clamp(SelectedObjectIndex, 0, transform.childCount - 1);
				}

				SelectedObjectIndex = Mathf.Clamp(SelectedObjectIndex, 0, transform.childCount - 1);
				var selectedObject = transform.GetChild(SelectedObjectIndex);

				_destPositionX = -selectedObject.localPosition.x;
			}
		}
		else if (Input.GetMouseButton(0) && _isScrolling)
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			if (Mathf.Sign(mousePosition.x) * Mathf.Sign(_prevTouchPosX) < 0)
			{
				_scrollStartTime = Time.time;
				_scrollStartPosX = Camera.main.WorldToScreenPoint(transform.position).x;
			}

			var curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
			var curPositionX = Camera.main.ScreenToWorldPoint(curScreenPoint).x + _offset;

			if (curPositionX < -_lastItem.transform.localPosition.x - GapBetweenItems*.5f)
			{
				curPositionX = -_lastItem.transform.localPosition.x - GapBetweenItems * .5f;
				
				_offset = curPositionX - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).x;
			}
			else if (curPositionX > -_firstItem.transform.localPosition.x + GapBetweenItems*.5f)
			{
				curPositionX = -_firstItem.transform.localPosition.x + GapBetweenItems * .5f;
				
				_offset = curPositionX - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).x;
			}
				
			transform.position = new Vector2(curPositionX * .5f + transform.position.x * .5f, transform.position.y);

			
		}

		SetObjectScale(SelectedObjectIndex);
		SetObjectScale(SelectedObjectIndex - 1);
		SetObjectScale(SelectedObjectIndex + 1);

		if (!_isScrolling)
		{
			transform.position = new Vector2(transform.position.x*(1-ScrollSpeed) + _destPositionX*ScrollSpeed, transform.position.y);
		}
	}

	private void SetObjectScale(int index)
	{

		if (index < 0 || index > _itemsInScroll.Count - 1) return;
		
		if (ScaleFactor == 1)
		{
			_itemsInScroll[index].transform.localScale = Vector3.one;
		}
		else
		{
			var objectTransform = _itemsInScroll[index].transform;
			var scaleFactor = ((ScaleFactor - 1) / (20*(1-ScaleFactor))) * Mathf.Abs(objectTransform.transform.position.x - _initialPosition.x) + 1;
			objectTransform.localScale =
				Vector3.one * Mathf.Clamp(scaleFactor, ScaleFactor, 1f); //TO DO Editle
		}
	}

}

#if UNITY_EDITOR
[CustomEditor(typeof(ScrollManager))]
public class ScrollManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		var myScript = target as ScrollManager;

		if (myScript == null) return;
		
		myScript.OneItemInScreen = GUILayout.Toggle(myScript.OneItemInScreen, "Default Gap Between Items");
     
		if(!myScript.OneItemInScreen)
			myScript.GapBetweenItems = EditorGUILayout.Slider("Gap Between Items", myScript.GapBetweenItems , 0f , 5f);
 
	}
}
#endif