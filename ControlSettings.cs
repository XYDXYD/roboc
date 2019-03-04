using System;
using Utility;

public struct ControlSettings
{
	public ControlType controlType;

	public bool verticalStrafing;

	public bool sidewaysDriving;

	public bool tracksTurnOnSpot;

	public ControlSettings(int controlType, bool[] controlOptions)
	{
		this = default(ControlSettings);
		if (Enum.IsDefined(typeof(ControlType), controlType))
		{
			this.controlType = (ControlType)controlType;
		}
		else
		{
			this.controlType = ControlType.CameraControl;
			Console.LogWarning($"Unsupported control type {controlType}, defaulting to {ControlType.CameraControl}");
		}
		verticalStrafing = controlOptions[0];
		sidewaysDriving = controlOptions[1];
		tracksTurnOnSpot = controlOptions[2];
	}

	public bool[] options()
	{
		return new bool[3]
		{
			verticalStrafing,
			sidewaysDriving,
			tracksTurnOnSpot
		};
	}

	public override bool Equals(object obj)
	{
		if (obj is ControlSettings)
		{
			return this == (ControlSettings)obj;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = (int)controlType;
		num = ((num << 1) | (verticalStrafing ? 1 : 0));
		num = ((num << 1) | (sidewaysDriving ? 1 : 0));
		return (num << 1) | (tracksTurnOnSpot ? 1 : 0);
	}

	public static bool operator ==(ControlSettings a, ControlSettings b)
	{
		return a.controlType == b.controlType && a.verticalStrafing == b.verticalStrafing && a.sidewaysDriving == b.sidewaysDriving && a.tracksTurnOnSpot == b.tracksTurnOnSpot;
	}

	public static bool operator !=(ControlSettings a, ControlSettings b)
	{
		return !(a == b);
	}
}
