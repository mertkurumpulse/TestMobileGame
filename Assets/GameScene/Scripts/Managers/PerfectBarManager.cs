using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerfectBarManager : MonoBehaviour
{

	private int _fillAmount;
	private CanvasGroup _canvasGroup;

	private readonly List<Image> _bars = new List<Image>();

	private void Start()
	{
		for (var i = 0; i < transform.childCount; i++)
		{
			_bars.Add(transform.GetChild(i).GetComponent<Image>());
		}

		_canvasGroup = GetComponent<CanvasGroup>();
	}

	public float FillBar(TurnQuality turnQuality)
	{	
		switch (turnQuality)
		{
			case TurnQuality.Bad:
			case TurnQuality.Good:

				_fillAmount = 0;

				for (var i = 0; i < _bars.Count; i++)
				{
					iTween.ColorTo(_bars[i].gameObject, new Hashtable
					{
						{"time", .2f},
						{"color", new Color(1, 1, 1, .35f)},
						{"easetype", iTween.EaseType.easeOutSine}
					});
				}
				
				break;
			default:
			case TurnQuality.Perfect:

				var fillColor = new Color(.5f, 1f, .5f, 1f);
				/*if (GamePlayManager.Instance.CurrentMode == Modes.Perfect)
				{
					fillColor = new Color(1f, 1f, .35f, 1f);
				}*/
				_fillAmount++;
				iTween.ColorTo(_bars[_fillAmount-1].gameObject, new Hashtable
				{
					{"color", fillColor},
					{"time", .2f},
					{"easetype", iTween.EaseType.easeOutSine}
				});

				break;
		}

		return (float)_fillAmount/_bars.Count;
	}

	public void ResetBars()
	{
		_fillAmount = 0;

		foreach (var bar in _bars)
		{
			iTween.ColorTo(bar.gameObject, new Hashtable
			{
				{"color", new Color(1, 1, 1, .35f)},
				{"time", .3f},
				{"delay",.3f},
				{"easetype", iTween.EaseType.easeInSine}
			});
		}
		
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", _canvasGroup.alpha},
			{"to", 1},
			{"time", .3f},
			{"delay", .3f},
			{"easetype", iTween.EaseType.easeInSine},
			{"onupdatetarget", gameObject},
			{"onupdate", "SetCanvasGroupAlpha"}
		});
	}

	public void HideBars()
	{
		_fillAmount = 0;

		foreach (var bar in _bars)
		{
			iTween.ColorTo(bar.gameObject, new Hashtable
			{
				{"color", new Color(1, 1, 1, 0)},
				{"time", .3f},
				{"delay",.3f},
				{"easetype", iTween.EaseType.easeInSine}
			});
		}
		
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", _canvasGroup.alpha},
			{"to", 0},
			{"time", .3f},
			{"delay", .3f},
			{"easetype", iTween.EaseType.easeInSine},
			{"onupdatetarget", gameObject},
			{"onupdate", "SetCanvasGroupAlpha"}
		});
	}

	private void SetCanvasGroupAlpha(float value)
	{
		_canvasGroup.alpha = value;
	}
}
