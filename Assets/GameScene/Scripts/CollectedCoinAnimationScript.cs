using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedCoinAnimationScript : MonoBehaviour {

	public SpriteRenderer SpriteRenderer;
	
	public event Action<GameObject> onCompletedAction;
	
	// Use this for initialization
	void Awake ()
	{
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Refresh()
	{
		transform.localScale = Vector3.one*0.05f;
		iTween.Stop(gameObject);
		SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b,1);
	}

	public void AnimatePoint()
	{
		
		iTween.Stop(gameObject);
		
		var animationDuration = .45f;
		
		Hashtable tweenParams = new Hashtable
		{
			{"from", SpriteRenderer.color},
			{"to", new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b,0)},
			{"time", animationDuration-.1f},
			{"easetype",iTween.EaseType.easeInSine},
			{"onupdate", "OnColorUpdated"},
			{"delay",.1f},
			{"oncomplete", "OnColorCompleted"}
		};

		iTween.ValueTo(gameObject, tweenParams);

		
		iTween.ScaleTo(gameObject, new Hashtable
		{
			{"x", .5f},
			{"y", .5f},
			{"time", animationDuration},
			{"easetype",iTween.EaseType.easeOutSine}
		});
		
	}
	
	
	private void OnColorUpdated(Color color)
	{
		SpriteRenderer.color = color;
	}
	
	private void OnColorCompleted()
	{
		Refresh();
		
		if (onCompletedAction!=null)
		{
			onCompletedAction(gameObject);
		}
	}
}
