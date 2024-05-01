using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{

	public GameObject ScorePanel;
	public Text Score;
	public Text HighScore;
	public Text TapToRestart;

	public bool IsActive;
	
	private void FadeTapToStart(float value)
	{
		var color = TapToRestart.color;
		color.a = value;
		TapToRestart.color = color;
	}

	public void SetScore(int score)
	{
		Score.text = score.ToString();

		var currentBestScore = GameManager.Instance.GetBestScore(GameManager.Instance.PlayingMap);

		//var averageScore = PlayerPrefs.GetInt("AverageScore", 0);
		var playCount = PlayerPrefs.GetInt("PlayCount", 0);

		if (score > 0)
		{
			//var newAverage = (averageScore + score) / (playCount + 1);
			
			//PlayerPrefs.SetInt("AverageScore", newAverage);
			PlayerPrefs.SetInt("PlayCount", playCount + 1);
		}
		
		var prefix = "Best: ";
		if (score > currentBestScore)
		{
			currentBestScore = score;
			GameManager.Instance.SetBestScore(GameManager.Instance.PlayingMap, currentBestScore);
			
			prefix = "New Best: ";
		}
		
		HighScore.text = prefix + currentBestScore;
	}

	public int GetHighScore()
	{
		return PlayerPrefs.GetInt("HighScore", 0);
	}

	public void Open()
	{
		gameObject.SetActive(true);
		ScorePanel.GetComponent<RectTransform>().localScale = new Vector3(0,1,1);

		TapToRestart.gameObject.SetActive(false);
		
		StartCoroutine(OpenAfterSeconds(.3f));
	}

	public void Close()
	{
		gameObject.SetActive(false);
		IsActive = false;
	}

	private IEnumerator OpenAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", 0},
			{"to", 1f},
			{"time", .2f},
			{"onupdatetarget", gameObject},
			{"oncompletetarget", gameObject},
			{"oncomplete", "OpenPanelCompleted"},
			{"onupdate", "OpenPanelAnimation"},
			{"easetype", iTween.EaseType.easeOutBack}
		});
	}

	private void OpenPanelAnimation(float step)
	{
		ScorePanel.GetComponent<RectTransform>().localScale = new Vector3(1,step,1);
	}

	private IEnumerator OpenPanelCompleted()
	{
		if (Time.time - PlayerPrefs.GetFloat("LastAdShowTime") > 20)
		{
			/*AdManager.Instance.ShowInterstitial(isCompleted =>
			{
				PlayerPrefs.SetFloat("LastAdShowTime", Time.time);
			});*/
		}
		else
		{
			GamePlayManager.Instance.CheckShowRateUsPopup();
		}

		yield return new WaitForSeconds(.3f);
		IsActive = true;

		GamePlayManager.Instance.HoldToTurnAnimation.Show(false);

		TapToRestart.gameObject.SetActive(true);
		iTween.FadeFrom(TapToRestart.gameObject, new Hashtable
		{
			{"alpha", 1},
			{"amount", 0},
			{"time", .2},
			{"easeType", iTween.EaseType.easeOutSine},
			{"oncompletetarget", gameObject},
			{"oncomplete", "TapToRestartFade"}
		});
	}

	private void TapToRestartFade()
	{
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"from", 1f},
			{"to", .65f},
			{"time", .45f},
			{"onupdatetarget", gameObject},
			{"onupdate", "FadeTapToStart"},
			{"easetype", iTween.EaseType.easeOutSine},
			{"loopType", iTween.LoopType.pingPong}
		});
	}
	
}
