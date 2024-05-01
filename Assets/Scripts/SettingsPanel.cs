using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Configuration;

public class SettingsPanel : MonoBehaviour
{

	public Button SettingsButton, SoundButton, RemoveAdsButton;

	public Sprite SoundEnabledSprite, SoundDisabledSprite;

	private bool _panelActive;

	private List<Vector2> _positions;

	private float _currentAnimationStep;
	
	// Use this for initialization
	private void Start()
	{
		_positions = new List<Vector2>
		{
			new Vector2(55, 0),
			new Vector2(55, -85),
			new Vector2(55, -170)
		};
		
		SettingsButton.GetComponent<RectTransform>().anchoredPosition = _positions[0];
		SoundButton.GetComponent<RectTransform>().anchoredPosition = _positions[0];
		RemoveAdsButton.GetComponent<RectTransform>().anchoredPosition = _positions[0];
		
		SettingsButton.onClick.AddListener(delegate {
			
			if (_panelActive)
			{
				ClosePanel();
			}
			else
			{
				OpenPanel();
			}
		});
		SoundButton.onClick.AddListener(delegate
		{
			GameManager.Instance.SoundEnabled = !GameManager.Instance.SoundEnabled;
			SoundButton.GetComponent<Image>().sprite =
				GameManager.Instance.SoundEnabled ? SoundEnabledSprite : SoundDisabledSprite;
		});
		RemoveAdsButton.onClick.AddListener(delegate {  });
	}

	private void OpenPanel()
	{
		
		_panelActive = true;
		
		var hashtables = new List<Hashtable>();

		for (int i = 0; i < 3; i++)
		{
			hashtables.Add(new Hashtable
			{
				{"from", _currentAnimationStep},
				{"to", 1},
				{"time", .1f},
				{"easetype", iTween.EaseType.easeInSine},
				{"onupdatetarget", gameObject},
				{"onupdate", "PanelAnimationStep"}
			});
		}
		
		iTween.ValueTo(SettingsButton.gameObject, hashtables[0]);
		iTween.ValueTo(SoundButton.gameObject, hashtables[1]);
		iTween.ValueTo(RemoveAdsButton.gameObject, hashtables[2]);
	}

	private void ClosePanel()
	{
		
		_panelActive = false;
		
		var hashtables = new List<Hashtable>();

		for (int i = 0; i < 3; i++)
		{
			hashtables.Add(new Hashtable
			{
				{"from", _currentAnimationStep},
				{"to", 0},
				{"time", .1f},
				{"easetype", iTween.EaseType.easeOutSine},
				{"onupdatetarget", gameObject},
				{"onupdate", "PanelAnimationStep"}
			});
		}
		
		iTween.ValueTo(SettingsButton.gameObject, hashtables[0]);
		iTween.ValueTo(SoundButton.gameObject, hashtables[1]);
		iTween.ValueTo(RemoveAdsButton.gameObject, hashtables[2]);
	}

	private void PanelAnimationStep(float step)
	{
		_currentAnimationStep = step;

		SettingsButton.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, step * 90);
		SoundButton.GetComponent<RectTransform>().anchoredPosition = _positions[1] - new Vector2(0, (1-step)*_positions[1].y);
		RemoveAdsButton.GetComponent<RectTransform>().anchoredPosition = _positions[2] - new Vector2(0, (1-step)*_positions[2].y);
	}
}
