using System.Collections.Generic;

namespace InputMask
{
	internal class InputActionMaskTutorial : IInputActionMask
	{
		private bool _rejectAllInputByDefault;

		private Dictionary<EditingInputAxis, bool> _editingInputAxisMask;

		private Dictionary<BuildModeInputActions, bool> _buildModeInputAxis;

		private Dictionary<GUIShortcutInputAxis, bool> _guishortcutInputActionsMask;

		private Dictionary<WorldSwitchingInputAxis, bool> _worldSwitchingInputAxisMask;

		private Dictionary<SimulationInputAction, bool> _simulationInputAxisMask;

		public InputActionMaskTutorial()
		{
			_editingInputAxisMask = new Dictionary<EditingInputAxis, bool>();
			_guishortcutInputActionsMask = new Dictionary<GUIShortcutInputAxis, bool>();
			_buildModeInputAxis = new Dictionary<BuildModeInputActions, bool>();
			_worldSwitchingInputAxisMask = new Dictionary<WorldSwitchingInputAxis, bool>();
			_simulationInputAxisMask = new Dictionary<SimulationInputAction, bool>();
		}

		public bool InputIsAvailable(UserInputCategory category, int axis)
		{
			switch (category)
			{
			case UserInputCategory.EditingInputAxis:
				return FilterInputByInputAxis((EditingInputAxis)axis, _editingInputAxisMask);
			case UserInputCategory.BuildModeInputAxis:
				return FilterInputByInputAxis((BuildModeInputActions)axis, _buildModeInputAxis);
			case UserInputCategory.GUIShortcutInputAxis:
				return FilterInputByInputAxis((GUIShortcutInputAxis)axis, _guishortcutInputActionsMask);
			case UserInputCategory.WorldSwitchingInputAxis:
				return FilterInputByInputAxis((WorldSwitchingInputAxis)axis, _worldSwitchingInputAxisMask);
			case UserInputCategory.SimulationInputAxis:
				return FilterInputByInputAxis((SimulationInputAction)axis, _simulationInputAxisMask);
			default:
				return true;
			}
		}

		private bool FilterInputByInputAxis<T>(T axis, Dictionary<T, bool> dictionary)
		{
			if (_rejectAllInputByDefault && !dictionary.ContainsKey(axis))
			{
				return false;
			}
			if (dictionary.ContainsKey(axis))
			{
				return dictionary[axis];
			}
			return true;
		}

		public void RejectInputAction(UserInputCategory category, int axis)
		{
			switch (category)
			{
			case UserInputCategory.EditingInputAxis:
				_editingInputAxisMask[(EditingInputAxis)axis] = false;
				break;
			case UserInputCategory.BuildModeInputAxis:
				_buildModeInputAxis[(BuildModeInputActions)axis] = false;
				break;
			case UserInputCategory.GUIShortcutInputAxis:
				_guishortcutInputActionsMask[(GUIShortcutInputAxis)axis] = false;
				break;
			case UserInputCategory.WorldSwitchingInputAxis:
				_worldSwitchingInputAxisMask[(WorldSwitchingInputAxis)axis] = false;
				break;
			case UserInputCategory.SimulationInputAxis:
				_simulationInputAxisMask[(SimulationInputAction)axis] = false;
				break;
			}
		}

		public void AcceptInputAction(UserInputCategory category, int axis)
		{
			switch (category)
			{
			case UserInputCategory.EditingInputAxis:
				_editingInputAxisMask[(EditingInputAxis)axis] = true;
				break;
			case UserInputCategory.BuildModeInputAxis:
				_buildModeInputAxis[(BuildModeInputActions)axis] = true;
				break;
			case UserInputCategory.GUIShortcutInputAxis:
				_guishortcutInputActionsMask[(GUIShortcutInputAxis)axis] = true;
				break;
			case UserInputCategory.WorldSwitchingInputAxis:
				_worldSwitchingInputAxisMask[(WorldSwitchingInputAxis)axis] = true;
				break;
			case UserInputCategory.SimulationInputAxis:
				_simulationInputAxisMask[(SimulationInputAction)axis] = true;
				break;
			}
		}

		public void RejectAllInputByDefault(bool setting)
		{
			_rejectAllInputByDefault = setting;
		}
	}
}
