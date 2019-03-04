using UnityEngine;

internal sealed class LightSwitcher : MonoBehaviour
{
	public LightSwitcher()
		: this()
	{
	}

	private void Start()
	{
		if (QualitySettings.GetQualityLevel() <= 2)
		{
			Projector componentInChildren = this.GetComponentInChildren<Projector>();
			componentInChildren.set_ignoreLayers(GameLayers.PROJECTOR_MUST_IGNORE_LAYER_MASK);
			Light componentInChildren2 = this.GetComponentInChildren<Light>();
			if (componentInChildren2 != null)
			{
				Object.Destroy(componentInChildren2.get_gameObject());
			}
		}
		else
		{
			Projector componentInChildren3 = this.GetComponentInChildren<Projector>();
			if (componentInChildren3 != null)
			{
				Object.Destroy(componentInChildren3.get_gameObject());
			}
		}
	}
}
