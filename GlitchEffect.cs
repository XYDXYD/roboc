using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class GlitchEffect : ImageEffectBase
{
	public Texture2D displacementMap;

	private float glitchup;

	private float glitchdown;

	private float flicker;

	private float glitchupTime = 0.05f;

	private float glitchdownTime = 0.05f;

	private float flickerTime = 0.5f;

	public float intensity;

	public GlitchEffect()
		: this()
	{
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.get_material().SetFloat("_Intensity", intensity);
		this.get_material().SetTexture("_DispTex", displacementMap);
		glitchup += Time.get_deltaTime() * intensity;
		glitchdown += Time.get_deltaTime() * intensity;
		flicker += Time.get_deltaTime() * intensity;
		if (flicker > flickerTime)
		{
			this.get_material().SetFloat("filterRadius", Random.Range(-3f, 3f) * intensity);
			flicker = 0f;
			flickerTime = Random.get_value();
		}
		if (glitchup > glitchupTime)
		{
			if (Random.get_value() < 0.1f * intensity)
			{
				this.get_material().SetFloat("flip_up", Random.Range(0f, 1f) * intensity);
			}
			else
			{
				this.get_material().SetFloat("flip_up", 0f);
			}
			glitchup = 0f;
			glitchupTime = Random.get_value() / 10f;
		}
		if (glitchdown > glitchdownTime)
		{
			if (Random.get_value() < 0.1f * intensity)
			{
				this.get_material().SetFloat("flip_down", 1f - Random.Range(0f, 1f) * intensity);
			}
			else
			{
				this.get_material().SetFloat("flip_down", 1f);
			}
			glitchdown = 0f;
			glitchdownTime = Random.get_value() / 10f;
		}
		if ((double)Random.get_value() < 0.05 * (double)intensity)
		{
			this.get_material().SetFloat("displace", Random.get_value() * intensity);
			this.get_material().SetFloat("scale", 1f - Random.get_value() * intensity);
		}
		else
		{
			this.get_material().SetFloat("displace", 0f);
		}
		Graphics.Blit(source, destination, this.get_material());
	}
}
