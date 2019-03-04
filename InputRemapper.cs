using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal class InputRemapper
{
	private int _playerId;

	private Player _player;

	private static InputRemapper _instance;

	public static InputRemapper Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new InputRemapper();
			}
			return _instance;
		}
	}

	private InputRemapper()
	{
		_player = ReInput.get_players().GetPlayer(_playerId);
	}

	public float GetAxis(string axisName)
	{
		return _player.GetAxis(axisName);
	}

	public float GetAxis(int actionId)
	{
		return _player.GetAxis(actionId);
	}

	public bool GetButtonDown(string axisName)
	{
		return _player.GetButtonDown(axisName);
	}

	public bool GetButtonDown(int actionId)
	{
		return _player.GetButtonDown(actionId);
	}

	public bool GetButtonUp(string axisName)
	{
		return _player.GetButtonUp(axisName);
	}

	public bool GetButtonUp(int actionId)
	{
		return _player.GetButtonUp(actionId);
	}

	public bool GetButton(string axisName)
	{
		return _player.GetButton(axisName);
	}

	public bool GetButton(int actionId)
	{
		return _player.GetButton(actionId);
	}

	public bool GetKeyboardKey(KeyCode keyCode)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return _player.controllers.get_Keyboard().GetKey(keyCode);
	}

	public string GetInputActionKeyMap(string inputActionName)
	{
		if (inputActionName.Contains("+") || inputActionName.Contains("-"))
		{
			return GetInputActionAxisKeyMap(inputActionName);
		}
		ActionElementMap firstButtonMapWithAction = _player.controllers.maps.GetFirstButtonMapWithAction(inputActionName, true);
		if (firstButtonMapWithAction == null)
		{
			return string.Empty;
		}
		return firstButtonMapWithAction.get_elementIdentifierName().ToUpper();
	}

	public KeyboardKeyCode GetInputActionKeyCode(string inputActionName)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (inputActionName.Contains("+") || inputActionName.Contains("-"))
		{
			string text = inputActionName.Replace("+", string.Empty).Replace("-", string.Empty);
			IEnumerable<ActionElementMap> enumerable = _player.controllers.maps.ElementMapsWithAction(text, true);
			IEnumerator<ActionElementMap> enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ActionElementMap current = enumerator.Current;
				if ((inputActionName.Contains("+") && (int)current.get_axisContribution() == 0) || (inputActionName.Contains("-") && (int)current.get_axisContribution() == 1))
				{
					return current.get_keyboardKeyCode();
				}
			}
		}
		ActionElementMap firstButtonMapWithAction = _player.controllers.maps.GetFirstButtonMapWithAction(inputActionName, true);
		if (firstButtonMapWithAction == null)
		{
			return 0;
		}
		return firstButtonMapWithAction.get_keyboardKeyCode();
	}

	private string GetInputActionAxisKeyMap(string inputActionName)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		string text = inputActionName.Replace("+", string.Empty).Replace("-", string.Empty);
		IEnumerable<ActionElementMap> enumerable = _player.controllers.maps.ElementMapsWithAction(text, true);
		IEnumerator<ActionElementMap> enumerator = enumerable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ActionElementMap current = enumerator.Current;
			if ((inputActionName.Contains("+") && (int)current.get_axisContribution() == 0) || (inputActionName.Contains("-") && (int)current.get_axisContribution() == 1))
			{
				return current.get_elementIdentifierName().ToUpper();
			}
		}
		return inputActionName;
	}

	public IList<InputCategory> GetCategories()
	{
		return ReInput.get_mapping().get_ActionCategories();
	}

	public IEnumerable<InputAction> GetActionsInCategory(int categoryId)
	{
		return GetDisplayOrderedActionsInCategory(categoryId);
	}

	private IEnumerable<InputAction> GetDisplayOrderedActionsInCategory(int categoryId)
	{
		List<InputAction> list = new List<InputAction>();
		for (int i = 0; i < InputActions.actionDisplayOrder.Length; i++)
		{
			InputAction action = ReInput.get_mapping().GetAction(InputActions.actionDisplayOrder[i]);
			if (action == null)
			{
				Console.LogError("Null InputAction '" + InputActions.actionDisplayOrder[i] + "'");
			}
			if (action.get_categoryId() == categoryId)
			{
				list.Add(action);
			}
		}
		return list;
	}

	public void RegisterOnControllerConnected(Action onControllerConnected)
	{
		ReInput.add_ControllerConnectedEvent((Action<ControllerStatusChangedEventArgs>)delegate
		{
			onControllerConnected();
		});
	}

	public void RegisterOnControllerDisconnected(Action onControllerDisconnected)
	{
		ReInput.add_ControllerDisconnectedEvent((Action<ControllerStatusChangedEventArgs>)delegate
		{
			onControllerDisconnected();
		});
	}

	public void UnregisterOnControllerConnected(Action onControllerConnected)
	{
		ReInput.remove_ControllerConnectedEvent((Action<ControllerStatusChangedEventArgs>)delegate
		{
			onControllerConnected();
		});
	}

	public void UnregisterOnControllerDisconnected(Action onControllerDisconnected)
	{
		ReInput.remove_ControllerDisconnectedEvent((Action<ControllerStatusChangedEventArgs>)delegate
		{
			onControllerDisconnected();
		});
	}
}
