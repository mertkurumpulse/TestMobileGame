using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedMapScrollItem : MonoBehaviour
{

	public Text MapName;
	public Image MapIcon;
	public Text ProgressText;

	public CameraShake ShakeScript;

	public Image ProgressBar;

	public List<Sprite> MapIcons;

	private void OnValidate()
	{
		MapIcon.sprite = MapIcons[0];
	}

	public void SetMapData(Constants.Maps map)
	{
		MapName.text = Constants.MapName[map];
		MapIcon.sprite = MapIcons[(int) map];
	}

	public void SetProgress(string progressText, float fillRate)
	{
		ProgressText.text = progressText;
		ProgressBar.fillAmount = Mathf.Clamp(fillRate, .03f, 1);
	}

	public void Shake()
	{
		ShakeScript.shakeDuration = .3f;
	}
}
