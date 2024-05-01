using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void ShowCoin(float delay)
	{
		iTween.Stop(gameObject);
		iTween.ScaleTo(gameObject, new Hashtable
		{
			{"x", .8f},
			{"y", .6f},
			{"delay", delay},
			{"time", .4f},
			{"easetype",iTween.EaseType.easeOutBack},
			{"oncomplete","ShowAnimationCompleted"}
		});
	}
	
	public void CollectCoin()
	{
		
	}
	
	
	private void ShowAnimationCompleted()
	{
		iTween.ScaleTo(gameObject, new Hashtable
		{
			{"x", gameObject.transform.localScale.x * .78f},
			{"y", gameObject.transform.localScale.y * .78f},
			{"time", 1f},
			{"loopType","pingPong"},
			{"easetype",iTween.EaseType.easeInOutSine}
		});
	}
}
