using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class EffectExtensions
{
	public static void ScoreEffect(this Text scoreText, int score, float effectFactor)
	{	
		scoreText.text = score.ToString();
		scoreText.transform.localScale = Vector3.one;
		scoreText.color = GamePlayManager.Instance.Track.ScoreColors[(int) GamePlayManager.Instance.CurrentMode];
		
		CameraEffects.Instance.PerfectModeEffect(effectFactor);

		iTween.ColorFrom(scoreText.gameObject, new Hashtable
		{
			{"color", GamePlayManager.Instance.Track.ScoreEffectColors[(int) GamePlayManager.Instance.CurrentMode]},
			{"time", .5},
			{"easetype", iTween.EaseType.easeInSine}
		});
		
		iTween.ScaleFrom(scoreText.gameObject, new Hashtable
		{
			{"x", 1.4f},
			{"y", 1.4f},
			{"time", .5f},
			{"easetype",iTween.EaseType.easeOutSine}
		});
		
		iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactLight);
	}

	public static void TurnQualityEffect(this Text qualityText, string text, Color color)
	{
		qualityText.text = text;
		qualityText.GetComponent<Outline>().effectColor = color;
		qualityText.color = Color.white;
		//qualityText.color = color;
		qualityText.transform.localScale = Vector3.one;
		
		iTween.ScaleTo(qualityText.gameObject, new Hashtable
		{
			{"x", 1.4f},
			{"y", 1.4f},
			{"time", 1f},
			{"easetype",iTween.EaseType.easeOutSine}
		});

		var newColor = Color.white;
		newColor.a = 0;
		iTween.ColorTo(qualityText.gameObject, new Hashtable
		{
			{"color", newColor},
			{"time", 1f},
			{"easetype", iTween.EaseType.easeOutSine}
		});
	}	
}