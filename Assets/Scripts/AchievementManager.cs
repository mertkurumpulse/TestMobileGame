using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Achievement
{
	public Constants.AchievementType Type;
	public int DestinationCount;
	public int CurrentCount;
	public Constants.Maps RewardId;
	public bool IsUnlocked;
	
	public Achievement(Constants.AchievementType type,int destinationCount,Constants.Maps rewardId)
	{
		this.Type = type;
		this.DestinationCount = destinationCount;
		RewardId = rewardId;
		CurrentCount = PlayerPrefs.GetInt(Constants.AchievementKeys[rewardId], 0);
		IsUnlocked = PlayerPrefs.GetInt(Constants.UnlockKeys[rewardId], 0) == 1;
		
	}

	public bool IsAchievementCompleted()
	{
		return CurrentCount >= DestinationCount;
	}
	
	public void Unlock()
	{
		IsUnlocked = true;
		PlayerPrefs.SetInt(Constants.UnlockKeys[RewardId], 1);
	}
}

public class AchievementManager : MonoSingleton<AchievementManager> {

	
    public Achievement IncreaseCurrentValue(Constants.AchievementType type, int increaseValue = 1)
    {
        var lastUnlocked = GetLastUnlockedMap();

        foreach (var achievement in Constants.AchievementList)
        {
            if (achievement.RewardId > lastUnlocked+1 || achievement.Type != type) continue;

            if (achievement.IsUnlocked) return null;

	        achievement.CurrentCount += increaseValue;
            PlayerPrefs.SetInt(Constants.AchievementKeys[achievement.RewardId], achievement.CurrentCount);

	        return achievement;
        }

	    return null;

    }

    public Achievement GetCurrentAchievement(Constants.Maps rewardId)
    {
        return Constants.AchievementList.FirstOrDefault(achievement => achievement.RewardId == rewardId);
    }

    public Constants.Maps GetLastUnlockedMap()
    {
        var lastUnlocked = Constants.AchievementList.LastOrDefault(achievement => achievement.IsUnlocked);
        
        return lastUnlocked == null ? Constants.Maps.CircuitDrift : lastUnlocked.RewardId;
    }


}
