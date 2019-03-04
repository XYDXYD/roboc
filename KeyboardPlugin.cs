using Svelto.ES.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class KeyboardPlugin : IInputPlugin, IComponent
{
	private Array _keyValues;

	private Dictionary<KeyCode, bool> _keys;

	private event Action<Dictionary<KeyCode, bool>> OnKeyDown = delegate
	{
	};

	public KeyboardPlugin()
	{
		_keyValues = Enum.GetValues(typeof(KeyCode));
	}

	public void RegisterComponent(IInputComponent component)
	{
		if (component is IHandleKeyboard)
		{
			IHandleKeyboard obj = component as IHandleKeyboard;
			OnKeyDown += obj.HandleKeyboard;
		}
	}

	public void UnregisterComponent(IInputComponent component)
	{
		if (component is IHandleKeyboard)
		{
			IHandleKeyboard obj = component as IHandleKeyboard;
			OnKeyDown -= obj.HandleKeyboard;
		}
	}

	public void Execute()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Input.get_anyKeyDown())
		{
			for (int i = 0; i < _keyValues.Length; i++)
			{
				KeyCode val = (KeyCode)_keyValues.GetValue(i);
				if (Input.GetKeyDown(val))
				{
					_keys.Add(val, value: true);
				}
			}
		}
		this.OnKeyDown(_keys);
	}
}
