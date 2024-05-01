using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementAnimation : MonoBehaviour
{	
	
	public TextMesh AchievementName;
	public SpriteRenderer MapIcon;
	public SpriteRenderer LockIcon;
	public Sprite UnlockSprite;
	
	public List<Sprite> MapIcons;

	private Vector3 _initialPosition, _destinationPosition;

	public void Setup(Constants.Maps map)
	{
		AchievementName.GetComponent<Renderer>().sortingLayerID = MapIcon.sortingLayerID;
		AchievementName.text = string.Format(Constants.AchievementNames[AchievementManager.Instance.GetCurrentAchievement(map).Type], AchievementManager.Instance.GetCurrentAchievement(map).DestinationCount);
		MapIcon.sprite = MapIcons[(int) map];

		print(GameManager.Instance.ScreenSize.x);
		
		_initialPosition =
			new Vector2(-GameManager.Instance.ScreenSize.x * .5f - (AchievementName.transform.localPosition.x + AchievementName.GetComponent<Renderer>().bounds.size.x),
				GameManager.Instance.ScreenSize.y * .5f - 1.5f);
		_destinationPosition = new Vector2(-GameManager.Instance.ScreenSize.x*.5f + 1, _initialPosition.y);

		transform.position = _initialPosition;
		
		iTween.MoveTo(gameObject, new Hashtable
		{
			{"position", _destinationPosition},
			{"time", .7f},
			{"easetype", iTween.EaseType.easeOutBack},
			{"oncompletetarget", gameObject},
			{"oncomplete", "ShakeLock"}
		});
		
		iTween.ValueTo(gameObject, new Hashtable
		{
			{"delay", 1.6f},
			{"time", .4f},
			{"from", 1},
			{"to", 0f},
			{"onstarttarget", gameObject},
			{"onstart", "Unlock"},
			{"onupdatetarget", gameObject},
			{"onupdate", "FadeOutLock"},
			{"oncompletetarget", gameObject},
			{"oncomplete", "UnlockAnimationCompleted"}
		});
	}

	private IEnumerator ShakeLock()
	{	
		yield return new WaitForSeconds(.4f);
		LockIcon.GetComponent<CameraShake>().shakeDuration = .5f;
	}

	private void Unlock()
	{
		LockIcon.sprite = UnlockSprite;
	}
	
	private void FadeOutLock(float value)
	{
		LockIcon.color = new Color(LockIcon.color.r, LockIcon.color.g, LockIcon.color.b, value);
		LockIcon.transform.localScale = Vector3.one*.7f*(1.2f - value * .2f);
	}

	private void UnlockAnimationCompleted()
	{
		iTween.MoveTo(gameObject, new Hashtable
		{
			{"position", _initialPosition},
			{"delay", .4f},
			{"time", .7f},
			{"easetype", iTween.EaseType.easeInBack}
		});
		
	}
	
}
