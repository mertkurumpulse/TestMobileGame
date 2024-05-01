using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PerfectTurnImage : MonoBehaviour {
	
	// Update is called once per frame
	private CarManager _carManager;

	public Image PerfectTurnRatioImage;

	private static bool _show;

	private GameObject _canvas;

	private Color _perfectColor, _badColor;

	public void Show(bool show)
	{
		if (GamePlayManager.Instance.Track.MapType != Constants.Maps.CircuitDrift) show = false;
		if (Equals(show, _show)) return;

		_show = show;

		if (!_show)
		{
			if (PerfectTurnRatioImage.fillAmount >= 1 - Mathf.Epsilon)
			{
				iTween.ValueTo(gameObject, new Hashtable
				{
					{"from", 0},
					{"to", 1},
					{"time", .4f},
					{"onupdate", "PerfectTurnEffect"},
					{"onupdatetarget", gameObject},
					{"oncomplete", "PerfectTurnEffectEnd"},
					{"oncompletetarget", gameObject}
				});
			}
			else
			{
				PerfectTurnRatioImage.fillAmount = 0;
			}
		}
	}

	private void PerfectTurnEffect(float value)
	{
		PerfectTurnRatioImage.rectTransform.localScale = Vector3.one * (1 + value * .24f);
		PerfectTurnRatioImage.color = value*(new Color(.7f, 1, .7f) - _perfectColor) + _perfectColor - new Color32(0, 0, 0, (byte)(value * 255));
	}

	private void PerfectTurnEffectEnd()
	{
		PerfectTurnRatioImage.color = Color.white;
		PerfectTurnRatioImage.rectTransform.localScale = Vector3.one;
		PerfectTurnRatioImage.fillAmount = 0;
	}
	
	private void Start()
	{
		
		_canvas = transform.parent.gameObject;
		_carManager = GamePlayManager.Instance.Car;
		PerfectTurnRatioImage = GetComponent<Image>();
		
		_perfectColor = new Color32(78, 222, 0,255);
		_badColor = new Color32(222,78,0,255);
	}

	void LateUpdate ()
	{
		if (_show)
		{
			CalculateDegreeToPerfectAngle();
			SetPosition();
		}
	}
	
	private void CalculateDegreeToPerfectAngle()
	{
		var perfectAngle = _carManager.Balancer.transform.eulerAngles.z - Mathf.Rad2Deg * Mathf.Atan(GamePlayManager.CentrifugalForce*.7f);
		
		var degreeToPerfectAngle = (_carManager.transform.eulerAngles.z - perfectAngle) % 360;

		if (degreeToPerfectAngle > 180 && degreeToPerfectAngle < 270)
		{
			PerfectTurnRatioImage.fillAmount = 0;
			return;
		}
		
		PerfectTurnRatioImage.fillClockwise = !(degreeToPerfectAngle > 180 || degreeToPerfectAngle < 0);
		
		
		degreeToPerfectAngle = degreeToPerfectAngle > 180 ? 360 - degreeToPerfectAngle : Mathf.Abs(degreeToPerfectAngle);
		degreeToPerfectAngle = Mathf.Clamp(degreeToPerfectAngle - GamePlayManager.Instance.PerfectTurnTolerence, 0, degreeToPerfectAngle - GamePlayManager.Instance.PerfectTurnTolerence);
		
		
		
		PerfectTurnRatioImage.fillAmount = (180 - Mathf.CeilToInt(degreeToPerfectAngle)) / 180f;

		if (PerfectTurnRatioImage.fillAmount >= 1 - Mathf.Epsilon)
		{
			PerfectTurnRatioImage.color = _perfectColor;
		}
		else
		{
			PerfectTurnRatioImage.color = _badColor;
		}


	}
	
	public void SetPosition()
	{
		Vector2 pos = GamePlayManager.SelectedPim.transform.position;  // get the game object position
		Vector2 leftDownCornerPosCanvas = -_canvas.GetComponent<RectTransform>().sizeDelta / 2 * _canvas.GetComponent<RectTransform>().localScale.x;

		Vector2 canvasPoint = new Vector2(
			(pos.x - leftDownCornerPosCanvas.x) / (_canvas.GetComponent<RectTransform>().sizeDelta.x *
			                                       _canvas.GetComponent<RectTransform>().localScale.x),
			(pos.y - leftDownCornerPosCanvas.y) / (_canvas.GetComponent<RectTransform>().sizeDelta.y *
			                                       _canvas.GetComponent<RectTransform>().localScale.y));
 
		// set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
		PerfectTurnRatioImage.rectTransform.anchorMin = canvasPoint;  
		PerfectTurnRatioImage.rectTransform.anchorMax = canvasPoint; 
	}
}
