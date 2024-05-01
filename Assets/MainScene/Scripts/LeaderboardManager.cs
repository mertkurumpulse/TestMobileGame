using UnityEngine;
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif


	public class LeaderboardManager : MonoBehaviour
	{
		private static readonly string[] LeaderboardIds = {"zero_map_leaderboard", "infinite_map_leaderboard", "butterfly_map_leaderboard"};
		
		public static LeaderboardManager Instance
		{
			get { return GameManager.Instance.LeaderboardManager; }
		}
		
		private void Awake()
		{
			Social.localUser.Authenticate(ProcessAuthentication);
		}

		private void ProcessAuthentication(bool success)
		{
			if (success)
			{
				ReportBestScore(Constants.Maps.CircuitDrift);
				ReportBestScore(Constants.Maps.InfiniteDrift);
			}
		}

		public void ShowLeaderboard(Constants.Maps map)
		{
			#if UNITY_IOS && !UNITY_EDITOR
			GameCenterPlatform.ShowLeaderboardUI(LeaderboardIds[(int)map], TimeScope.AllTime);
			#endif
		}

		public static void ReportBestScore(Constants.Maps map)
		{
			#if UNITY_IOS && !UNITY_EDITOR
			Social.ReportScore(GameManager.Instance.GetBestScore(map), LeaderboardIds[(int) map], result => {});
			#endif
		}
	}

