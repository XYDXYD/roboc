using InputMask;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class EditingInputPlugin : IInputPlugin, IComponent
{
	private const float SEC_BEFORE_ENABLE_CONTINUOUS_ACTION = 0.75f;

	private const int INVALID_AXIS_ENUM_INDEX = -1;

	private Dictionary<string, int> _editingSimpleInputsList;

	private InputEditingData _inputEditingData = new InputEditingData();

	private float _timePassedSinceButtonPressed;

	[Inject]
	internal IInputActionMask inputActionMask
	{
		private get;
		set;
	}

	private event Action OnInputWhileEditing = delegate
	{
	};

	private event Action<InputEditingData> OnUpdateWhileEditing = delegate
	{
	};

	public EditingInputPlugin()
	{
		SetSimpleEditingInputsList();
	}

	public void RegisterComponent(IInputComponent component)
	{
		if (component is IHandleEditingInput)
		{
			IHandleEditingInput obj = component as IHandleEditingInput;
			OnUpdateWhileEditing += obj.HandleEditingInput;
		}
		if (component is IHandleWholeEditingInput)
		{
			IHandleWholeEditingInput obj2 = component as IHandleWholeEditingInput;
			OnInputWhileEditing += obj2.HandleInputWhileInBuildMode;
		}
	}

	public void UnregisterComponent(IInputComponent component)
	{
		if (component is IHandleEditingInput)
		{
			IHandleEditingInput obj = component as IHandleEditingInput;
			OnUpdateWhileEditing -= obj.HandleEditingInput;
		}
		if (component is IHandleWholeEditingInput)
		{
			IHandleWholeEditingInput obj2 = component as IHandleWholeEditingInput;
			OnInputWhileEditing -= obj2.HandleInputWhileInBuildMode;
		}
	}

	public void Execute()
	{
		_inputEditingData.Reset();
		HandleVariableEditingInput();
		Dictionary<string, int>.Enumerator enumerator = _editingSimpleInputsList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, int> current = enumerator.Current;
			string key = current.Key;
			int value = current.Value;
			if (!InputRemapper.Instance.GetButtonDown(key))
			{
				continue;
			}
			float axis = InputRemapper.Instance.GetAxis(key);
			if (value != -1)
			{
				if (!inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, value))
				{
					_inputEditingData.Set((EditingInputAxis)value, 0f);
				}
				else
				{
					_inputEditingData.Set((EditingInputAxis)value, 1f * Mathf.Sign(axis));
				}
			}
			this.OnInputWhileEditing();
		}
		this.OnUpdateWhileEditing(_inputEditingData);
	}

	private void SetSimpleEditingInputsList()
	{
		_editingSimpleInputsList = new Dictionary<string, int>(22);
		_editingSimpleInputsList.Add("Cube Manipulator", 8);
		_editingSimpleInputsList.Add("Mirror Build Mode", 2);
		_editingSimpleInputsList.Add("Paint Applicator", 9);
		_editingSimpleInputsList.Add("Centre Robot", 10);
		_editingSimpleInputsList.Add("Quit", 1);
		_editingSimpleInputsList.Add("Buy Cosmetic Credits", 12);
		_editingSimpleInputsList.Add("Show COM", 13);
		_editingSimpleInputsList.Add("Premium", 11);
		_editingSimpleInputsList.Add("Undo", 14);
		_editingSimpleInputsList.Add("HideHudBuild", 15);
		_editingSimpleInputsList.Add("Move Mirror Line Left", -1);
		_editingSimpleInputsList.Add("Move Robot Back", -1);
		_editingSimpleInputsList.Add("Move Robot Down", -1);
		_editingSimpleInputsList.Add("Move Robot Forward", -1);
		_editingSimpleInputsList.Add("Move Robot Left", -1);
		_editingSimpleInputsList.Add("Move Robot Right", -1);
		_editingSimpleInputsList.Add("Move Robot Up", -1);
		_editingSimpleInputsList.Add("Place Cube", -1);
		_editingSimpleInputsList.Add("Remove Cube", -1);
		_editingSimpleInputsList.Add("Rotate Cube", -1);
	}

	private void HandleVariableEditingInput()
	{
		if (InputRemapper.Instance.GetButtonUp("Rotate Cube"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 0))
			{
				_inputEditingData.Set(EditingInputAxis.ROTATE_CUBE, 1f);
			}
		}
		else if (!InputRemapper.Instance.GetButton("Rotate Cube") && inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 0))
		{
			float axis = InputRemapper.Instance.GetAxis("Rotate Cube");
			_inputEditingData.Set(EditingInputAxis.ROTATE_CUBE, axis);
		}
		if (InputRemapper.Instance.GetButtonUp("Move Mirror Line Left"))
		{
			_inputEditingData.Set(EditingInputAxis.MOVE_MIRROR_LINE, -1f);
		}
		else if (InputRemapper.Instance.GetButtonUp("Move Mirror Line Right"))
		{
			_inputEditingData.Set(EditingInputAxis.MOVE_MIRROR_LINE, 1f);
		}
		if (InputRemapper.Instance.GetButtonUp("Move Robot Right"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 4))
			{
				_inputEditingData.Set(EditingInputAxis.NUDGE_ROBOT_X, 1f);
			}
		}
		else if (InputRemapper.Instance.GetButtonUp("Move Robot Left") && inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 4))
		{
			_inputEditingData.Set(EditingInputAxis.NUDGE_ROBOT_X, -1f);
		}
		if (InputRemapper.Instance.GetButtonUp("Move Robot Up"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 5))
			{
				_inputEditingData.Set(EditingInputAxis.NUDGE_ROBOT_Y, 1f);
			}
		}
		else if (InputRemapper.Instance.GetButtonUp("Move Robot Down") && inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 5))
		{
			_inputEditingData.Set(EditingInputAxis.NUDGE_ROBOT_Y, -1f);
		}
		if (InputRemapper.Instance.GetButtonUp("Move Robot Forward"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 6))
			{
				_inputEditingData.Set(EditingInputAxis.NUDGE_ROBOT_Z, 1f);
			}
		}
		else if (InputRemapper.Instance.GetButtonUp("Move Robot Back") && inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 6))
		{
			_inputEditingData.Set(EditingInputAxis.NUDGE_ROBOT_Z, -1f);
		}
		if (InputRemapper.Instance.GetButtonDown("Show COM") && inputActionMask.InputIsAvailable(UserInputCategory.EditingInputAxis, 13))
		{
			_inputEditingData.Set(EditingInputAxis.TOGGLE_CENTER_OF_MASS, 1f);
		}
		if (InputRemapper.Instance.GetButton("Undo"))
		{
			if (_timePassedSinceButtonPressed > 0.75f)
			{
				_inputEditingData.Set(EditingInputAxis.UNDO_LAST_ACTION, 1f);
			}
			else
			{
				_timePassedSinceButtonPressed += Time.get_deltaTime();
			}
		}
		else if (InputRemapper.Instance.GetButtonUp("Undo"))
		{
			_timePassedSinceButtonPressed = 0f;
		}
	}
}
