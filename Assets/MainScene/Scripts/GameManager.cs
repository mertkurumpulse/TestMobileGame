using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{

	public static GameManager Instance;

	public LeaderboardManager LeaderboardManager;
	public AdManager AdManager;
	
	private List<int> _bestScores;

	public Constants.Maps PlayingMap { get; private set; }
	
	public int GetBestScore(Constants.Maps map)
	{
		return _bestScores[(int) map];
	}

	public Vector2 ScreenSize
	{
		get { return new Vector2(Camera.main.orthographicSize * Camera.main.aspect * 2, Camera.main.orthographicSize * 2); }
	}
	
	public void SetBestScore(Constants.Maps map, int score)
	{
		_bestScores[(int) map] = score;
		PlayerPrefs.SetInt("BestScore" + map, score);
		LeaderboardManager.ReportBestScore(map);
	}
	
	private void InitBestScores()
	{
		_bestScores = new List<int>();
		var mapEnumNames = Enum.GetNames(typeof(Constants.Maps));
		
		for (var i = 0; i < mapEnumNames.Length; i++)
		{
			_bestScores.Add(PlayerPrefs.GetInt("BestScore" + mapEnumNames[i], 0));
		}
	}

    private bool _soundEnabled;
    public bool SoundEnabled
    {
        get { return _soundEnabled; }
        set
        {
            _soundEnabled = value;
            PlayerPrefs.SetInt("SoundEnabled", _soundEnabled ? 1 : 0);
            SoundManager.Instance.IsDisabled = !_soundEnabled;
        }
    }

	private int _goldCount = 0;
	public int GoldCount
	{
		get { return _goldCount; }
		set 
		{
			_goldCount = value;
			PlayerPrefs.SetInt("GoldCount", _goldCount);
			if (MainSceneManager.Instance)
			{
				MainSceneManager.Instance.GoldText.text = _goldCount.ToString();
			}
		}
	}

    private void InitSettings()
	{
		SoundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
		
		PlayerPrefs.SetFloat("LastAdShowTime", 0);
		
		GoldCount = PlayerPrefs.GetInt("GoldCount", 0);
	}

	private void Awake () 
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
		
		PlayerPrefs.DeleteAll();
		
		Application.targetFrameRate = 60;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		InitBestScores();
		InitSettings();

		LeaderboardManager = GetComponent<LeaderboardManager>();
	}

	public void OpenGameScene(Constants.Maps map)
	{
		PlayingMap = map;
		
		SceneManager.LoadScene(1);
	}
	
}
