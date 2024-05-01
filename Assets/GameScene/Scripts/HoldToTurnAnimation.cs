using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldToTurnAnimation : MonoBehaviour
{

	public Text HoldToTurnText;
	
	public GameObject Car;
	public GameObject Pim;
	public GameObject Hand;
	public GameObject Wave;

	private LineRenderer _pimLineRenderer;

	private Vector2 _carInitialPosition = new Vector2(2.5f, -.75f);

	private void Start()
	{
		_pimLineRenderer = Pim.GetComponent<LineRenderer>();
	}

	public void Show(bool show)
	{
		
		
		iTween.Stop(HoldToTurnText.gameObject);
		iTween.Stop(Car.gameObject);
		iTween.Stop(Hand.gameObject);
		iTween.Stop(Wave.gameObject);
		iTween.Stop(Pim.gameObject);
		
		Car.transform.SetParent(transform);
		
		HoldToTurnText.gameObject.SetActive(false);
		gameObject.SetActive(false);
		
		if (!show) return;
		
		HoldToTurnText.gameObject.SetActive(true);
		
		if (GamePlayManager.Instance.Track.MapType == Constants.Maps.CircuitDrift)
		{
			CarAnimationFirst();
			HoldToTurnText.text = "Hold To Turn";
			gameObject.SetActive(false);
		}
		else
		{
			HoldToTurnText.text = "Tap To Start";
		}
		
		iTween.FadeFrom(HoldToTurnText.gameObject, new Hashtable
		{
			{"alpha", 1},
			{"amount", 0},
			{"time", .2},
			{"easeType", iTween.EaseType.easeOutSine},
			{"oncompletetarget", gameObject},
			{"oncomplete", "TapToRestartFade"}
		});
		
		iTween.FadeFrom(HoldToTurnText.gameObject, new Hashtable
		{
			{"alpha", 1f},
			{"amount", .65f},
			{"time", .45f},
			{"easetype", iTween.EaseType.easeOutSine},
			{"loopType", iTween.LoopType.pingPong}
		});
		
		
	}
	
	private void CarAnimationFirst()
	{
		Car.transform.position = _carInitialPosition;
		
		Car.transform.rotation = Quaternion.Euler(new Vector3(0,0,180));
		Pim.transform.rotation = Quaternion.identity;
		Pim.GetComponent<LineRenderer>().positionCount = 0;
		Car.GetComponent<SpriteRenderer>().enabled = false;
		
		Wave.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
		
		iTween.MoveTo(Car.gameObject, new Hashtable
		{
			{"delay", 1},
			{"time", .5f},
			{"position", new Vector3(Car.transform.position.x, Pim.transform.position.y, 0)},
			{"easetype", iTween.EaseType.easeInSine},
			{"onstart", "AnimationStarted"},
			{"onstarttarget", gameObject},
			{"oncomplete", "AttachCarAndStartRotation"},
			{"oncompletetarget", gameObject}
		});
		
		iTween.ScaleTo(Hand.gameObject, new Hashtable
		{
			{"delay", 1.3},
			{"time", .2f},
			{"scale", Vector3.one*.4f},
			{"easetype", iTween.EaseType.easeInSine}
		});
	}

	private void AnimationStarted()
	{
		Car.GetComponent<SpriteRenderer>().enabled = true;
	}

	private void AttachCarAndStartRotation()
	{
		Pim.transform.rotation = Quaternion.identity;
		Car.transform.SetParent(Pim.transform);

		_pimLineRenderer.positionCount = 2;
		iTween.RotateTo(Pim.gameObject, new Hashtable
		{
			{"rotation", new Vector3(0,0,-180f)},
			{"time", 1.3f},
			{"oncomplete", "CarAnimationLast"},
			{"oncompletetarget", gameObject},
			{"easetype", iTween.EaseType.linear},
			{"onupdate", "SetLineRendererPoints"},
			{"onupdatetarget", gameObject}
		});
		
		iTween.ScaleTo(Hand.gameObject, new Hashtable
		{
			{"delay", 1f},
			{"time", .3f},
			{"scale", Vector3.one*.5f},
			{"easetype", iTween.EaseType.easeOutSine}
		});
		
		Wave.GetComponent<SpriteRenderer>().color = Color.white;
		Wave.transform.localScale = Vector3.one*.8f;
		
		iTween.ScaleTo(Wave.gameObject, new Hashtable
		{
			{"time", .3f},
			{"scale", Vector3.one*1.5f},
			{"easetype", iTween.EaseType.easeOutSine}
		});
		
		iTween.ValueTo(Wave.gameObject, new Hashtable
		{
			{"from", 1},
			{"to", 0},
			{"time", .3f},
			{"onupdate", "FadeWave"},
			{"onupdatetarget", gameObject},
			{"easetype", iTween.EaseType.easeOutSine}
		});
	}

	private void FadeWave(float alpha)
	{
		Wave.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
	}

	private void SetLineRendererPoints()
	{
		var positions = new Vector3[2];
		var from = Pim.transform.position;
		var to = Car.transform.position;
		
		positions[0] = Vector2.Lerp(from, to, .18f / Vector2.Distance(from, to));
		positions[1] = Vector2.Lerp(to, from, .2f / Vector2.Distance(from, to));
		
		
		_pimLineRenderer.SetPositions(positions);
	}

	private void CarAnimationLast()
	{
		_pimLineRenderer.positionCount = 0;
		
		Car.transform.SetParent(transform);
		iTween.MoveTo(Car.gameObject, new Hashtable
		{
			{"time", .5f},
			{"position", new Vector3(Car.transform.position.x, _carInitialPosition.y, 0)},
			{"easetype", iTween.EaseType.linear},
			{"oncomplete", "CarAnimationFirst"},
			{"oncompletetarget", gameObject}
		});
	}
}
