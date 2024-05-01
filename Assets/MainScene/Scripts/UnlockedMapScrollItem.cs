using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockedMapScrollItem : MonoBehaviour
{

	public Text MapName;
	public Image MapIcon;
	public Text BestScoreText;
	public Text BestScore;

	public List<Sprite> MapIcons;

	private void OnValidate()
	{
		MapIcon.sprite = MapIcons[0];
	}

	private void Start()
	{
		iTween.ValueTo(MapIcon.gameObject, new Hashtable
		{
			{"from", 0},
			{"to", 1},
			{"time", 1},
			{"easetype", iTween.EaseType.easeInOutCubic},
			{"onupdatetarget", gameObject},
			{"onupdate", "MapIconAnimation"},
			{"looptype", iTween.LoopType.pingPong}
		});
	}

	private void MapIconAnimation(float step)
	{
		MapIcon.rectTransform.anchoredPosition = new Vector2(MapIcon.rectTransform.anchoredPosition.x, -.15f - step*.15f);
	}
	
	public void SetMapData(Constants.Maps map)
	{
		MapName.text = Constants.MapName[map];
		MapIcon.sprite = MapIcons[(int) map];
	}

	public void SetBestScoreText(int score)
	{
		BestScore.text = score.ToString();
		
		BestScoreText.gameObject.SetActive(score > 0);
		BestScore.gameObject.SetActive(score > 0);
	}
}
