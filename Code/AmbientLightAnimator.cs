using UnityEngine;

public class AmbientLightAnimator : MonoBehaviour
{
	public float ambientIntensity = 1f;

	public AmbientLightAnimator()
		: this()
	{
	}

	private void Update()
	{
		RenderSettings.set_ambientIntensity(ambientIntensity);
	}
}
