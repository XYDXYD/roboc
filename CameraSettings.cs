using System;
using UnityEngine;
using Utility;

internal sealed class CameraSettings
{
	private const string ENABLE_CAMERA_SHAKE = "EnableCameraShake";

	private const bool DEFAULT_CAMERA_SHAKE_ENABLED = true;

	private bool _enableCameraShake;

	public event Action<bool> OnChangeCameraSettings = delegate
	{
	};

	public CameraSettings()
	{
		if (PlayerPrefs.HasKey("EnableCameraShake"))
		{
			_enableCameraShake = (PlayerPrefs.GetInt("EnableCameraShake") == 1);
		}
		else
		{
			_enableCameraShake = true;
		}
	}

	public bool IsCameraShakeEnabled()
	{
		return _enableCameraShake;
	}

	public void ChangeSettings(bool enableCameraShake)
	{
		_enableCameraShake = enableCameraShake;
		Console.Log("enable camera shake set to: " + _enableCameraShake);
		this.OnChangeCameraSettings(_enableCameraShake);
	}

	public void SaveCameraSettings()
	{
		PlayerPrefs.SetInt("EnableCameraShake", _enableCameraShake ? 1 : 0);
	}
}
