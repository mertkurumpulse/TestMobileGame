using UnityEngine;

public class PinManager : MonoBehaviour
{
	public bool RotateToClockwise;

	public bool IsActive = false;

	public bool IsRotating = false;

	public float ForwardVelocityOnAttached { get; private set; }

	public GameObject Rotator;
	public RopeManager RopeManager;

	private void OnValidate()
	{
		Rotator = transform.GetChild(0).gameObject;
		RopeManager = transform.GetChild(1).GetComponent<RopeManager>();
	}

	private void Update()
	{
		if (!IsActive)
		{
			IsRotating = false;
			return;
		}
		
		if (RopeManager.RopeIsActive && (transform.InverseTransformPoint(GamePlayManager.Instance.Car.transform.position).y < 0 || IsRotating))
		{
			var car = GamePlayManager.Instance.Car;
			
			if (!IsRotating)
			{
				ForwardVelocityOnAttached = GamePlayManager.Instance.Car.ForwardVelocity.magnitude;
				car.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				GamePlayManager.Instance.Car.StartDriftTrail();
			}
			
			IsRotating = true;

			var radius = Vector2.Distance(Rotator.transform.position, car.transform.position);
			var perimeter = 2 * Mathf.PI * radius;

			var f = ForwardVelocityOnAttached*1.4f / perimeter;

			Rotator.transform.localEulerAngles = new Vector3(0, 0,
				Rotator.transform.localEulerAngles.z + Time.deltaTime * f * 360 * (RotateToClockwise ? -1 : 1));
			
			
			if (ForwardVelocityOnAttached < Mathf.Sqrt(GamePlayManager.MaxVelocity))
			{
				ForwardVelocityOnAttached += Time.deltaTime*4;

				if (car.transform.localPosition.magnitude > 1.5f)
				{
					car.transform.localPosition *= .995f;
				}
			}
		}
		else
		{
			IsRotating = false;
		}
	}

	private void LateUpdate()
	{
		if (!IsActive)
		{
			IsRotating = false;
			return;
		}

		RopeManager.CreateRope();
	}


	public void SetActive(bool isActive)
	{
		IsActive = isActive;

		if (IsActive)
		{
			GamePlayManager.Instance.Car.transform.SetParent(Rotator.transform);
		}
		else
		{
			if (IsRotating)
			{
				var car = GamePlayManager.Instance.Car;
				car.GetComponent<Rigidbody2D>().velocity = ForwardVelocityOnAttached * car.transform.up + (RotateToClockwise ? -1 : 1)*GamePlayManager.CentrifugalForce*ForwardVelocityOnAttached*car.transform.right;
				GamePlayManager.Instance.Car.StopDriftTrail();
			}
			GamePlayManager.Instance.Car.transform.SetParent(null);
			RopeManager.DestroyRope();
			RopeManager.RopeIsActive = false;
			ForwardVelocityOnAttached = 0;
		}
	}
}
