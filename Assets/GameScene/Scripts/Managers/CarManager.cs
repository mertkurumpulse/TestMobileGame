using System.Collections;
using UnityEngine;

public class CarManager : MonoBehaviour
{
	public GameObject LeftRearWheel, RightRearWheel;
	public GameObject WheelTrail;
	private GameObject _leftWheelTrail, _rightWheelTrail;
	public GameObject Glow;
	public GameObject Trail;

	public Sprite GlowPerfect, GlowAwesome;
	
	public int CarType;
	
	private Rigidbody2D _rigidbody2D;

	private GameObject _explosionObject;

	public bool AttachedToPin{get{ return transform.parent != null; }}

	public Vector2 ForwardVelocity { get; private set; }

	public GameObject Balancer { get; private set; }

	private Coroutine _driftCoroutine;

	private bool _linePassed;

	private void Start ()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		
		_explosionObject = GameObject.FindGameObjectWithTag(Tags.ExplosionObject);

		transform.position = GamePlayManager.Instance.Track.CarInitialPosition;
		transform.rotation = Quaternion.Euler(0, 0, GamePlayManager.Instance.Track.CarInitialZRotation);
	}

	private void Update()
	{
		if (GamePlayManager.Instance.GamePhase != GamePhase.Play) return;
		
		if (_linePassed && !GamePlayManager.SelectedPim.IsActive)
		{
			var forwardVelocity = _rigidbody2D.velocity.ProjectionOnto(transform.up);
			var otherComponent = _rigidbody2D.velocity - forwardVelocity;
			
			GamePlayManager.Instance.LapCompleted(Vector2.Angle(forwardVelocity + otherComponent*.7f, Balancer.transform.up));
			_linePassed = false;
		}
		
		HandleVelocity();
		
		if (Balancer && (!AttachedToPin || !GamePlayManager.SelectedPim.IsRotating))
		{
			BalanceCar();
		}
		
		if (AttachedToPin)
		{
			TurningDriftEffect();
		}
	}
	
	public void ChangeMode(Modes mode)
	{
		GetComponent<SpriteRenderer>().sprite = GamePlayManager.Instance.CarSprites[CarType].Sprites[(int) mode];
		if (mode == Modes.Normal)
		{
			Trail.GetComponent<TrailScript>().PauseTrail();
		}
		else
		{
			Trail.GetComponent<TrailScript>().ResumeTrail();
			
			if (mode == Modes.Perfect)
			{
				Trail.GetComponent<LineRenderer>().material.color = new Color(.32f, 1f, .41f);
				WheelTrail.GetComponent<LineRenderer>().material.color = new Color(.32f, 1f, .41f);
				Glow.GetComponent<SpriteRenderer>().sprite = GlowPerfect;
			}
			else if (mode == Modes.Awesome)
			{
				Trail.GetComponent<LineRenderer>().material.color = new Color(0, .95f, 1f);
				WheelTrail.GetComponent<LineRenderer>().material.color = new Color(0, .95f, 1f);
				Glow.GetComponent<SpriteRenderer>().sprite = GlowAwesome;
			}
		}
		
		Glow.SetActive(mode != Modes.Normal);
	}

	private void TurningDriftEffect()
	{
		if (GamePlayManager.SelectedPim.IsRotating)
		{
			var to = transform.parent.position - transform.position;

			var from = GamePlayManager.TurningEffect(GamePlayManager.SelectedPim.RotateToClockwise);
		
			var q = Quaternion.FromToRotation(from, to);

			transform.rotation = Quaternion.Lerp(transform.rotation, q , .2f);
		}
	}

	private void HandleVelocity()
	{
		ForwardVelocity = _rigidbody2D.velocity.ProjectionOnto(transform.up);
		var otherComponent = _rigidbody2D.velocity - ForwardVelocity;
		
		// Arabayı Toparlamak için
		_rigidbody2D.AddTorque(-_rigidbody2D.angularVelocity * .05f, ForceMode2D.Force);
		_rigidbody2D.AddForce(-otherComponent*1f, ForceMode2D.Force);

		
		if (!AttachedToPin || !GamePlayManager.SelectedPim.IsRotating)
		{
			if (ForwardVelocity.sqrMagnitude < GamePlayManager.MaxVelocity)
			{
				_rigidbody2D.AddForce(transform.up*4f, ForceMode2D.Force);
			}
			else if (ForwardVelocity.sqrMagnitude > GamePlayManager.MaxVelocity)
			{
				_rigidbody2D.velocity *= (GamePlayManager.MaxVelocity + (ForwardVelocity.sqrMagnitude - GamePlayManager.MaxVelocity)*.975f)/ForwardVelocity.sqrMagnitude;
			}
		}
	}

	private void BalanceCar()
	{
		if (Vector2.Angle(Balancer.transform.up, transform.up) > 60) return;
		
		var v1 = transform.position - Balancer.transform.position;
		var v2 = v1.ProjectionOnto(Balancer.transform.up);

		var v3 = v1 - v2 + Balancer.transform.up*1f;

		Quaternion destRotation = Quaternion.FromToRotation(v3, Balancer.transform.up);
		
		destRotation *= Balancer.transform.rotation;

		Vector3 velocity = _rigidbody2D.velocity;
		var deviation = velocity.ProjectionOnto(v1 - v2); // deviation component of the car's velocity according to balancer object direction
		
		if (Vector2.Angle(deviation, (v1 - v2).normalized) > 10)
		{
			destRotation = Balancer.transform.rotation;
		}
		
		transform.rotation = Quaternion.Lerp(transform.rotation, destRotation, .1f);
	}

	public void StartDriftTrail()
	{
		if (_driftCoroutine != null) StopCoroutine(_driftCoroutine);
		
		_leftWheelTrail = Instantiate(WheelTrail, Vector3.zero, Quaternion.identity);
		_rightWheelTrail = Instantiate(WheelTrail, Vector3.zero, Quaternion.identity);
		
		_leftWheelTrail.transform.SetParent(WheelTrail.transform.parent);
		_rightWheelTrail.transform.SetParent(WheelTrail.transform.parent);

		if (GamePlayManager.Instance.CurrentMode == Modes.Normal)
		{
			_leftWheelTrail.GetComponent<LineRenderer>().startColor = new Color(0,0,0,.1f);
			_leftWheelTrail.GetComponent<LineRenderer>().endColor = Color.black;
			_rightWheelTrail.GetComponent<LineRenderer>().endColor = Color.black;
			_rightWheelTrail.GetComponent<LineRenderer>().startColor = new Color(0,0,0,.1f);
		}
		
		_driftCoroutine = StartCoroutine(DrawDriftTrail());
	}
	
	private IEnumerator DrawDriftTrail()
	{
		var leftLineRenderer = _leftWheelTrail.GetComponent<LineRenderer>();
		leftLineRenderer.positionCount = leftLineRenderer.positionCount+1;
		leftLineRenderer.SetPosition(leftLineRenderer.positionCount-1, LeftRearWheel.transform.position);
		
		var rightLineRenderer = _rightWheelTrail.GetComponent<LineRenderer>();
		rightLineRenderer.positionCount++;
		rightLineRenderer.SetPosition(rightLineRenderer.positionCount-1, RightRearWheel.transform.position);

		
		yield return new WaitForEndOfFrame();
		
		_driftCoroutine = StartCoroutine(DrawDriftTrail());

	}

	public void StopDriftTrail()
	{
		if (_driftCoroutine == null) return;
		
		_leftWheelTrail.AddComponent<DriftTrailTimer>();
		_rightWheelTrail.AddComponent<DriftTrailTimer>();
		
		StopCoroutine(_driftCoroutine);
		_driftCoroutine = null;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.transform.CompareTag(Tags.Track))
		{
			if (GamePlayManager.Instance.GamePhase != GamePhase.Play) return;
			
			GamePlayManager.SelectedPim.SetActive(false);
			
			transform.GetComponent<SpriteRenderer>().enabled = false;
			_explosionObject.transform.position = transform.position;
			_explosionObject.GetComponent<ExplosionManager>().Explode();
			enabled = false;
			Glow.SetActive(false);
			_linePassed = false;
			CameraEffects.Instance.Shake(.1f, .3f);

			ResetToDefault();
			
			GamePlayManager.Instance.GameOver();
			StartCoroutine(RespawnAfterSeconds(.5f));
			
			GamePlayManager.Instance.Track.SetupBridge(null);
		}
	}

	private float _lastTriggerTime;
	private const float CanTriggerAgainAfterSeconds = 1f;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.transform.CompareTag(Tags.Balancer))
		{
			Balancer = other.gameObject;
		}
		else if (other.transform.CompareTag(Tags.ReleaseBalancer))
		{
			Balancer = null;
		}
		else if (other.transform.CompareTag(Tags.StartFinish))
		{
			if (Time.time - _lastTriggerTime < CanTriggerAgainAfterSeconds) return; //Block multiple triggerEnter events
			_lastTriggerTime = Time.time;

			_linePassed = true;
			
			GamePlayManager.Instance.Track.SetupBridge(other.gameObject);
		}
		else if (other.transform.CompareTag(Tags.Coin))
		{
			GamePlayManager.Instance.CollectCoin(other.gameObject);
		}
	}

	private void ResetToDefault()
	{
		Balancer = null;
		_rigidbody2D.velocity = Vector2.zero;
		_rigidbody2D.angularVelocity = 0;
		_linePassed = false;
		transform.position = GamePlayManager.Instance.Track.CarInitialPosition;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, GamePlayManager.Instance.Track.CarInitialZRotation));
		Trail.GetComponent<TrailScript>().StopTrail();
		
		enabled = true;
	}

	private IEnumerator RespawnAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);

		transform.GetComponent<SpriteRenderer>().enabled = true;	
	}
}
