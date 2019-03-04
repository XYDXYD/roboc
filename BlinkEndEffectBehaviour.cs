using UnityEngine;

public class BlinkEndEffectBehaviour : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	public BlinkEndEffectBehaviour()
		: this()
	{
	}

	private void Awake()
	{
		_particleSystem = this.GetComponent<ParticleSystem>();
	}

	private void OnEnable()
	{
		_particleSystem.Play();
	}

	private void OnDisable()
	{
		_particleSystem.Stop();
	}
}
