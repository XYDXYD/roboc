using Rewired;
using System.Collections.Generic;

internal sealed class InputRemappingVersionValidator
{
	private uint _latestVersion;

	private uint _existingVersion;

	private Dictionary<ActionElementMap, ControllerMap> _elementMaps = new Dictionary<ActionElementMap, ControllerMap>();

	private InputRemappingSaveDataRewired _inputRemappingSaveData;

	public InputRemappingVersionValidator(InputRemappingSaveDataRewired inputRemappingSaveData, uint latestVersion, uint existingVersion)
	{
		_inputRemappingSaveData = inputRemappingSaveData;
		_latestVersion = latestVersion;
		_existingVersion = existingVersion;
	}

	public void UpdateCurrentVersion(int controllerId, ControllerType controllerType)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (_latestVersion == 1 && _existingVersion == 0)
		{
			foreach (KeyValuePair<ActionElementMap, ControllerMap> elementMap in _elementMaps)
			{
				if (elementMap.Key.get_actionId() == 77)
				{
					_inputRemappingSaveData.ReplaceActionBinding(controllerId, controllerType, elementMap.Value, elementMap.Key, 306);
				}
				else if (elementMap.Key.get_actionId() == 35)
				{
					_inputRemappingSaveData.ReplaceActionBinding(controllerId, controllerType, elementMap.Value, elementMap.Key, 99);
				}
			}
		}
	}

	public void Initialize(Player player, int controllerId, ControllerType controllerType)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_elementMaps.Clear();
		IEnumerable<ControllerMap> maps = player.controllers.maps.GetMaps(controllerType, controllerId);
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
						if (val.get_actionId() == 77)
						{
							_elementMaps.Add(val, firstMapInCategory);
						}
						if (val.get_actionId() == 35)
						{
							_elementMaps.Add(val, firstMapInCategory);
						}
					}
				}
			}
		}
	}
}
