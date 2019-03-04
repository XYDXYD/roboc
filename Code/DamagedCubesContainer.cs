using System.Collections.Generic;

internal sealed class DamagedCubesContainer
{
	private Dictionary<int, Dictionary<uint, int>> _damagedCubes = new Dictionary<int, Dictionary<uint, int>>();

	public void RegisterCubeDamage(int machineId)
	{
		_damagedCubes.Add(machineId, new Dictionary<uint, int>());
	}

	public void UnregisterCubeDamage(int machineId)
	{
		_damagedCubes.Remove(machineId);
	}

	public bool IsCubeDamageRegistered(int machineId)
	{
		return _damagedCubes.ContainsKey(machineId);
	}

	public void SetCubeDamaged(int machineId, uint gridKey, int damage)
	{
		if (!_damagedCubes[machineId].ContainsKey(gridKey))
		{
			_damagedCubes[machineId].Add(gridKey, damage);
		}
		else
		{
			Dictionary<uint, int> dictionary;
			uint key;
			(dictionary = _damagedCubes[machineId])[key = gridKey] = dictionary[key] + damage;
		}
	}

	public Dictionary<uint, int> GetCubeDamage(int machineId)
	{
		return _damagedCubes[machineId];
	}
}
