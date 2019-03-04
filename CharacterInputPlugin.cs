using InputMask;
using Simulation.Hardware.Modules;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class CharacterInputPlugin : IInputPlugin, IComponent
{
	private const float MOUSE_WHEEL_SENSITIVITY = 0.1f;

	private BSMode _bsMode = BSMode.HideBattleStats;

	private Dictionary<string, int> _battleStatsInputsList;

	private Dictionary<string, int> _cursorInputsList;

	private Dictionary<string, int> _defaultInputsList;

	private Dictionary<string, int> _enabledInputs;

	private Dictionary<string, int> _mapInputsList;

	private Dictionary<string, int> _stunnedDefaultInputsList;

	private Dictionary<string, int> _stunnedMapInputList;

	private InputCharacterData _inputCharacterData = new InputCharacterData();

	private MMode _mapMode = MMode.HideMap;

	private Mode _cursorMode;

	private bool _machineStunned;

	private int _zoomInSteps;

	private int _zoomOutSteps;

	[Inject]
	internal IInputActionMask inputActionMask
	{
		private get;
		set;
	}

	private event Action<InputCharacterData> OnCharacterInput = delegate
	{
	};

	public CharacterInputPlugin(ICursorMode cursorMode)
	{
		cursorMode.OnSwitch += OnCursorModeChange;
		SetDefaultInputsList();
		SetCursorInputsList();
		SetMapInputsList();
		SetBattleStatsInputsList();
		SetStunnedInputList();
		SetStunnedMapInputList();
		_enabledInputs = _defaultInputsList;
	}

	public void Execute()
	{
		_inputCharacterData.Reset();
		Dictionary<string, int>.Enumerator enumerator = _enabledInputs.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, int> current = enumerator.Current;
			CharacterInputAxis value = (CharacterInputAxis)current.Value;
			float num = 0f;
			num = ((current.Value != 11) ? InputRemapper.Instance.GetAxis(current.Key) : ((!InputRemapper.Instance.GetButtonDown("Taunt")) ? 0f : 1f));
			if (num != 0f)
			{
				if (value == CharacterInputAxis.VERTICAL || value == CharacterInputAxis.HORIZONTAL)
				{
					num = Mathf.Clamp(num, -1f, 1f);
				}
				_inputCharacterData.Set(value, num);
			}
		}
		_inputCharacterData.Set(CharacterInputAxis.TAUNT, (!InputRemapper.Instance.GetButtonDown("Taunt")) ? 0f : 1f);
		this.OnCharacterInput(_inputCharacterData);
	}

	public void OnCursorModeChange(Mode cursorMode)
	{
		_cursorMode = cursorMode;
		switch (cursorMode)
		{
		case Mode.Lock:
			if (!_machineStunned)
			{
				_enabledInputs = _defaultInputsList;
			}
			else
			{
				_enabledInputs = _stunnedDefaultInputsList;
			}
			break;
		case Mode.LockNoKeyInput:
			_enabledInputs = _cursorInputsList;
			break;
		default:
			if (_mapMode != 0 && _bsMode != 0)
			{
				_enabledInputs = new Dictionary<string, int>();
			}
			break;
		}
	}

	public void RegisterComponent(IInputComponent component)
	{
		if (component is IHandleCharacterInput)
		{
			IHandleCharacterInput obj = component as IHandleCharacterInput;
			OnCharacterInput += obj.HandleCharacterInput;
		}
		else if (component is IMapModeComponent)
		{
			(component as IMapModeComponent).OnSwitch += OnMapModChange;
		}
		else if (component is IBattleStatsModeComponent)
		{
			(component as IBattleStatsModeComponent).OnSwitch += OnBSModeChange;
		}
		else if (component is EmpModuleInputDisablingComponent)
		{
			(component as EmpModuleInputDisablingComponent).machineStunned += HandleMachineStunned;
		}
	}

	public void UnregisterComponent(IInputComponent component)
	{
		if (component is IHandleCharacterInput)
		{
			IHandleCharacterInput obj = component as IHandleCharacterInput;
			OnCharacterInput -= obj.HandleCharacterInput;
		}
		else if (component is IMapModeComponent)
		{
			(component as IMapModeComponent).OnSwitch -= OnMapModChange;
		}
		else if (component is IBattleStatsModeComponent)
		{
			(component as IBattleStatsModeComponent).OnSwitch -= OnBSModeChange;
		}
		else if (component is EmpModuleInputDisablingComponent)
		{
			(component as EmpModuleInputDisablingComponent).machineStunned -= HandleMachineStunned;
		}
	}

	private void SetDefaultInputsList()
	{
		_defaultInputsList = new Dictionary<string, int>();
		_defaultInputsList.Add("MouseX", 4);
		_defaultInputsList.Add("MouseY", 5);
		_defaultInputsList.Add("Horizontal", 0);
		_defaultInputsList.Add("Vertical", 1);
		_defaultInputsList.Add("Jump", 2);
		_defaultInputsList.Add("Crouch", 3);
		_defaultInputsList.Add("PulseAR", 9);
		_defaultInputsList.Add("Taunt", 11);
		_defaultInputsList.Add("Fire 1", 6);
		_defaultInputsList.Add("Fire 2", 7);
		_defaultInputsList.Add("ZoomCamera", 8);
		_defaultInputsList.Add("Zoom In", 8);
		_defaultInputsList.Add("Zoom Out", 8);
		_defaultInputsList.Add("Pick Cube", 13);
		_defaultInputsList.Add("Map Ping", 15);
		_defaultInputsList.Add("ShowScore", 14);
		_defaultInputsList.Add("AcceptSurrender", 19);
		_defaultInputsList.Add("DeclineSurrender", 20);
		_defaultInputsList.Add("SelectSlot1", 23);
		_defaultInputsList.Add("SelectSlot2", 24);
		_defaultInputsList.Add("SelectSlot3", 25);
		_defaultInputsList.Add("SelectSlot4", 26);
		_defaultInputsList.Add("SelectSlot5", 27);
		_defaultInputsList.Add("StrafeLeft", 28);
		_defaultInputsList.Add("StrafeRight", 29);
	}

	private void SetCursorInputsList()
	{
		_cursorInputsList = new Dictionary<string, int>();
		_cursorInputsList.Add("MouseX", 4);
		_cursorInputsList.Add("MouseY", 5);
	}

	private void SetMapInputsList()
	{
		_mapInputsList = new Dictionary<string, int>();
		_mapInputsList.Add("Horizontal", 0);
		_mapInputsList.Add("Vertical", 1);
		_mapInputsList.Add("Jump", 2);
		_mapInputsList.Add("Crouch", 3);
		_mapInputsList.Add("Map Ping", 15);
		_mapInputsList.Add("MapPingDanger", 16);
		_mapInputsList.Add("MapPingGoingHere", 18);
		_mapInputsList.Add("MapPingMoveHere", 17);
	}

	private void SetBattleStatsInputsList()
	{
		_battleStatsInputsList = new Dictionary<string, int>();
		_battleStatsInputsList.Add("MouseX", 4);
		_battleStatsInputsList.Add("MouseY", 5);
		_battleStatsInputsList.Add("Horizontal", 0);
		_battleStatsInputsList.Add("Vertical", 1);
		_battleStatsInputsList.Add("Jump", 2);
		_battleStatsInputsList.Add("Crouch", 3);
		_battleStatsInputsList.Add("PulseAR", 9);
		_battleStatsInputsList.Add("ZoomCamera", 8);
		_battleStatsInputsList.Add("Zoom In", 8);
		_battleStatsInputsList.Add("Zoom Out", 8);
		_battleStatsInputsList.Add("Pick Cube", 13);
		_battleStatsInputsList.Add("ShowScore", 14);
		_battleStatsInputsList.Add("AcceptSurrender", 19);
		_battleStatsInputsList.Add("DeclineSurrender", 20);
	}

	private void SetStunnedInputList()
	{
		_stunnedDefaultInputsList = new Dictionary<string, int>();
		_stunnedDefaultInputsList.Add("MouseX", 4);
		_stunnedDefaultInputsList.Add("MouseY", 5);
		_stunnedDefaultInputsList.Add("ZoomCamera", 8);
		_stunnedDefaultInputsList.Add("Zoom In", 8);
		_stunnedDefaultInputsList.Add("Zoom Out", 8);
		_stunnedDefaultInputsList.Add("Map Ping", 15);
		_stunnedDefaultInputsList.Add("ShowScore", 14);
		_stunnedDefaultInputsList.Add("AcceptSurrender", 19);
		_stunnedDefaultInputsList.Add("DeclineSurrender", 20);
	}

	private void SetStunnedMapInputList()
	{
		_stunnedMapInputList = new Dictionary<string, int>();
		_stunnedMapInputList.Add("MouseX", 4);
		_stunnedMapInputList.Add("MouseY", 5);
		_stunnedMapInputList.Add("ZoomCamera", 8);
		_stunnedMapInputList.Add("Zoom In", 8);
		_stunnedMapInputList.Add("Zoom Out", 8);
		_stunnedMapInputList.Add("Map Ping", 15);
		_stunnedMapInputList.Add("AcceptSurrender", 19);
		_stunnedMapInputList.Add("DeclineSurrender", 20);
		_stunnedMapInputList.Add("MapPingDanger", 16);
		_stunnedMapInputList.Add("MapPingGoingHere", 18);
		_stunnedMapInputList.Add("MapPingMoveHere", 17);
	}

	private void OnMapModChange(MMode mapMode)
	{
		_mapMode = mapMode;
		switch (mapMode)
		{
		case MMode.ShowMap:
			if (!_machineStunned)
			{
				_enabledInputs = _mapInputsList;
			}
			else
			{
				_enabledInputs = _stunnedMapInputList;
			}
			break;
		case MMode.HideMap:
			if (_cursorMode != Mode.Free)
			{
				if (!_machineStunned)
				{
					_enabledInputs = _defaultInputsList;
				}
				else
				{
					_enabledInputs = _stunnedDefaultInputsList;
				}
			}
			break;
		}
	}

	private void OnBSModeChange(BSMode bsMode)
	{
		_bsMode = bsMode;
		switch (bsMode)
		{
		case BSMode.ShowBattleStats:
			if (!_machineStunned)
			{
				_enabledInputs = _battleStatsInputsList;
			}
			else
			{
				_enabledInputs = _stunnedDefaultInputsList;
			}
			break;
		case BSMode.HideBattleStats:
			if (_cursorMode != Mode.Free)
			{
				if (!_machineStunned)
				{
					_enabledInputs = _defaultInputsList;
				}
				else
				{
					_enabledInputs = _stunnedDefaultInputsList;
				}
			}
			break;
		}
	}

	private void HandleMachineStunned(bool stunned)
	{
		_machineStunned = stunned;
		OnMapModChange(_mapMode);
		OnBSModeChange(_bsMode);
		OnCursorModeChange(_cursorMode);
	}
}
