using Svelto.ES.Legacy;
using System;
using UnityEngine;

internal sealed class AdvancedRobotEditSettings : IHandleEditingInput, IInputComponent, IComponent
{
	private const string PLAYER_PREF_ADVANCE_EDIT_COM = "AdvancedEditCenterOfMass";

	private const string PLAYER_PREF_ADVANCE_EDIT_HINTS = "AdvancedEditHints";

	public bool centerOfMass
	{
		get;
		private set;
	}

	public bool showHints
	{
		get;
		private set;
	}

	public event Action<bool> OnToggleCenterOfMass = delegate
	{
	};

	public AdvancedRobotEditSettings()
	{
		centerOfMass = (PlayerPrefs.GetInt("AdvancedEditCenterOfMass", 0) == 1);
		showHints = (PlayerPrefs.GetInt("AdvancedEditHints", 1) == 1);
	}

	public void HandleEditingInput(InputEditingData data)
	{
		if (data[EditingInputAxis.TOGGLE_CENTER_OF_MASS] == 1f)
		{
			ToggleCenterOfMass();
			data.Set(EditingInputAxis.TOGGLE_CENTER_OF_MASS, 0f);
		}
	}

	public void UpdateSettings(bool enableCenterOfMass, bool showHints_)
	{
		UpdateCenterOfMass(enableCenterOfMass);
		showHints = showHints_;
		PlayerPrefs.SetInt("AdvancedEditHints", showHints_ ? 1 : 0);
	}

	private void UpdateCenterOfMass(bool enableCenterOfMass)
	{
		centerOfMass = enableCenterOfMass;
		PlayerPrefs.SetInt("AdvancedEditCenterOfMass", enableCenterOfMass ? 1 : 0);
		this.OnToggleCenterOfMass(enableCenterOfMass);
	}

	private void ToggleCenterOfMass()
	{
		centerOfMass = !centerOfMass;
		UpdateCenterOfMass(centerOfMass);
	}

	public void TurnOffAdvancedEditSettings()
	{
		centerOfMass = false;
		UpdateCenterOfMass(centerOfMass);
	}
}
