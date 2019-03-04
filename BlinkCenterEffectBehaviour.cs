using UnityEngine;

public class BlinkCenterEffectBehaviour : MonoBehaviour
{
	private ParticleSystem _particleSystem;

	private ParticleSystemRenderer _particleSystemRenderer;

	public BlinkCenterEffectBehaviour()
		: this()
	{
	}

	private void Awake()
	{
		_particleSystem = this.GetComponent<ParticleSystem>();
		_particleSystemRenderer = this.GetComponent<ParticleSystemRenderer>();
	}

	private void OnEnable()
	{
		_particleSystemRenderer.set_enabled(true);
		_particleSystem.Play();
	}

	private void OnDisable()
	{
		_particleSystemRenderer.set_enabled(false);
		_particleSystem.Stop();
	}
}
