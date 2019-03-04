using Svelto.IoC;
using UnityEngine;

internal class WeatherSwitcher : MonoBehaviour
{
	public ParticleSystem particleSystem;

	[Inject]
	internal WeatherManager weatherManager
	{
		private get;
		set;
	}

	public WeatherSwitcher()
		: this()
	{
	}

	private void Start()
	{
		weatherManager.RegisterOnWeatherChange(WeatherChanged);
	}

	private void OnDestroy()
	{
		weatherManager.UnregisterOnWeatherChange(WeatherChanged);
	}

	private void WeatherChanged(WeatherType weatherType)
	{
		if (particleSystem != null)
		{
			if (weatherType == WeatherType.snow)
			{
				particleSystem.Play();
				return;
			}
			particleSystem.Stop();
			particleSystem.Clear();
		}
	}
}
