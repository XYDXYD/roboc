using System;
using UnityEngine;
using Utility;

internal sealed class MouseSettings
{
	private float _buildSpeed;

	private float _fightSpeed;

	private bool _invertY;

	private bool _toggleZoom;

	public const string MOUSE_SPEED_BUILD = "MouseSpeedBuild";

	public const string MOUSE_SPEED_FIGHT = "MouseSpeedFight";

	public const string MOUSE_INVERT_Y = "MouseInvertY";

	public const string TOGGLE_ZOOM_MODE = "ToggleZoom";

	private const float DEFAULT_MOUSE_SPEED = 0.5f;

	private const bool DEFAULT_INVERT_Y = false;

	private const bool DEFAULT_TOGGLE_ZOOM = false;

	public event Action<float, float, bool> OnChangeMouseSettings = delegate
	{
	};

	public MouseSettings()
	{
		_buildSpeed = PlayerPrefs.GetFloat("MouseSpeedBuild", 0.5f);
		_fightSpeed = PlayerPrefs.GetFloat("MouseSpeedFight", 0.5f);
		if (PlayerPrefs.HasKey("MouseInvertY"))
		{
			_invertY = (PlayerPrefs.GetInt("MouseInvertY") == 1);
		}
		if (PlayerPrefs.HasKey("ToggleZoom"))
		{
			_toggleZoom = (PlayerPrefs.GetInt("ToggleZoom") == 1);
		}
	}

	public float GetBuildSpeed()
	{
		if (PlayerPrefs.HasKey("MouseSpeedBuild"))
		{
			return PlayerPrefs.GetFloat("MouseSpeedBuild");
		}
		return 0.5f;
	}

	public float GetFightSpeed()
	{
		if (PlayerPrefs.HasKey("MouseSpeedFight"))
		{
			return PlayerPrefs.GetFloat("MouseSpeedFight");
		}
		return 0.5f;
	}

	public bool IsInvertY()
	{
		if (PlayerPrefs.HasKey("MouseInvertY"))
		{
			return PlayerPrefs.GetInt("MouseInvertY") == 1;
		}
		return false;
	}

	public bool IsToggleZoom()
	{
		if (PlayerPrefs.HasKey("ToggleZoom"))
		{
			return PlayerPrefs.GetInt("ToggleZoom") == 1;
		}
		return false;
	}

	public void ChangeSettings(float buildSpeed, float fightSpeed, bool invertY, bool toggleZoom)
	{
		_buildSpeed = buildSpeed;
		_fightSpeed = fightSpeed;
		_invertY = invertY;
		_toggleZoom = toggleZoom;
		Console.Log("build speed set to: " + buildSpeed + " fight speed set to " + fightSpeed + " invert set to " + _invertY);
		this.OnChangeMouseSettings(_buildSpeed, _fightSpeed, _invertY);
	}

	public void SaveMouseSettings()
	{
		PlayerPrefs.SetFloat("MouseSpeedBuild", _buildSpeed);
		PlayerPrefs.SetFloat("MouseSpeedFight", _fightSpeed);
		PlayerPrefs.SetInt("MouseInvertY", _invertY ? 1 : 0);
		PlayerPrefs.SetInt("ToggleZoom", _toggleZoom ? 1 : 0);
	}
}
