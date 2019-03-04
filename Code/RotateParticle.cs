using UnityEngine;
using Utility;

internal class RotateParticle : MonoBehaviour
{
	[Tooltip("Changes Start Rotation by rotationRate degrees per second")]
	public float rotationRate = 360f;

	private float _currentRotation;

	private ParticleSystem _particle;

	public RotateParticle()
		: this()
	{
	}

	private void Awake()
	{
		_particle = this.GetComponent<ParticleSystem>();
		if (_particle == null)
		{
			Console.LogError("RotateParticle is not on an object with a ParticleSystem");
			Object.Destroy(this);
		}
		else
		{
			_currentRotation = _particle.get_startRotation();
		}
	}

	private void Update()
	{
		_currentRotation += rotationRate * Time.get_deltaTime();
		while (_currentRotation < 0f)
		{
			_currentRotation += 360f;
		}
		while (_currentRotation > 360f)
		{
			_currentRotation -= 360f;
		}
		_particle.set_startRotation(_currentRotation);
	}
}
