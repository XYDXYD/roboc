using UnityEngine;

internal sealed class BlinkStartEffectBehaviour : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	public BlinkStartEffectBehaviour()
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
