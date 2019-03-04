using Svelto.ES.Legacy;
using UnityEngine;

internal sealed class WorldSwitchAnimation : MonoBehaviour, IRunOnWorldSwitching, IComponent
{
	public bool fadeIn;

	public AnimationClip clip;

	public int priority = 2;

	public bool FadeIn => fadeIn;

	public int Priority => priority;

	public float Duration => (!(clip != null)) ? 0f : clip.get_length();

	public WorldSwitchAnimation()
		: this()
	{
	}

	public void Execute(WorldSwitchMode currentMode)
	{
		if (this != null)
		{
			Animation component = this.GetComponent<Animation>();
			if (component != null && clip != null)
			{
				component.Play(clip.get_name());
			}
		}
	}
}
