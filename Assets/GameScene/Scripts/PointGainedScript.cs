using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointGainedScript : MonoBehaviour
{

	public GameObject TextCanvas;
	private Text _text;
	
	public event Action<GameObject> onCompletedAction;
	
	// Use this for initialization
	void Awake ()
	{
		_text = TextCanvas.GetComponent<Text>();
	}

	private void Refresh()
	{
		transform.localScale /= 1.1f;
		iTween.Stop(gameObject);
		_text.color = new Color(_text.color.r, _text.color.g, _text.color.b,1);
	}

	public void AnimatePoint(int point)
	{
		
		iTween.Stop(gameObject);
		
		_text.text = "+" + point;
		var animationDuration = 1f;
		
		Hashtable tweenParams = new Hashtable
		{
			{"from", _text.color},
			{"to", new Color(_text.color.r, _text.color.g, _text.color.b,0)},
			{"time", animationDuration-.2f},
			{"easetype",iTween.EaseType.easeInSine},
			{"onupdate", "OnColorUpdated"},
			{"delay",.2f},
			{"oncomplete", "OnColorCompleted"}
		};

		iTween.ValueTo(gameObject, tweenParams);

		
		iTween.MoveTo(gameObject, new Hashtable
		{
			{"y",transform.position.y + .6f},
			{"time", animationDuration},
			{"easetype",iTween.EaseType.easeOutSine}
		});
		
		iTween.ScaleBy(gameObject, new Hashtable
		{
			{"x", 1.1},
			{"y", 1.1},
			{"time", animationDuration},
			{"easetype",iTween.EaseType.easeOutSine}
		});
		
	}
	
	private void OnColorUpdated(Color color)
	{
		_text.color = color;
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
