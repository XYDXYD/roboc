using Rewired;
using Svelto.DataStructures;
using System.Collections.Generic;

internal class ButtonRemapHelperRewired
{
	public FasterList<string> GetMouseAxisNames()
	{
		Player player = ReInput.get_players().GetPlayer(0);
		FasterList<string> val = new FasterList<string>();
		for (int i = 0; i < player.controllers.get_Mouse().get_AxisElementIdentifiers().Count; i++)
		{
			val.Add(StringTableBase<StringTable>.Instance.GetMouseJoystickString(player.controllers.get_Mouse().get_AxisElementIdentifiers()[i].get_name()));
		}
		return val;
	}

	public FasterList<string> GetMouseElementNamesSplitAxes()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Player player = ReInput.get_players().GetPlayer(0);
		FasterList<string> val = new FasterList<string>();
		for (int i = 0; i < player.controllers.get_Mouse().get_ElementIdentifiers().Count; i++)
		{
			string mouseJoystickString = StringTableBase<StringTable>.Instance.GetMouseJoystickString(player.controllers.get_Mouse().get_ElementIdentifiers()[i].get_name());
			if ((int)player.controllers.get_Mouse().get_ElementIdentifiers()[i].get_elementType() == 0)
			{
				val.Add($"{mouseJoystickString} +");
				val.Add($"{mouseJoystickString} -");
			}
			else
			{
				val.Add(mouseJoystickString);
			}
		}
		return val;
	}

	public Dictionary<string, string> GenerateMouseStringsToAxisName()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Player player = ReInput.get_players().GetPlayer(0);
		string empty = string.Empty;
		for (int i = 0; i < player.controllers.get_Mouse().get_AxisElementIdentifiers().Count; i++)
		{
			empty = player.controllers.get_Mouse().get_AxisElementIdentifiers()[i].get_name();
			dictionary.Add(StringTableBase<StringTable>.Instance.GetMouseJoystickString(empty), empty);
		}
		for (int j = 0; j < player.controllers.get_Mouse().get_ElementIdentifiers().Count; j++)
		{
			empty = player.controllers.get_Mouse().get_ElementIdentifiers()[j].get_name();
			string mouseJoystickString = StringTableBase<StringTable>.Instance.GetMouseJoystickString(empty);
			if ((int)player.controllers.get_Mouse().get_ElementIdentifiers()[j].get_elementType() == 0)
			{
				dictionary.Add($"{mouseJoystickString} +", $"{empty} +");
				dictionary.Add($"{mouseJoystickString} -", $"{empty} -");
			}
			else
			{
				dictionary.Add(mouseJoystickString, empty);
			}
		}
		return dictionary;
	}

	public FasterList<string> GetJoypadAxisNames()
	{
		Player player = ReInput.get_players().GetPlayer(0);
		FasterList<string> val = new FasterList<string>();
		for (int i = 0; i < player.controllers.get_Joysticks().Count; i++)
		{
			for (int j = 0; j < player.controllers.get_Joysticks()[i].get_AxisElementIdentifiers().Count; j++)
			{
				val.Add(player.controllers.get_Joysticks()[i].get_AxisElementIdentifiers()[j].get_name());
			}
		}
		return val;
	}

	public FasterList<string> GetJoypadElementNamesSplitAxes()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Player player = ReInput.get_players().GetPlayer(0);
		FasterList<string> val = new FasterList<string>();
		for (int i = 0; i < player.controllers.get_Joysticks().Count; i++)
		{
			for (int j = 0; j < player.controllers.get_Joysticks()[i].get_ElementIdentifiers().Count; j++)
			{
				if ((int)player.controllers.get_Joysticks()[i].get_ElementIdentifiers()[j].get_elementType() == 0)
				{
					val.Add(player.controllers.get_Joysticks()[i].get_ElementIdentifiers()[j].get_name() + " +");
					val.Add(player.controllers.get_Joysticks()[i].get_ElementIdentifiers()[j].get_name() + " -");
				}
				else
				{
					val.Add(player.controllers.get_Joysticks()[i].get_ElementIdentifiers()[j].get_name());
				}
			}
		}
		return val;
	}

	public Dictionary<string, string> GenerateJoystickStringsToAxisName()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Player player = ReInput.get_players().GetPlayer(0);
		string empty = string.Empty;
		for (int i = 0; i < player.controllers.get_Joysticks().Count; i++)
		{
			for (int j = 0; j < player.controllers.get_Joysticks()[i].get_AxisElementIdentifiers().Count; j++)
			{
				empty = player.controllers.get_Joysticks()[i].get_AxisElementIdentifiers()[j].get_name();
				dictionary.Add(StringTableBase<StringTable>.Instance.GetMouseJoystickString(empty), empty);
			}
			for (int k = 0; k < player.controllers.get_Joysticks()[i].get_ElementIdentifiers().Count; k++)
			{
				empty = player.controllers.get_Joysticks()[i].get_ElementIdentifiers()[k].get_name() + " +";
				string mouseJoystickString = StringTableBase<StringTable>.Instance.GetMouseJoystickString(empty);
				if ((int)player.controllers.get_Joysticks()[i].get_ElementIdentifiers()[k].get_elementType() == 0)
				{
					dictionary.Add($"{mouseJoystickString} +", $"{empty} +");
					dictionary.Add($"{mouseJoystickString} -", $"{empty} -");
				}
				else
				{
					dictionary.Add(mouseJoystickString, empty);
				}
			}
		}
		return dictionary;
	}

	public Dictionary<int, ButtonActionMap> GenerateButtonActionMaps(bool isShopAvailable)
	{
		IList<InputCategory> categories = InputRemapper.Instance.GetCategories();
		Dictionary<int, ButtonActionMap> dictionary = new Dictionary<int, ButtonActionMap>(categories.Count);
		foreach (InputCategory item in categories)
		{
			if (item.get_userAssignable())
			{
				ButtonActionMap value = GenerateButtonMapForCategory(item, isShopAvailable);
				dictionary.Add(item.get_id(), value);
			}
		}
		return dictionary;
	}

	private ButtonActionMap GenerateButtonMapForCategory(InputCategory category, bool isShopAvailable)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Invalid comparison between Unknown and I4
		ButtonActionMap buttonActionMap = new ButtonActionMap();
		buttonActionMap.catecoryId = category.get_id();
		buttonActionMap.categoryName = category.get_descriptiveName();
		IEnumerable<InputAction> actionsInCategory = InputRemapper.Instance.GetActionsInCategory(category.get_id());
		ControllerMap mapForType = GetMapForType(0, category.get_name());
		ControllerMap mapForType2 = GetMapForType(1, category.get_name());
		ControllerMap mapForType3 = GetMapForType(2, category.get_name());
		foreach (InputAction item in actionsInCategory)
		{
			if (item.get_userAssignable() && ButtonMappingUsedChecker.IsMappingUsed(item.get_name(), isShopAvailable))
			{
				bool flag = (int)item.get_type() == 0;
				if (flag && item.get_negativeDescriptiveName() != string.Empty && item.get_positiveDescriptiveName() != string.Empty)
				{
					SetupActionButtonsSplit(ref buttonActionMap, mapForType, mapForType2, mapForType3, item, 0);
					SetupActionButtonsSplit(ref buttonActionMap, mapForType, mapForType2, mapForType3, item, 1);
				}
				else
				{
					SetupActionButtons(ref buttonActionMap, mapForType, mapForType2, mapForType3, item, flag);
				}
			}
		}
		return buttonActionMap;
	}

	private ControllerMap GetMapForType(ControllerType controllerType, string categoryName)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Player player = ReInput.get_players().GetPlayer(0);
		return player.controllers.maps.GetFirstMapInCategory(controllerType, 0, categoryName);
	}

	private void SetupActionButtons(ref ButtonActionMap buttonActionMap, ControllerMap keyboardMap, ControllerMap mouseMap, ControllerMap joypadMap, InputAction action, bool fullAxis)
	{
		ButtonReassignData value = AddButtonAsignmentData(keyboardMap, action, fullAxis);
		ButtonReassignData value2 = AddButtonAsignmentData(mouseMap, action, fullAxis);
		ButtonReassignData value3 = AddButtonAsignmentData(joypadMap, action, fullAxis);
		buttonActionMap.map.Add(buttonActionMap.map.Count, value);
		buttonActionMap.map.Add(buttonActionMap.map.Count, value2);
		buttonActionMap.map.Add(buttonActionMap.map.Count, value3);
	}

	private void SetupActionButtonsSplit(ref ButtonActionMap buttonActionMap, ControllerMap keyboardMap, ControllerMap mouseMap, ControllerMap joypadMap, InputAction action, Pole pole)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ButtonReassignData value = AddButtonAssignmentDataSplit(keyboardMap, action, pole);
		ButtonReassignData value2 = AddButtonAssignmentDataSplit(mouseMap, action, pole);
		ButtonReassignData value3 = AddButtonAssignmentDataSplit(joypadMap, action, pole);
		buttonActionMap.map.Add(buttonActionMap.map.Count, value);
		buttonActionMap.map.Add(buttonActionMap.map.Count, value2);
		buttonActionMap.map.Add(buttonActionMap.map.Count, value3);
	}

	private ButtonReassignData AddButtonAsignmentData(ControllerMap controllerMap, InputAction action, bool fullAxis)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<ActionElementMap> buttonsForAction = null;
		if (controllerMap != null)
		{
			buttonsForAction = controllerMap.GetElementMapsWithAction(action.get_id());
		}
		return new ButtonReassignData(action.get_id(), action.get_descriptiveName(), 0, buttonsForAction, controllerMap, action.get_type(), fullAxis);
	}

	private ButtonReassignData AddButtonAssignmentDataSplit(ControllerMap controllerMap, InputAction action, Pole pole)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<ActionElementMap> enumerable = null;
		if (controllerMap != null)
		{
			enumerable = controllerMap.GetElementMapsWithAction(action.get_id());
		}
		FasterList<ActionElementMap> val = new FasterList<ActionElementMap>();
		string actionName = ((int)pole != 0) ? action.get_negativeDescriptiveName() : action.get_positiveDescriptiveName();
		if (enumerable != null)
		{
			foreach (ActionElementMap item in enumerable)
			{
				if (item.get_axisContribution() == pole)
				{
					val.Add(item);
				}
			}
		}
		return new ButtonReassignData(action.get_id(), actionName, pole, (IEnumerable<ActionElementMap>)val, controllerMap, action.get_type(), assignFullAxis: false);
	}

	public static ControllerType GetRewiredType(InputAssignmentButton.InputType controllerType)
	{
		switch (controllerType)
		{
		case InputAssignmentButton.InputType.Keyboard:
			return 0;
		case InputAssignmentButton.InputType.Mouse:
			return 1;
		case InputAssignmentButton.InputType.Joypad:
			return 2;
		default:
			return 0;
		}
	}
}
