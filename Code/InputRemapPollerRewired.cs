using Rewired;
using UnityEngine;

internal class InputRemapPollerRewired
{
	private int _playerId;

	private int _controllerId;

	private ControllerType _controllerType;

	private ControllerPollingInfo _pollingInfo = default(ControllerPollingInfo);

	public InputRemapPollerRewired(ControllerType controllerType)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_controllerType = controllerType;
	}

	public ModifierKeyFlags GetModifierKeyFlags()
	{
		return 0;
	}

	public ControllerPollingInfo GetAssignmentInfo()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return _pollingInfo;
	}

	public bool PollControllerForAssignment()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		bool result = false;
		ControllerType controllerType = _controllerType;
		if ((int)controllerType != 0)
		{
			if ((int)controllerType != 2)
			{
				if ((int)controllerType == 1)
				{
					result = PollMouseForAssignment();
				}
			}
			else
			{
				result = PollJoystickForAssignment();
			}
		}
		else
		{
			result = PollKeyboardForAssignment();
		}
		return result;
	}

	private bool PollKeyboardForAssignment()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		foreach (ControllerPollingInfo item in ReInput.get_controllers().get_Keyboard().PollForAllKeys())
		{
			ControllerPollingInfo current = item;
			if (IsKeyAllowed(current.get_keyboardKey()))
			{
				_pollingInfo = current;
				return true;
			}
		}
		return false;
	}

	private static bool IsKeyAllowed(KeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		return (int)key != 0 && (int)key != 27;
	}

	private bool PollJoystickForAssignment()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Player player = ReInput.get_players().GetPlayer(_playerId);
		if (player == null)
		{
			return false;
		}
		_pollingInfo = player.controllers.polling.PollControllerForFirstButton(_controllerType, _controllerId);
		return _pollingInfo.get_success();
	}

	private bool PollMouseForAssignment()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Player player = ReInput.get_players().GetPlayer(_playerId);
		if (player == null)
		{
			return false;
		}
		_pollingInfo = player.controllers.polling.PollControllerForFirstButton(_controllerType, _controllerId);
		return _pollingInfo.get_success();
	}
}
