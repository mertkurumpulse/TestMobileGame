using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;

public enum Modes
{
	Normal,
	Perfect,
	Awesome,
	OutOfGame
}

public enum TurnQuality
{
	Bad,
	Good,
	Perfect
}

public enum GamePhase
{
	TapToPlay,
	Play,
	GameOver
}


public class GamePlayManager : MonoBehaviour
{
	public static GamePlayManager Instance;
	
	public Modes CurrentMode;

	private bool _perfectModeSeen;
	
	public CarManager Car;
	public Text Score;
	public Text TurnQualityText;
	public RaceTrackManager Track;

	public List<GameObject> Tracks;
	
	public PerfectBarManager BarManager;

	private Collectibles _collectibles;

	private TurnQuality _turnQuality;
	public int PerfectTurnTolerence { get; private set; }

	public GamePhase GamePhase;
	
	public List<ListWrapper> CarSprites;

	public BackgroundManager BackgroundManager;
	
	private int _score;

	public List<Sprite> Pims;
	public List<Sprite> PimsSelected;

	private List<PinManager> _pims;
	public static PinManager SelectedPim;

	private float _timeElapsedAfterGameStart;
	private int _lapCount;
	
	private static float _additionalVelocity = 0;
	private static float _maxVelocity;

	private PoolManager _pointsPoolManager;

	public GameObject Point;
	public GameObject CoinCollectedGameObject;

	public AchievementAnimation AchievementAnimation;
	
	public static float MaxVelocity
	{
		get
		{
			var maxVelocity = Mathf.Clamp(_maxVelocity + _additionalVelocity, _maxVelocity, _maxVelocity * 1.5f);

			return maxVelocity;
		}
		private set { _maxVelocity = value; }
	}

	public static float CentrifugalForce { get; private set; }

	private static Vector2 _turningEffect;

	public HoldToTurnAnimation HoldToTurnAnimation;

	public GameObject WorldSpaceCanvas;
	
	public GameOverManager GameOverManager;

	public Button ReturnToMenuButton;
	
	public static Vector2 TurningEffect(bool rotateClockwise)
	{
		return rotateClockwise ? _turningEffect : new Vector2(-_turningEffect.x, _turningEffect.y);
	}

	private void OnValidate()
	{
		Car = GameObject.FindGameObjectWithTag(Tags.Car).GetComponent<CarManager>();
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		Instance = this;

		_perfectModeSeen = PlayerPrefs.GetInt("PerfectModeSeen", 0) == 1;
		
		Track = Instantiate(Tracks[(int) GameManager.Instance.PlayingMap]).GetComponent<RaceTrackManager>(); 
		
		_pointsPoolManager = new PoolManager();
		_pointsPoolManager.SetUnits(new List<GameObject>{Point,CoinCollectedGameObject});
		
		GamePhase = GamePhase.TapToPlay;

		_collectibles = GameObject.FindGameObjectWithTag(Tags.Collectibles).GetComponent<Collectibles>();
		_pims = Track.Pins;
		
		GameOverManager.Close();
		HoldToTurnAnimation.Show(true);
		ReturnToMenuButton.gameObject.SetActive(true);
		
		SetMode(Modes.OutOfGame);
		
		CameraEffects.Instance.SetPostProcessFactor(1);
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void ReturnToMainMenu()
	{
		SceneManager.LoadScene(0);
	}
	
	private void StartGame ()
	{	
		GamePhase = GamePhase.Play;

		CameraEffects.Instance.SetPostProcessFactor(1);
		
		HoldToTurnAnimation.Show(false);
		ReturnToMenuButton.gameObject.SetActive(false);
		
		GameOverManager.Close();
		
		Score.gameObject.SetActive(true);
		_score = 0;
		Score.text = _score.ToString();
		
		SwitchPim();

		_timeElapsedAfterGameStart = 0;
		_lapCount = 0;

		SetMode(Modes.Normal);
	}

	private void SetMode(Modes mode)
	{
		
		switch (mode)
		{
			case Modes.Perfect:
			{
				iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Success);
				
				BarManager.ResetBars();
				CameraEffects.Instance.PerfectModeEffect(.4f);
				CameraEffects.Instance.ZoomIn();
				CameraEffects.Instance.SetPostProcessFactor(.95f);
				CameraEffects.Instance.EnablePostProcessWaveEffect(true);
				Score.GetComponent<Outline>().effectColor = new Color(.1f, .1f, .1f, .4f);

				MaxVelocity = 30;
				CentrifugalForce = .5f;
				_turningEffect = new Vector2(.8f, .2f).normalized;
				PerfectTurnTolerence = 7;

				if (!_perfectModeSeen)
				{
					_perfectModeSeen = true;
					PlayerPrefs.SetInt("PerfectModeSeen", 1);
				}
				
				break;
			}
			case Modes.Awesome:
				iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Success);
				
				BarManager.HideBars();
				CameraEffects.Instance.PerfectModeEffect(.5f);
				CameraEffects.Instance.ZoomIn();
				CameraEffects.Instance.SetPostProcessFactor(.92f);
				CameraEffects.Instance.EnablePostProcessWaveEffect(true);
				Score.GetComponent<Outline>().effectColor = new Color32(255, 255, 255, 50);

				MaxVelocity = 35;
				CentrifugalForce = .5f;
				_turningEffect = new Vector2(.8f, .2f).normalized;
				PerfectTurnTolerence = 7;
				break;
			case Modes.Normal:
				
				BarManager.ResetBars();
				CameraEffects.Instance.NormalModeEffect(.2f);
				CameraEffects.Instance.ZoomOut();
				CameraEffects.Instance.SetPostProcessFactor(1);
				CameraEffects.Instance.EnablePostProcessWaveEffect(false);
				Score.GetComponent<Outline>().effectColor = new Color32(140, 140, 110, 100);

				MaxVelocity = 20;
				CentrifugalForce = .2f;
				_turningEffect = new Vector2(.9f, .1f).normalized;
				PerfectTurnTolerence = 8;
				break;
			case Modes.OutOfGame:
				//CameraEffects.Instance.ZoomIn();
				Score.gameObject.SetActive(false);
				CameraEffects.Instance.EnablePostProcessWaveEffect(false);
				SelectedPim = null;
				MaxVelocity = 0;
				
				mode = Modes.Normal;
				break;
		}
		
