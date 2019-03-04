using Svelto.IoC;
using UnityEngine;

internal class WeatherSetter : MonoBehaviour
{
	public WeatherType weatherType;

	[Inject]
	internal WeatherManager weatherManager
	{
		private get;
		set;
	}

	public WeatherSetter()
		: this()
	{
	}

	private void Start()
	{
		weatherManager.SetWeatherType(weatherType);
	}
}
