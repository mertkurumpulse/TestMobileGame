using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class RaceTrackManager : MonoBehaviour
{

	public Constants.Maps MapType;
	
	public GameObject Shadow;
	private Color[] _shadowColors;
	
	private GameObject[] _checkpointGameObjects; 

	public List<Sprite> ModeSprites;
	
	public List<Sprite> RopeSprites;

	public GameObject Bridge, Tunnel;
	
	public List<PinManager> Pins;

	public List<GameObject> BridgeActiveCheckPoints;
	public List<Sprite> BridgeSprites;

	public List<Color> ScoreEffectColors
	{
		get
		{
			return new List<Color>
			{
				new Color32(40, 40, 40, 255),
				new Color32(150, 255, 150, 255),
				new Color(0, .85f, .9f, .8f)
			};
		}
	}

	public List<Color> ScoreColors
	{
		get
		{
			return new List<Color>
			{
				new Color32(0,0,0,100),
				new Color32(150, 255, 150, 50),
				new Color(0, .95f, 1f, .15f)
			};
		}
	}

	public bool BridgeEnabledByDefault = true;
	
	
	public Vector3 CarInitialPosition;
	public float CarInitialZRotation;

	private void Awake()
	{
		_shadowColors = new Color[3];
		_shadowColors[0] = new Color(.11f, .11f, .11f, 1f);
		_shadowColors[1] = new Color(.11f, .5f, 1f);
		_shadowColors[2] = Color.white;//new Color(.5f, .98f, 1f);

		_checkpointGameObjects = GameObject.FindGameObjectsWithTag(Tags.StartFinish);
		
		SetupBridge(null);
	}

	public void ChangeMode(Modes mode)
	{
		GetComponent<SpriteRenderer>().sprite = ModeSprites[(int) mode];
		Shadow.GetComponent<SpriteRenderer>().color = _shadowColors[(int) mode];

		foreach (var pinManager in Pins)
		{
			pinManager.RopeManager.SetMode(mode);
		}

		if (Bridge)
		{
			Bridge.GetComponent<SpriteRenderer>().sprite = BridgeSprites[(int) mode];
		}
	}

	public void SetupBridge(GameObject checkPoint)
	{
		if (Bridge == null) return;
		
		if (BridgeActiveCheckPoints.Contains(checkPoint)|| (checkPoint == null && BridgeEnabledByDefault))
		{
			Bridge.transform.GetChild(0).gameObject.SetActive(true);
			Bridge.GetComponent<SpriteRenderer>().sortingOrder = 1;
			
			Tunnel.SetActive(false);
		}
		else
		{
			Bridge.transform.GetChild(0).gameObject.SetActive(false);
			Bridge.GetComponent<SpriteRenderer>().sortingOrder =
				GamePlayManager.Instance.Car.GetComponent<SpriteRenderer>().sortingOrder + 5;
			
			Tunnel.SetActive(true);
		}

		GamePlayManager.Instance.WorldSpaceCanvas.GetComponent<Canvas>().sortingOrder =
			Bridge.GetComponent<SpriteRenderer>().sortingOrder + 1;
	}
}
