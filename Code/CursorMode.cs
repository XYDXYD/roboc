using Svelto.Context;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal sealed class CursorMode : ICursorMode, IWaitForFrameworkInitialization
{
	private Stack<Mode> _modeStack;

	private Mode _currentMode = Mode.undefined;

	private bool _forceFree;

	private CursorModeManager _manager;

	public Mode currentMode => _currentMode;

	public event Action<Mode> OnSwitch = delegate
	{
	};

	public CursorMode()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		GameObject val = new GameObject("Cursormode");
		_manager = val.AddComponent<CursorModeManager>();
		_modeStack = new Stack<Mode>();
	}

	void IWaitForFrameworkInitialization.OnFrameworkInitialized()
	{
		SwitchMode(Mode.Lock);
	}

	public void ForceFree()
	{
		PushFreeMode();
		_forceFree = true;
	}

	public void ReleaseForceFree()
	{
		_forceFree = false;
		PopFreeMode();
	}

	public void PushFreeMode()
	{
		_modeStack.Push(_currentMode);
		SwitchMode(Mode.Free);
	}

	public void PopFreeMode()
	{
		if (_modeStack.Count == 0)
		{
			Console.LogWarning("Popping Cursor Free mode more times that it has been pushed");
			SwitchMode(Mode.Lock);
		}
		else
		{
			Mode newMode = _modeStack.Pop();
			SwitchMode(newMode);
		}
	}

	public void PushNoKeyInputMode()
	{
		_modeStack.Push(_currentMode);
		SwitchMode(Mode.LockNoKeyInput);
	}

	public void Reset()
	{
		SwitchMode(Mode.Lock);
		_modeStack.Clear();
	}

	private void SwitchMode(Mode newMode)
	{
		if (_currentMode != newMode && !_forceFree)
		{
			_currentMode = newMode;
			this.OnSwitch(newMode);
			Refresh();
		}
	}

	public void Refresh()
	{
		_manager.lockCursor = (_currentMode == Mode.Lock || _currentMode == Mode.LockNoKeyInput);
	}
}
