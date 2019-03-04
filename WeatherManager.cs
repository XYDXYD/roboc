using System;

internal sealed class WeatherManager
{
	private WeatherType _weatherType;

	private event Action<WeatherType> _OnWeatherChange = delegate
	{
	};

	public void SetWeatherType(WeatherType weatherType)
	{
		_weatherType = weatherType;
		this._OnWeatherChange(_weatherType);
	}

	public void RegisterOnWeatherChange(Action<WeatherType> onWeatherChange)
	{
		_OnWeatherChange += onWeatherChange;
		this._OnWeatherChange(_weatherType);
	}

	public void UnregisterOnWeatherChange(Action<WeatherType> onWeatherChange)
	{
		_OnWeatherChange -= onWeatherChange;
	}
}