		iTween.Stop(Score.gameObject);
		Score.rectTransform.localScale = Vector3.one;
		Score.color = Track.ScoreColors[(int) mode];
		
		Track.ChangeMode(mode);
		BackgroundManager.ChangeMode(CurrentMode, mode);
		Car.ChangeMode(mode);
		
		CurrentMode = mode;
		UpdatePims();
	}

	private void Update()
	{
		switch (GamePhase)
		{
			case GamePhase.TapToPlay:
				TapToPlayPhaseUpdate();
				break;
			case GamePhase.Play:
				PlayPhaseUpdate();
				break;
			case GamePhase.GameOver:
				GameOverPhaseUpdate();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void TapToPlayPhaseUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartGame();
		}
		
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				StartGame();
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ReturnToMainMenu();
		}
	}

	private bool _gameOverTapFlag = false;
	
	private void GameOverPhaseUpdate()
	{
		if (!GameOverManager.IsActive) return;


		if (Input.GetKeyDown(KeyCode.Space))
		{
			_gameOverTapFlag = true;
		}
		else if (Input.GetKeyUp(KeyCode.Space) && _gameOverTapFlag)
		{
			_gameOverTapFlag = false;
			StartGame();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ReturnToMainMenu();
		}
		
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				_gameOverTapFlag = true;
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Ended && _gameOverTapFlag)
			{
				_gameOverTapFlag = false;
				StartGame();
			}
		}
	}

	private void PlayPhaseUpdate()
	{
		_timeElapsedAfterGameStart += Time.deltaTime;
		_additionalVelocity = _timeElapsedAfterGameStart * .2f;
		
		if (Input.GetKeyDown(KeyCode.Space) && SelectedPim && !Car.AttachedToPin)
		{
			SelectedPim.SetActive(true);
		}
		else if (Input.GetKeyUp(KeyCode.Space) && SelectedPim)
		{
			SelectedPim.SetActive(false);
		}
		
		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began && SelectedPim && !Car.AttachedToPin)
			{
				SelectedPim.SetActive(true);
			}
			else if (SelectedPim &&
			         (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended))
			{
				SelectedPim.SetActive(false);
			}
		}
	}

	public void GameOver()
	{
		GamePhase = GamePhase.GameOver;
		
		SetMode(Modes.OutOfGame);
		
		_additionalVelocity = 0;

		TurnQualityText.color = new Color(0, 0, 0, 0);
		
		BarManager.HideBars();
		CameraEffects.Instance.SetBloomEffectFactor(0);
		iTween.Stop(CameraEffects.Instance.gameObject);
		CameraEffects.Instance.ZoomIn();
		Score.gameObject.SetActive(false);
		
		_collectibles.RemoveAllCoins();
		
		iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.Warning);

		GameOverManager.SetScore(_score);
		GameOverManager.Open();
		
		ReturnToMenuButton.gameObject.SetActive(true);
	}

	private void SwitchPim()
	{
		if (SelectedPim == null)
		{
			SelectedPim = _pims[0];
		}
		else
		{
			SelectedPim = _pims[(_pims.IndexOf(SelectedPim) + 1) % _pims.Count];
		}
		
		UpdatePims();
		
	}

	private void UpdatePims()
	{
		foreach (var pim in _pims)
		{
			if (pim == SelectedPim)
			{
				pim.Rotator.GetComponent<SpriteRenderer>().sprite = PimsSelected[(int)CurrentMode];
			}
			else
			{
				pim.Rotator.GetComponent<SpriteRenderer>().sprite = Pims[(int)CurrentMode];
			}
		}
	}

	public void LapCompleted(float angle)
	{
		if (angle < PerfectTurnTolerence)
		{
			_turnQuality = TurnQuality.Perfect;
		}
		else if (angle < PerfectTurnTolerence*1.5f)
		{
			_turnQuality = TurnQuality.Good;
		}
		else
		{
			_turnQuality = TurnQuality.Bad;
		}
		
		var gainedScore = 0;
		
		switch (_turnQuality)
		{
			case TurnQuality.Bad:
				break;
			case TurnQuality.Good:
				Car.GetComponent<Rigidbody2D>().AddForce(Car.transform.up*.5f, ForceMode2D.Impulse);
				break;
			case TurnQuality.Perfect:
				Car.GetComponent<Rigidbody2D>().AddForce(Car.transform.up*1f, ForceMode2D.Impulse);
				
				UpdateAchievementData(Constants.AchievementType.PerfectCount);
		
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		float fillRatio = CurrentMode == Modes.Awesome ? 0 : BarManager.FillBar(_turnQuality);
		
		if (CurrentMode != Modes.Normal && _turnQuality != TurnQuality.Perfect)
		{
			BarManager.FillBar(_turnQuality);
			SetMode(Modes.Normal);
		}
		else if (CurrentMode == Modes.Normal && fillRatio == 1)
		{
			SetMode(Modes.Perfect);
		}
		else if (CurrentMode == Modes.Perfect)
		{
			if (fillRatio == 1)
			{
				SetMode(Modes.Awesome);
			}
			else
			{
				CameraEffects.Instance.ZoomOut(fillRatio);
			}
		}

		if (_turnQuality == TurnQuality.Perfect)
		{
			if (CurrentMode == Modes.Awesome)
			{
				TurnQualityText.TurnQualityEffect("Awesome!", new Color(0, .95f, 1f));
			}
			else
			{
				TurnQualityText.TurnQualityEffect("Perfect!", Color.green);
			}
		}

		var additionalRandom = 0;
		switch (CurrentMode)
		{
			case Modes.Normal:
				gainedScore = 1;
				break;
			case Modes.Perfect:
				gainedScore = 2;
				additionalRandom = 1;
				break;
			case Modes.Awesome:
				gainedScore = 4;
				additionalRandom = 2;
				break;
		}
		if (gainedScore>0)
		{
			_score += gainedScore;
			
			
			Score.ScoreEffect(_score, gainedScore > 3 ? .3f : gainedScore*.1f);
		}
		
		_lapCount++;
		if (_lapCount > 2 && _collectibles.CanCreateCoins() && UnityEngine.Random.Range(0,4-additionalRandom) == 0)
		{
			_collectibles.CreateCoin();
		}

		SpawnGainedPoint(gainedScore,Car.transform.position);
		
		SwitchPim();
		
		UpdateAchievementData(Constants.AchievementType.ScoreCollect,gainedScore);
		
	}

	public void CollectCoin(GameObject coin)
	{
		GameManager.Instance.GoldCount++;
		
		_collectibles.CollectCoin(coin);
		
		SpawnGainedPoint(1,coin.transform.position);
		SpawnCoinCollectedAnimation(coin.transform.position);
	}

	private void UpdateAchievementData(Constants.AchievementType type,int increaseCount = 1)
	{
		if (GameManager.Instance.PlayingMap == Constants.Maps.InfiniteDrift) return;
		
		var currentDetail = AchievementManager.Instance.IncreaseCurrentValue(type,increaseCount);
		
		if (currentDetail != null && currentDetail.IsAchievementCompleted() && !currentDetail.IsUnlocked)
		{
			currentDetail.Unlock();
			AchievementAnimation.Setup(currentDetail.RewardId);
		}
	}
	
	private void SpawnGainedPoint(int gainedPoint,Vector3 pos)
	{
		var point = _pointsPoolManager.GetObjectFromPool(0);
		point.transform.position = pos;
		point.transform.SetParent(Point.transform.parent);
		
		var pointScript = point.GetComponent<PointGainedScript>();
		pointScript.AnimatePoint(gainedPoint);
		pointScript.onCompletedAction += PointScriptOnOnCompletedAction;
		
	}

	private void PointScriptOnOnCompletedAction(GameObject gameObject)
	{
		gameObject.GetComponent<PointGainedScript>().onCompletedAction -= PointScriptOnOnCompletedAction;
		_pointsPoolManager.AddObjectToPool(gameObject);
	}
	
	private void SpawnCoinCollectedAnimation(Vector3 pos)
	{
		var coinCollected = _pointsPoolManager.GetObjectFromPool(1);
		coinCollected.transform.position = pos;
		coinCollected.transform.SetParent(CoinCollectedGameObject.transform.parent);
		
		var animationScript = coinCollected.GetComponent<CollectedCoinAnimationScript>();
		animationScript.AnimatePoint();
		animationScript.onCompletedAction += CoinCollectedOnOnCompletedAction;
		
	}

	private void CoinCollectedOnOnCompletedAction(GameObject gameObject)
	{
		gameObject.GetComponent<CollectedCoinAnimationScript>().onCompletedAction -= CoinCollectedOnOnCompletedAction;
		_pointsPoolManager.AddObjectToPool(gameObject);
	}
	
	
	public void CheckShowRateUsPopup()
	{
		if (GameManager.Instance.GetBestScore(Constants.Maps.CircuitDrift) < 25 || Time.time < 40) return;
		
		var currentDate = DateTime.Now;
		
		var oldDate = PlayerPrefs.GetString("lastRatePopupShowTime", "") == "" ? currentDate.AddHours(-3) : DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("lastRatePopupShowTime")));
		
		var difference = currentDate.Subtract(oldDate);
		
		var minTimeSpan = new TimeSpan(2, 0, 0);

