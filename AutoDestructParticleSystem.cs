using UnityEngine;

[ExecuteInEditMode]
internal class AutoDestructParticleSystem : MonoBehaviour
{
	public ParticleSystem particleSystem;

	public AutoDestructParticleSystem()
		: this()
	{
	}

	private void Awake()
	{
		if (particleSystem == null)
		{
			particleSystem = this.GetComponent<ParticleSystem>();
		}
		this.set_enabled(true);
	}

	private void LateUpdate()
	{
		if (this.get_isActiveAndEnabled() && Application.get_isPlaying() && particleSystem != null && !particleSystem.get_loop() && !particleSystem.get_isPlaying() && !particleSystem.IsAlive(true))
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
