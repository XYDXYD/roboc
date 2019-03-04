using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

internal sealed class CubeDamagePropagator
{
	private Queue<InstantiatedCube> _cubesToProcess = new Queue<InstantiatedCube>(100);

	private HashSet<InstantiatedCube> _cubesToIgnore = new HashSet<InstantiatedCube>();

	private HashSet<InstantiatedCube> _alreadyTouchedCubes = new HashSet<InstantiatedCube>();

	private Dictionary<InstantiatedCube, int> _proposedResult = new Dictionary<InstantiatedCube, int>(100);

	private FasterList<int> _shuffledList = new FasterList<int>(12);

	internal int ComputeProposedDamage(InstantiatedCube target, int damage, float protoniumDamageScale, ref Dictionary<InstantiatedCube, int> proposedResult)
	{
		if (target.persistentCubeData.isIndestructible)
		{
			return 0;
		}
		int damageAlreadyDealt = 0;
		_cubesToIgnore.Clear();
		_cubesToProcess.Clear();
		_alreadyTouchedCubes.Clear();
		_cubesToProcess.Enqueue(target);
		_alreadyTouchedCubes.Add(target);
		ComputeDamagePropagation(damage, ref damageAlreadyDealt, ref proposedResult, protoniumDamageScale);
		return damageAlreadyDealt;
	}

	internal Dictionary<InstantiatedCube, int> GetProposedDamage(InstantiatedCube target, float damageBoost, int damageAmount, float damageBuff, float damageMultiplier, float protoniumDamageScale, float campaignDifficultyFactor)
	{
		_proposedResult.Clear();
		int damage = Mathf.CeilToInt(damageBuff * (float)damageAmount * damageMultiplier * damageBoost * campaignDifficultyFactor);
		ComputeProposedDamage(target, damage, protoniumDamageScale, ref _proposedResult);
		return _proposedResult;
	}

	private bool ComputeDamagePropagation(int damage, ref int damageAlreadyDealt, ref Dictionary<InstantiatedCube, int> destroyedTargets, float protoniumDamageScale)
	{
		while (_cubesToProcess.Count > 0)
		{
			if (damageAlreadyDealt >= damage)
			{
				return false;
			}
			InstantiatedCube instantiatedCube = _cubesToProcess.Dequeue();
			if (instantiatedCube.pendingDamage >= instantiatedCube.health || (destroyedTargets.ContainsKey(instantiatedCube) && destroyedTargets[instantiatedCube] <= 0))
			{
				_cubesToIgnore.Add(instantiatedCube);
			}
			if (!_cubesToIgnore.Contains(instantiatedCube))
			{
				AddDamagedCube(instantiatedCube, damage, ref damageAlreadyDealt, ref destroyedTargets, protoniumDamageScale);
				_cubesToIgnore.Add(instantiatedCube);
				if (damageAlreadyDealt >= damage)
				{
					return false;
				}
			}
			FasterList<CubeNodeInstance> neighbours = instantiatedCube.cubeNodeInstance.GetNeighbours();
			_shuffledList.FastClear();
			for (int i = 0; i < neighbours.get_Count(); i++)
			{
				_shuffledList.Add(i);
			}
			_shuffledList.Shuffle<int>();
			for (int j = 0; j < _shuffledList.get_Count(); j++)
			{
				InstantiatedCube instantiatedCube2 = neighbours.get_Item(_shuffledList.get_Item(j)).instantiatedCube;
				if (instantiatedCube2.persistentCubeData.isIndestructible)
				{
					_alreadyTouchedCubes.Add(instantiatedCube2);
				}
				if (!_alreadyTouchedCubes.Contains(instantiatedCube2))
				{
					_alreadyTouchedCubes.Add(instantiatedCube2);
					_cubesToProcess.Enqueue(instantiatedCube2);
				}
			}
		}
		return true;
	}

	internal void GenerateDestructionGroupHitInfo(Dictionary<InstantiatedCube, int> cubes, List<HitCubeInfo> destroyedCubes)
	{
		destroyedCubes.Clear();
		Dictionary<InstantiatedCube, int>.Enumerator enumerator = cubes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<InstantiatedCube, int> current = enumerator.Current;
			HitCubeInfo cubeHitInfo = MegabotDestructionUtilities.GetCubeHitInfo(current.Key, current.Value);
			destroyedCubes.Add(cubeHitInfo);
		}
	}

	private void AddDamagedCube(InstantiatedCube cubeInstance, int damage, ref int damageAlreadyDealt, ref Dictionary<InstantiatedCube, int> destroyedTargets, float protoniumDamageScale)
	{
		int health = cubeInstance.health;
		int damageRemaining = damage - damageAlreadyDealt;
		if (!destroyedTargets.TryGetValue(cubeInstance, out int value))
		{
			value = health;
		}
		int pendingCombinedHealth = value - cubeInstance.pendingDamage;
		int dealtDamage = 0;
		int countedDamage = 0;
		DealPhysicalDamage(pendingCombinedHealth, damageRemaining, ref dealtDamage, ref countedDamage, cubeInstance, protoniumDamageScale);
		damageAlreadyDealt += countedDamage;
		destroyedTargets[cubeInstance] = value - dealtDamage;
	}

	private void DealPhysicalDamage(int pendingCombinedHealth, int damageRemaining, ref int dealtDamage, ref int countedDamage, InstantiatedCube cube, float protoniumDamageScale)
	{
		if (cube.persistentCubeData.protoniumCube && protoniumDamageScale > 0f)
		{
			dealtDamage = Mathf.Min(pendingCombinedHealth, Mathf.CeilToInt((float)damageRemaining * protoniumDamageScale));
			countedDamage = Mathf.CeilToInt((float)dealtDamage / protoniumDamageScale);
		}
		else
		{
			dealtDamage = Mathf.Min(pendingCombinedHealth, damageRemaining);
			countedDamage = dealtDamage;
		}
	}

	public static int GetWeaponDamage(IProjectileDamageStatsComponent stats)
	{
		return Mathf.CeilToInt((float)stats.damage * stats.damageBoost * stats.damageBuff * stats.damageMultiplier * stats.campaignDifficultyFactor);
	}
}
