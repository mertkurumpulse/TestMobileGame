using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

	private int score;
	public Text text;
	public int Score
	{
		
		get { return score; }
		set
		{
			score = value;
			text.text = score.ToString();
			
			transform.localScale = Vector3.one*1.4f;
			
			iTween.ScaleTo(gameObject, new Hashtable
			{
				{"x", 1},
				{"y", 1},
				{"time", .35f},
				{"easetype",iTween.EaseType.easeOutSine}
			});
			
		}
	}
	
	private void Start ()
	{
		Score = 0;
	}

}