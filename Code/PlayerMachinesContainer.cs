using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System.Collections.Generic;
using Utility;

internal sealed class PlayerMachinesContainer
{
	public const int INVALID_PLAYER_ID = -1;

	public const int INVALID_MACHINE_ID = -1;

	public const int DEFAULT_ROBOT_TYPE = -1;

	public const int LANDER_ROBOT_TYPE = -2;

	public const int SWITCHABLE_ROBOT_MIN_ID = 1000;

	public const int DEFAULT_MIN_ID = 0;

	public const int LANDER_MIN_ID = 500;

	private Dictionary<TargetType, Dictionary<int, int>> _machinePlayer = new Dictionary<TargetType, Dictionary<int, int>>();

	private Dictionary<TargetType, Dictionary<int, int>> _playerMachines = new Dictionary<TargetType, Dictionary<int, int>>();

	private Dictionary<TargetType, FasterList<int>> _allMachines = new Dictionary<TargetType, FasterList<int>>(400);

	public void RegisterPlayerMachine(TargetType type, int playerId, int machineId)
	{
		if (!_machinePlayer.TryGetValue(type, out Dictionary<int, int> value))
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			_machinePlayer[type] = dictionary;
			value = dictionary;
			_playerMachines[type] = new Dictionary<int, int>();
		}
		Dictionary<int, int> dictionary2 = _playerMachines[type];
		value[machineId] = playerId;
		if (playerId != -1)
		{
			dictionary2[playerId] = machineId;
		}
		if (!_allMachines.ContainsKey(type))
		{
			_allMachines[type] = new FasterList<int>();
		}
		_allMachines[type].Add(machineId);
	}

	public void SetPlayerMachine(TargetType type, int playerId, int machineId)
	{
		_machinePlayer[type][machineId] = playerId;
		if (playerId != -1)
		{
			_playerMachines[type][playerId] = machineId;
		}
	}

	public void UnregisterPlayerMachine(TargetType type, int playerId)
	{
		Dictionary<int, int> dictionary = _playerMachines[type];
		if (dictionary.TryGetValue(playerId, out int value))
		{
			dictionary.Remove(playerId);
			_machinePlayer[type].Remove(value);
			_allMachines[type].Remove(value);
		}
		else
		{
			Console.LogError("Machine " + value + " not found for unregister");
		}
	}

	public bool IsMachineRegistered(TargetType type, int machineId)
	{
		if (_machinePlayer.TryGetValue(type, out Dictionary<int, int> value))
		{
			return value.ContainsKey(machineId);
		}
		return false;
	}

	public bool HasPlayerRegisteredMachine(TargetType type, int playerId)
	{
		if (_playerMachines.TryGetValue(type, out Dictionary<int, int> value))
		{
			return value.ContainsKey(playerId);
		}
		return false;
	}

	public int GetPlayerFromMachineId(TargetType type, int machineId)
	{
		if (!_machinePlayer[type].ContainsKey(machineId))
		{
			return -1;
		}
		return _machinePlayer[type][machineId];
	}

	public int GetActiveMachine(TargetType type, int playerId)
	{
		if (_playerMachines.TryGetValue(type, out Dictionary<int, int> value) && value.TryGetValue(playerId, out int value2))
		{
			return value2;
		}
		return -1;
	}

	public static int GetPrimaryMachineId(int playerId)
	{
		return playerId;
	}
}
