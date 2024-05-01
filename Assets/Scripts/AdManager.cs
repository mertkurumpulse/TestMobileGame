using UnityEngine;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;


public delegate void AdCallBack(bool isCompleted);

public class AdManager : MonoBehaviour
{

	/*private InterstitialAdManager _interstitialAdManager;
	private NonSkippableAdManager _nonSkippableAdManager;

#if UNITY_ANDROID
	private const string AppKey = "a7a5c0ff2de8ce25cefe18d886edac3d4ddd87ff1449330c";
#else
	private const string AppKey = "9c34229ce7cd0da02fd4c499f7d0fd2b90b5c3c1c0dec2ab";
#endif

	
	public static bool RemoveAdPurchased
	{
		get
		{
			return PlayerPrefs.GetInt("RemoveAdsPurchased", 0) == 1;
		}

		set
		{
			PlayerPrefs.SetInt("RemoveAdsPurchased", value ? 1 : 0);
			TapToPlayCanvasManager.Instance.NoAdsButton.gameObject.SetActive(false);
			TapToPlayCanvasManager.Instance.RestoreButton.gameObject.SetActive(false);
		}
	}*/

	public static AdManager Instance
	{
		get { return GameManager.Instance.AdManager; }
	}

	/*
	private void Awake()
	{
		_interstitialAdManager = new InterstitialAdManager();
		_nonSkippableAdManager = new NonSkippableAdManager();

		//if (RemoveAdPurchased)
		//{
		//	Appodeal.initialize(AppKey, Appodeal.NON_SKIPPABLE_VIDEO);
		//}
		//else
		//{
		Appodeal.initialize(AppKey, Appodeal.NON_SKIPPABLE_VIDEO | Appodeal.INTERSTITIAL);
		//}
	}

	public static bool CanShowNonSkippableVideo()
	{
		return Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO);
	}

	public void ShowNonSkippableVideo(AdCallBack callback)
	{
		if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
		{
			_nonSkippableAdManager.AdCallBack = callback;
			Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
		}
		else
		{
			callback(false);
		}
	}

	public void ShowInterstitial(AdCallBack callback)
	{
		if (RemoveAdPurchased)
		{
			callback(true);
			return;
		}

		if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
		{
			_interstitialAdManager.AdCallBack = callback;
			Appodeal.show(Appodeal.INTERSTITIAL);
		}
		else
		{
			callback(false);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus) Appodeal.onResume();
	}
*/
}

/*
public class InterstitialAdManager : IInterstitialAdListener
{
	public AdCallBack AdCallBack;

	public InterstitialAdManager()
	{
		Appodeal.setInterstitialCallbacks(this);
	}

	public void onInterstitialLoaded(bool isPrecache)
	{
	}

	public void onInterstitialFailedToLoad()
	{
	}

	public void onInterstitialShown()
	{
	}

	public void onInterstitialClosed()
	{
		if (AdCallBack != null)
		{
			AdCallBack(true);
		}
	}

	public void onInterstitialClicked()
	{
	}
}

public class NonSkippableAdManager : INonSkippableVideoAdListener
{
	public AdCallBack AdCallBack;

	public NonSkippableAdManager()
	{
		Appodeal.setNonSkippableVideoCallbacks(this);
	}

	public void onNonSkippableVideoLoaded()
	{
	}

	public void onNonSkippableVideoFailedToLoad()
	{
	}

	public void onNonSkippableVideoShown()
	{
	}

	public void onNonSkippableVideoFinished()
	{
	}

	public void onNonSkippableVideoClosed(bool finished)
	{
		if (AdCallBack != null)
		{
			AdCallBack(finished);
		}
	}
}

*/