#if UNITY_IOS
		if (difference > minTimeSpan)
		{
			Device.RequestStoreReview();
		}
#endif
		
		
		PlayerPrefs.SetString("lastRatePopupShowTime", currentDate.ToBinary().ToString());
	}
}

public struct Tags
{
	public const string Track = "Track";
	public const string Pin = "Pin";
	public const string Car = "Car";
	public const string ExplosionObject = "ExplosionObject";
	public const string Balancer = "Balancer";
	public const string ReleaseBalancer = "ReleaseBalancer";
	public const string StartFinish = "StartFinish";
	public const string Score = "Score";
	public const string GameManager = "GameController";
	public const string CarGlow = "CarGlow";
	public const string TurnText = "TurnText";
	public const string Collectibles = "Collectibles";
	public const string Coin = "Coin";
	public const string HoldToTurn = "HoldToTurn";
}

[Serializable]
public class ListWrapper
{
	public List<Sprite> Sprites;
}

public static class ExtensionMethods
{
	public static Vector3 ProjectionOnto(this Vector3 vector2, Vector3 componentVector)
	{
		return componentVector * Vector2.Dot(vector2, componentVector) / componentVector.sqrMagnitude;
	}
	
	public static Vector2 ProjectionOnto(this Vector2 vector2, Vector2 componentVector)
	{
		return componentVector * Vector2.Dot(vector2, componentVector) / componentVector.sqrMagnitude;
	}
}