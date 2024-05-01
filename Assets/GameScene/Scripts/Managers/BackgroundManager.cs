using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

	public List<Sprite> BackgroundSprites;

	public GameObject Background1, Background2;
	
	

	public void ChangeMode(Modes from, Modes to)
	{	
		Background1.GetComponent<SpriteRenderer>().sprite = BackgroundSprites[(int) from];
		Background2.GetComponent<SpriteRenderer>().sprite = BackgroundSprites[(int) to];

		Background1.GetComponent<SpriteRenderer>().color = Color.white;
		Background2.GetComponent<SpriteRenderer>().color = Color.white;
		
		FadeOutBackground1(.2f, (int)from > (int)to);
	}

	private void FadeOutBackground1(float time, bool easeIn)
	{
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", 1},
			{"to", 0},
			{"time", time},
			{"onupdatetarget", gameObject},
			{"onupdate", "Background1FadeCallback"},
			{"easetype", easeIn ? iTween.EaseType.easeInSine : iTween.EaseType.easeOutSine}
		});
	}

	private void Background1FadeCallback(float value)
	{
		var color = Background1.GetComponent<SpriteRenderer>().color;
		color.a = value;
		Background1.GetComponent<SpriteRenderer>().color = color;
	}
	
	
}
