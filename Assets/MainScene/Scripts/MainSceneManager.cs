using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{

    public ScrollManager MapsScrollManager, CarsScrollManager;

    public GameObject ScrollItemPrefab;
    public GameObject ScrollDisabledItemPrefab;

    public GameObject Background;

    public Button PlayButton;

    public Button LeaderboardButton;

    public Text GoldText;
    
    public static MainSceneManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        for (var i = 0; i < Enum.GetNames(typeof(Constants.Maps)).Length; i++)
        {
            GameObject scrollItem;

            var currentAchievement = AchievementManager.Instance.GetCurrentAchievement((Constants.Maps) i);
            
            if (currentAchievement == null || currentAchievement.IsUnlocked)
            {
                scrollItem = Instantiate(ScrollItemPrefab);

                scrollItem.name = "ScrollItem" + i;
                var scrollItemManager = scrollItem.GetComponent<UnlockedMapScrollItem>();
                scrollItemManager.SetMapData((Constants.Maps)i);
                scrollItemManager.SetBestScoreText(GameManager.Instance.GetBestScore((Constants.Maps) i));
            }
            else
            {
                scrollItem = Instantiate(ScrollDisabledItemPrefab);

                scrollItem.name = "ScrollItem" + i;
                var scrollItemManager = scrollItem.GetComponent<LockedMapScrollItem>();
                scrollItemManager.SetMapData((Constants.Maps)i);
                scrollItemManager.SetProgress(String.Format(Constants.AchievementTexts[currentAchievement.Type],currentAchievement.DestinationCount - currentAchievement.CurrentCount) , currentAchievement.CurrentCount/(float) currentAchievement.DestinationCount);
            }
            
            MapsScrollManager.ScrollList.Add(scrollItem);
        }
        
        MapsScrollManager.Setup();
        CarsScrollManager.Setup();
        
        
        MapsScrollManager.SetSelectedItem((int)AchievementManager.Instance.GetLastUnlockedMap());
        CarsScrollManager.SetSelectedItem(PlayerPrefs.GetInt("LastUsedCar", 0));
        
        
        /*SoundButton.onClick.AddListener(delegate
        {
            GameManager.Instance.SoundEnabled = !GameManager.Instance.SoundEnabled;
        });
        
        VibrationButton.onClick.AddListener(delegate
        {
            GameManager.Instance.VibrationEnabled = !GameManager.Instance.VibrationEnabled;
        });
        
        LeaderboardButton.onClick.AddListener(delegate
        {
            LeaderboardManager.Instance.ShowLeaderboard((Constants.Maps)MapsScrollManager.SelectedObjectIndex);
        });*/

        PlayButton.onClick.AddListener(delegate
        {
            if (AchievementManager.Instance.GetCurrentAchievement((Constants.Maps) MapsScrollManager.SelectedObjectIndex) == null || AchievementManager.Instance.GetCurrentAchievement((Constants.Maps) MapsScrollManager.SelectedObjectIndex).IsUnlocked)
            {
                GameManager.Instance.OpenGameScene((Constants.Maps) MapsScrollManager.SelectedObjectIndex);
            }
            else
            {
                MapsScrollManager.ScrollList[MapsScrollManager.SelectedObjectIndex].GetComponent<LockedMapScrollItem>().Shake();
            }
            
        });

        GoldText.text = GameManager.Instance.GoldCount.ToString();
        
        Background.FitHeight(Camera.main.orthographicSize*2*100);
    }

    public void GoToNextMap()
    {
        MapsScrollManager.SetSelectedItem(MapsScrollManager.SelectedObjectIndex + 1);
    }

    public void GoToPreviousMap()
    {
        MapsScrollManager.SetSelectedItem(MapsScrollManager.SelectedObjectIndex - 1);
    }

    public void GoToNextCar()
    {
        CarsScrollManager.SetSelectedItem(CarsScrollManager.SelectedObjectIndex + 1);
    }

    public void GoToPreviousCar()
    {
        CarsScrollManager.SetSelectedItem(CarsScrollManager.SelectedObjectIndex - 1);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
}