using System.Collections.Generic;

public static class Constants
{
    public enum Maps
    {
        CircuitDrift=0,
        InfiniteDrift=1,
    }
    
    public enum AchievementType
    {
        ScoreCollect,
        PerfectCount,
    }
    
    public static readonly Dictionary<Maps, string> MapName = new Dictionary<Maps, string>
    {
        {Maps.CircuitDrift, "Circuit Drift"},
        {Maps.InfiniteDrift, "Infinite Drift"}
    };
    
    public static readonly Dictionary<Maps, string> AchievementKeys = new Dictionary<Maps, string>()
    {
        {Maps.InfiniteDrift, "ach_map2"}
    };
    
    public static readonly Dictionary<Maps, string> UnlockKeys = new Dictionary<Maps, string>()
    {
        {Maps.InfiniteDrift, "lock_map2"}
    };
    
    public static readonly Dictionary<AchievementType, string> AchievementTexts = new Dictionary<AchievementType, string>()
    {
        {AchievementType.PerfectCount, "Make {0} More Perfect Turns"}
    };

    public static readonly Dictionary<AchievementType, string> AchievementNames = new Dictionary<AchievementType, string>
    {
        {AchievementType.PerfectCount, "Make {0} Perfect Turns"}
    };
    
    public static readonly List<Achievement> AchievementList = new List<Achievement>()
    {
        new Achievement(AchievementType.PerfectCount,20,Maps.InfiniteDrift)
    };
	
}