using UnityEngine;

internal sealed class TeleportEffect : MonoBehaviour
{
	public GameObject startEffect;

	public GameObject endEffect;

	public GameObject trailEffect;

	public TrailRenderer[] trailRenderers;

	public ParticleSystemRenderer trailEffectParticleRenderer;

	public ParticleSystem trailEffectParticleSystem;

	public float trailTime;

	public TeleportEffect()
		: this()
	{
	}

	private void Start()
	{
		trailEffect = Object.Instantiate<GameObject>(trailEffect);
		trailEffectParticleRenderer = trailEffect.GetComponentInChildren<ParticleSystemRenderer>();
		trailEffectParticleSystem = trailEffect.GetComponentInChildren<ParticleSystem>();
		trailRenderers = trailEffect.GetComponentsInChildren<TrailRenderer>();
		trailTime = trailRenderers[0].get_time();
		for (int i = 0; i < trailRenderers.Length; i++)
		{
			trailRenderers[i].set_enabled(false);
		}
		endEffect = Object.Instantiate<GameObject>(endEffect);
		startEffect = Object.Instantiate<GameObject>(startEffect);
	}
}
