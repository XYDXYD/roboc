using Rewired;
using Svelto.DataStructures;
using System.Collections.Generic;
using System.Text;

internal class ButtonReassignData
{
	private int _actionId;

	private string _actionName;

	private Pole _actionAxisContribution;

	private IEnumerable<ActionElementMap> _elementMapsForAction;

	private ControllerMap _controllerMap;

	private InputActionType _actionType;

	private bool _assignFullAxis;

	public ButtonReassignData(int actionId, string actionName, Pole actionAxisContribution, IEnumerable<ActionElementMap> buttonsForAction, ControllerMap controllerMap, InputActionType actionType, bool assignFullAxis)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		_actionId = actionId;
		_actionName = actionName;
		_actionAxisContribution = actionAxisContribution;
		_elementMapsForAction = buttonsForAction;
		_controllerMap = controllerMap;
		_actionType = actionType;
		_assignFullAxis = assignFullAxis;
	}

	public string ActionName(bool localised = true)
	{
		if (localised)
		{
			return StringTableBase<StringTable>.Instance.GetString(_actionName);
		}
		return _actionName;
	}

	public bool IsAxis()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		return (int)_actionType == 0;
	}

	public bool IsSplit()
	{
		return !_assignFullAxis;
	}

	public string ActionsToString()
	{
		if (_elementMapsForAction == null)
		{
			return string.Empty;
		}
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ActionElementMap item in _elementMapsForAction)
		{
			if (num > 0)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetMouseJoystickString(item.get_elementIdentifierName().Replace("+", string.Empty).Replace("-", string.Empty)));
			if (item.get_elementIdentifierName().Contains("-"))
			{
				stringBuilder.Append("-");
			}
			if (item.get_elementIdentifierName().Contains("+"))
			{
				stringBuilder.Append("+");
			}
			num++;
		}
		return stringBuilder.ToString();
	}

	public KeyboardKeyCode KeyCode()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (_elementMapsForAction != null)
		{
			using (IEnumerator<ActionElementMap> enumerator = _elementMapsForAction.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ActionElementMap current = enumerator.Current;
					return current.get_keyboardKeyCode();
				}
			}
			return 0;
		}
		return 0;
	}

	public bool CanRebind()
	{
		return _controllerMap != null;
	}

	public void RemoveElementMapsForAction()
	{
		if (_elementMapsForAction != null)
		{
			foreach (ActionElementMap item in _elementMapsForAction)
			{
				_controllerMap.DeleteElementMap(item.get_id());
			}
		}
	}

	public void ReplaceOrCreateElementMap(InputRemapPollerRewired poller)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ElementAssignmentConflictCheck elementAssignmentConflictCheck = ToAssignmentConflictCheck(poller.GetAssignmentInfo(), poller.GetModifierKeyFlags());
		RemoveConflicts(elementAssignmentConflictCheck);
		_controllerMap.ReplaceOrCreateElementMap(elementAssignmentConflictCheck.ToElementAssignment());
	}

	public void ReplaceOrCreateElementMap(string elementName, ControllerType controllerType)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		FasterList<ControllerElementIdentifier> val = new FasterList<ControllerElementIdentifier>();
		Player player = ReInput.get_players().GetPlayer(0);
		if ((int)controllerType == 1)
		{
			val.AddRange((ICollection<ControllerElementIdentifier>)player.controllers.get_Mouse().get_ElementIdentifiers());
		}
		else if ((int)controllerType == 2)
		{
			for (int i = 0; i < player.controllers.get_Joysticks().Count; i++)
			{
				val.AddRange((ICollection<ControllerElementIdentifier>)player.controllers.get_Joysticks()[i].get_ElementIdentifiers());
			}
		}
		ControllerElementIdentifier val2 = null;
		AxisRange val3 = 0;
		for (int j = 0; j < val.get_Count(); j++)
		{
			if (!IsSplit())
			{
				if (elementName == val.get_Item(j).get_name())
				{
					val2 = val.get_Item(j);
					break;
				}
				continue;
			}
			string empty = string.Empty;
			string empty2 = string.Empty;
			if ((int)val.get_Item(j).get_elementType() == 0)
			{
				empty = val.get_Item(j).get_name() + " +";
				empty2 = val.get_Item(j).get_name() + " -";
			}
			else
			{
				empty = val.get_Item(j).get_name();
				empty2 = val.get_Item(j).get_name();
			}
			if (elementName == empty)
			{
				val2 = val.get_Item(j);
				val3 = 1;
				break;
			}
			if (elementName == empty2)
			{
				val2 = val.get_Item(j);
				val3 = 2;
				break;
			}
		}
		if (val2 != null)
		{
			ElementAssignmentConflictCheck elementAssignmentConflictCheck = default(ElementAssignmentConflictCheck);
			elementAssignmentConflictCheck._002Ector(0, _controllerMap.get_controllerType(), _controllerMap.get_controllerId(), _controllerMap.get_id(), val2.get_elementType(), val2.get_id(), val3, 0, 0, _actionId, _actionAxisContribution, false);
			RemoveConflicts(elementAssignmentConflictCheck);
			_controllerMap.ReplaceOrCreateElementMap(elementAssignmentConflictCheck.ToElementAssignment());
		}
	}

	private void RemoveConflicts(ElementAssignmentConflictCheck elementAssignmentConflictCheck)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (ReInput.get_controllers().conflictChecking.DoesElementAssignmentConflict(elementAssignmentConflictCheck, true, false))
		{
			ReInput.get_controllers().conflictChecking.RemoveElementAssignmentConflicts(elementAssignmentConflictCheck, true, false);
		}
	}

	private ElementAssignmentConflictCheck ToAssignmentConflictCheck(ControllerPollingInfo controllerPollingInfo, ModifierKeyFlags modiferKeyFlags)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		ElementAssignmentConflictCheck result = default(ElementAssignmentConflictCheck);
		result._002Ector(0, controllerPollingInfo.get_controllerType(), controllerPollingInfo.get_controllerId(), _controllerMap.get_id(), controllerPollingInfo.get_elementType(), controllerPollingInfo.get_elementIdentifierId(), GetAxisRange(controllerPollingInfo), controllerPollingInfo.get_keyboardKey(), modiferKeyFlags, _actionId, _actionAxisContribution, false);
		return result;
	}

	private AxisRange GetAxisRange(ControllerPollingInfo pollingInfo)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!pollingInfo.get_success())
		{
			return 1;
		}
		ControllerElementType elementType = pollingInfo.get_elementType();
		Pole axisPole = pollingInfo.get_axisPole();
		AxisRange result = 1;
		if ((int)elementType == 0)
		{
			result = (((int)_actionType != 0) ? (((int)axisPole == 0) ? 1 : 2) : ((!_assignFullAxis) ? (((int)axisPole == 0) ? 1 : 2) : 0));
		}
		return result;
	}
}
