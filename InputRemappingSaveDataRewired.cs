using Authentication;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Utility;

internal class InputRemappingSaveDataRewired : IInputRemappingSaveData
{
	private const string SAVE_FILE_BASE_NAME = "_ControlRemap";

	public const uint CURRENT_VERSION = 2u;

	private uint _version;

	private InputRemappingVersionValidator _versiopnValidator;

	private string GetFileName()
	{
		return Application.get_persistentDataPath() + "\\" + User.Username + "_ControlRemap";
	}

	public void Save()
	{
		string fileName = GetFileName();
		Serialise()?.Save(fileName);
	}

	public void Load()
	{
		string fileName = GetFileName();
		if (File.Exists(fileName))
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(fileName);
			Deserialise(xmlDocument);
		}
	}

	public bool HasSavedSettings()
	{
		string fileName = GetFileName();
		return File.Exists(fileName);
	}

	public void Revert()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Player player = ReInput.get_players().GetPlayer(0);
		IEnumerator enumerator = Enum.GetValues(typeof(ControllerType)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ControllerType val = (ControllerType)enumerator.Current;
				player.controllers.maps.LoadDefaultMaps(val);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void Delete()
	{
		string fileName = GetFileName();
		if (File.Exists(fileName))
		{
			File.Delete(fileName);
		}
	}

	private XmlDocument Serialise()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("root");
			XmlNode xmlNode2 = xmlDocument.CreateElement("version");
			xmlNode2.InnerText = 2u.ToString();
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = xmlDocument.CreateElement("players");
			IList<Player> allPlayers = ReInput.get_players().get_AllPlayers();
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player val = allPlayers[i];
				XmlNode xmlNode4 = xmlDocument.CreateElement("player");
				string basePlayerPrefsKey = GetBasePlayerPrefsKey(val);
				XmlNode xmlNode5 = xmlDocument.CreateElement("playerId");
				xmlNode5.InnerText = basePlayerPrefsKey;
				xmlNode4.AppendChild(xmlNode5);
				PlayerSaveData saveData = val.GetSaveData(true);
				XmlNode xmlNode6 = xmlDocument.CreateElement("controllerMaps");
				foreach (ControllerMapSaveData allControllerMapSaveDatum in saveData.get_AllControllerMapSaveData())
				{
					XmlNode xmlNode7 = xmlDocument.CreateElement("controllerMap");
					string controllerMapPlayerPrefsKey = GetControllerMapPlayerPrefsKey(val, allControllerMapSaveDatum);
					string value = allControllerMapSaveDatum.get_map().ToXmlString();
					AddKeyAndValueToXmlNode(xmlDocument, xmlNode7, controllerMapPlayerPrefsKey, value);
					xmlNode6.AppendChild(xmlNode7);
				}
				xmlNode4.AppendChild(xmlNode6);
				xmlNode3.AppendChild(xmlNode4);
			}
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode8 = xmlDocument.CreateElement("joysticks");
			foreach (Joystick joystick in ReInput.get_controllers().get_Joysticks())
			{
				XmlNode xmlNode9 = xmlDocument.CreateElement("joystick");
				JoystickCalibrationMapSaveData calibrationMapSaveData = joystick.GetCalibrationMapSaveData();
				string joystickCalibrationMapPlayerPrefsKey = GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData);
				string value2 = calibrationMapSaveData.get_map().ToXmlString();
				AddKeyAndValueToXmlNode(xmlDocument, xmlNode9, joystickCalibrationMapPlayerPrefsKey, value2);
				xmlNode8.AppendChild(xmlNode9);
			}
			xmlNode.AppendChild(xmlNode8);
			xmlDocument.AppendChild(xmlNode);
			return xmlDocument;
		}
		catch (Exception ex)
		{
			Console.LogException(ex);
			return null;
		}
	}

	private static void AddKeyAndValueToXmlNode(XmlDocument xmlDoc, XmlNode inputBehaviourNode, string key, string value)
	{
		XmlNode xmlNode = xmlDoc.CreateElement("key");
		xmlNode.InnerText = key;
		inputBehaviourNode.AppendChild(xmlNode);
		XmlNode xmlNode2 = xmlDoc.CreateElement("value");
		xmlNode2.InnerText = value;
		inputBehaviourNode.AppendChild(xmlNode2);
	}

	private void Deserialise(XmlDocument xmlDoc)
	{
		try
		{
			IList<Player> allPlayers = ReInput.get_players().get_AllPlayers();
			XmlNode xmlNode = xmlDoc.SelectSingleNode("root");
			_version = 0u;
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("version");
			if (xmlNode2 != null)
			{
				uint.TryParse(xmlNode2.InnerText, out _version);
			}
			_versiopnValidator = new InputRemappingVersionValidator(this, 2u, _version);
			XmlNode xmlNode3 = xmlNode.SelectSingleNode("players");
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player val = allPlayers[i];
				if (!(val.get_name() == "Default") && !(val.get_name() == "Temp"))
				{
					string basePlayerPrefsKey = GetBasePlayerPrefsKey(val);
					XmlNode xmlNode4 = xmlNode3.SelectSingleNode("descendant::player[playerId='" + basePlayerPrefsKey + "']");
					if (xmlNode4 != null)
					{
						XmlNode controllerMaps = xmlNode4.SelectSingleNode("controllerMaps");
						List<string> allControllerMapsXml = GetAllControllerMapsXml(val, userAssignableMapsOnly: true, 0, ReInput.get_controllers().get_Keyboard(), controllerMaps);
						List<string> allControllerMapsXml2 = GetAllControllerMapsXml(val, userAssignableMapsOnly: true, 1, ReInput.get_controllers().get_Mouse(), controllerMaps);
						List<List<string>> list = new List<List<string>>();
						foreach (Joystick joystick in val.controllers.get_Joysticks())
						{
							List<string> allControllerMapsXml3 = GetAllControllerMapsXml(val, userAssignableMapsOnly: true, 2, joystick, controllerMaps);
							list.Add(allControllerMapsXml3);
						}
						AddLoadedElementMapsToPlayer(val, 0, 0, allControllerMapsXml);
						int num = 0;
						foreach (Joystick joystick2 in val.controllers.get_Joysticks())
						{
							AddLoadedElementMapsToPlayer(val, joystick2.id, 2, list[num]);
							num++;
						}
						AddLoadedElementMapsToPlayer(val, 0, 1, allControllerMapsXml2);
					}
				}
			}
			XmlNode parentNode = xmlNode.SelectSingleNode("joysticks");
			foreach (Joystick joystick3 in ReInput.get_controllers().get_Joysticks())
			{
				string joystickCalibrationMapXmlKey = GetJoystickCalibrationMapXmlKey(joystick3);
				string valueFromKey = GetValueFromKey(parentNode, "joystick", joystickCalibrationMapXmlKey);
				joystick3.ImportCalibrationMapFromXmlString(valueFromKey);
			}
		}
		catch (Exception ex)
		{
			Console.LogException(ex);
		}
	}

	private void AddLoadedElementMapsToPlayer(Player player, int controllerId, ControllerType controllerType, List<string> loadedMapsXml)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		Player player2 = ReInput.get_players().GetPlayer("Temp");
		_versiopnValidator.Initialize(player, controllerId, controllerType);
		player2.controllers.maps.ClearMaps(controllerType, true);
		player2.controllers.maps.AddMapsFromXml(controllerType, controllerId, loadedMapsXml);
		IEnumerable<ControllerMap> maps = player2.controllers.maps.GetMaps(controllerType, controllerId);
		foreach (ControllerMap item in maps)
		{
			ControllerMap firstMapInCategory = player.controllers.maps.GetFirstMapInCategory(controllerType, controllerId, item.get_categoryId());
			if (firstMapInCategory != null)
			{
				ActionElementMap[] elementMaps = item.GetElementMaps();
				if (elementMaps != null)
				{
					ActionElementMap[] array = elementMaps;
					foreach (ActionElementMap val in array)
					{
						InputAction action = ReInput.get_mapping().GetAction(val.get_actionId());
						if (action != null && action.get_userAssignable())
						{
							ReplaceElementMap(controllerId, controllerType, firstMapInCategory, val);
						}
					}
				}
			}
		}
		_versiopnValidator.UpdateCurrentVersion(controllerId, controllerType);
		player2.controllers.maps.ClearAllMaps(true);
	}

	public void ReplaceElementMap(int controllerId, ControllerType controllerType, ControllerMap currentControllerMap, ActionElementMap loadedElementMap)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		ElementAssignmentConflictCheck elementAssignmentConflictCheck = default(ElementAssignmentConflictCheck);
		elementAssignmentConflictCheck._002Ector(0, controllerType, controllerId, currentControllerMap.get_id(), loadedElementMap.get_elementType(), loadedElementMap.get_elementIdentifierId(), loadedElementMap.get_axisRange(), loadedElementMap.get_keyCode(), loadedElementMap.get_modifierKeyFlags(), loadedElementMap.get_actionId(), loadedElementMap.get_axisContribution(), loadedElementMap.get_invert());
		DeleteExistingElementMapForAction(loadedElementMap, currentControllerMap);
		DisassociateInputFromOtherActions(elementAssignmentConflictCheck);
		currentControllerMap.ReplaceOrCreateElementMap(elementAssignmentConflictCheck.ToElementAssignment());
	}

	public void ReplaceActionBinding(int controllerId, ControllerType controllerType, ControllerMap currentControllerMap, ActionElementMap loadedElementMap, KeyCode keyCode)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		ElementAssignmentConflictCheck elementAssignmentConflictCheck = default(ElementAssignmentConflictCheck);
		elementAssignmentConflictCheck._002Ector(0, controllerType, controllerId, currentControllerMap.get_id(), loadedElementMap.get_elementType(), loadedElementMap.get_elementIdentifierId(), loadedElementMap.get_axisRange(), keyCode, loadedElementMap.get_modifierKeyFlags(), loadedElementMap.get_actionId(), loadedElementMap.get_axisContribution(), loadedElementMap.get_invert());
		DeleteExistingElementMapForAction(loadedElementMap, currentControllerMap);
		DisassociateInputFromOtherActions(elementAssignmentConflictCheck);
		currentControllerMap.ReplaceOrCreateElementMap(elementAssignmentConflictCheck.ToElementAssignment());
	}

	private void DeleteExistingElementMapForAction(ActionElementMap loadedElementMap, ControllerMap controllerMap)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ActionElementMap[] elementMapsWithAction = controllerMap.GetElementMapsWithAction(loadedElementMap.get_actionId());
		if (elementMapsWithAction == null)
		{
			return;
		}
		ActionElementMap[] array = elementMapsWithAction;
		foreach (ActionElementMap val in array)
		{
			if (val.get_axisContribution() == loadedElementMap.get_axisContribution())
			{
				controllerMap.DeleteElementMap(val.get_id());
			}
		}
	}

	private void DisassociateInputFromOtherActions(ElementAssignmentConflictCheck elementAssignmentConflictCheck)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (ReInput.get_controllers().conflictChecking.DoesElementAssignmentConflict(elementAssignmentConflictCheck, true, false))
		{
			ReInput.get_controllers().conflictChecking.RemoveElementAssignmentConflicts(elementAssignmentConflictCheck, true, false);
		}
	}

	private static string GetValueFromKey(XmlNode parentNode, string nodeName, string key)
	{
		string xpath = "descendant::" + nodeName + "[key='" + key + "']";
		XmlNode xmlNode = parentNode.SelectSingleNode(xpath);
		if (xmlNode == null)
		{
			return string.Empty;
		}
		XmlNode xmlNode2 = xmlNode.SelectSingleNode("value");
		return xmlNode2.InnerText;
	}

	private string GetInputBehaviorPlayerPrefsKey(Player player, InputBehavior saveData)
	{
		string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
		basePlayerPrefsKey += "+dataType:InputBehavior";
		return basePlayerPrefsKey + "+id:" + saveData.get_id();
	}

	private string GetControllerMapPlayerPrefsKey(Player player, ControllerMapSaveData saveData)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
		basePlayerPrefsKey += "+dataType:ControllerMap";
		basePlayerPrefsKey = basePlayerPrefsKey + "+controllerMapType:" + saveData.get_mapTypeString();
		string text = basePlayerPrefsKey;
		basePlayerPrefsKey = text + "+categoryId:" + saveData.get_map().get_categoryId() + "+layoutId:" + saveData.get_map().get_layoutId();
		basePlayerPrefsKey = basePlayerPrefsKey + "+hardwareIdentifier:" + saveData.get_controllerHardwareIdentifier();
		if (saveData.get_mapType() == typeof(JoystickMap))
		{
			basePlayerPrefsKey = basePlayerPrefsKey + "+hardwareGuid:" + saveData.get_joystickHardwareTypeGuid().ToString();
		}
		return basePlayerPrefsKey;
	}

	private string GetJoystickCalibrationMapPlayerPrefsKey(JoystickCalibrationMapSaveData saveData)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		empty += "+dataType:CalibrationMap";
		string str = empty;
		ControllerType controllerType = saveData.get_controllerType();
		empty = str + "+controllerType:" + ((object)controllerType).ToString();
		empty = empty + "+hardwareIdentifier:" + saveData.get_hardwareIdentifier();
		return empty + "+hardwareGuid:" + saveData.get_joystickHardwareTypeGuid().ToString();
	}

	private List<string> GetAllControllerMapsXml(Player player, bool userAssignableMapsOnly, ControllerType controllerType, Controller controller, XmlNode controllerMaps)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		IList<InputMapCategory> mapCategories = ReInput.get_mapping().get_MapCategories();
		for (int i = 0; i < mapCategories.Count; i++)
		{
			InputMapCategory val = mapCategories[i];
			if (userAssignableMapsOnly && !val.get_userAssignable())
			{
				continue;
			}
			IList<InputLayout> list2 = ReInput.get_mapping().MapLayouts(controllerType);
			for (int j = 0; j < list2.Count; j++)
			{
				InputLayout val2 = list2[j];
				string controllerMapXmlKey = GetControllerMapXmlKey(player, controllerType, val.get_id(), val2.get_id(), controller);
				string valueFromKey = GetValueFromKey(controllerMaps, "controllerMap", controllerMapXmlKey);
				if (!(valueFromKey == string.Empty))
				{
					list.Add(valueFromKey);
				}
			}
		}
		return list;
	}

	private string GetControllerMapXmlKey(Player player, ControllerType controllerType, int categoryId, int layoutId, Controller controller)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Invalid comparison between Unknown and I4
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
		basePlayerPrefsKey += "+dataType:ControllerMap";
		basePlayerPrefsKey = basePlayerPrefsKey + "+controllerMapType:" + controller.get_mapTypeString();
		string text = basePlayerPrefsKey;
		basePlayerPrefsKey = text + "+categoryId:" + categoryId + "+layoutId:" + layoutId;
		basePlayerPrefsKey = basePlayerPrefsKey + "+hardwareIdentifier:" + controller.get_hardwareIdentifier();
		if ((int)controllerType == 2)
		{
			Joystick val = controller;
			basePlayerPrefsKey = basePlayerPrefsKey + "+hardwareGuid:" + val.get_hardwareTypeGuid().ToString();
		}
		return basePlayerPrefsKey;
	}

	private string GetJoystickCalibrationMapXmlKey(Joystick joystick)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		empty += "+dataType:CalibrationMap";
		string str = empty;
		ControllerType type = joystick.get_type();
		empty = str + "+controllerType:" + ((object)type).ToString();
		empty = empty + "+hardwareIdentifier:" + joystick.get_hardwareIdentifier();
		return empty + "+hardwareGuid:" + joystick.get_hardwareTypeGuid().ToString();
	}

	private string GetBasePlayerPrefsKey(Player player)
	{
		return "+playerName:" + player.get_name();
	}
}
