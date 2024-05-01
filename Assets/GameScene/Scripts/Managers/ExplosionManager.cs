using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
	private List<GameObject> _particles;
	private List<Vector3> _velocities;

	private bool _isActive;

	public float ParticleLifeTime = 1;
	public float ParticleExplosionVelocity = .1f;
	private float _particleStartTime;

	private void Start()
	{
		_particles = new List<GameObject>();
		_velocities = new List<Vector3>();
	}

	private void Update()
	{
		if (_isActive)
		{
			_particleStartTime += Time.deltaTime;
			for (var i = 0; i < _particles.Count; i++)
			{
				_particles[i].transform.position += (ParticleLifeTime - _particleStartTime) * _velocities[i];
				Color color = _particles[i].GetComponent<SpriteRenderer>().color;
				color.a = ParticleLifeTime - _particleStartTime;
				_particles[i].GetComponent<SpriteRenderer>().color = color;
			}
		}

		if (_particleStartTime > ParticleLifeTime)
		{
			Reset();
		}
	}

	private void Reset()
	{
		_isActive = false;
		transform.position = new Vector3(-8,0,0);
		for (var i = 0; i < _particles.Count; i++)
		{
			_particles[i].transform.localPosition = Vector3.zero;
		}
	}

	public void Explode()
	{
		if (_isActive) return;
		
		_isActive = true;
		_particleStartTime = 0;
		
		_velocities.Clear();
		_particles.Clear();
		for (var i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder =
				GamePlayManager.Instance.Car.GetComponent<SpriteRenderer>().sortingOrder;
			_particles.Add(transform.GetChild(i).gameObject);
			_velocities.Add(new Vector3(Random.Range(-ParticleExplosionVelocity, ParticleExplosionVelocity), Random.Range(-ParticleExplosionVelocity, ParticleExplosionVelocity), 0));
		}

	}
}
