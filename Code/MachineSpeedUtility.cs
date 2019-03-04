using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

internal sealed class MachineSpeedUtility
{
	private IDictionary<int, MovementStatsData> _movementStats;

	private float _lerpValue = 1f;

	private float _maxSpeed;

	private float _minSpeed = float.MaxValue;

	private float _measurementSystemMultiplier;

	public MachineSpeedUtility(MovementStats movementStats, bool useDecimalSystem)
	{
		_measurementSystemMultiplier = ((!useDecimalSystem) ? 1f : 1.61f);
		SetMinMaxSpeed(movementStats.data);
		_lerpValue = movementStats.lerpValue;
	}

	public void CalculateSpeed(FasterList<MachineCell> movementParts, Dictionary<ItemCategory, int> partsPerCategory, int affectTopSpeedNodeCount, out int baseSpeed, out float baseSpeedPercent, out float speedBoostPercent)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = float.MaxValue;
		for (int i = 0; i < movementParts.get_Count(); i++)
		{
			MachineCell machineCell = movementParts.get_Item(i);
			InstantiatedCube info = machineCell.info;
			ItemDescriptor itemDescriptor = info.persistentCubeData.itemDescriptor;
			if (!_movementStats.TryGetValue(itemDescriptor.GenerateKey(), out MovementStatsData value))
			{
				continue;
			}
			float cubeTopSpeed = GetCubeTopSpeed(machineCell, itemDescriptor, value);
			bool flag = itemDescriptor.itemCategory != ItemCategory.Thruster && itemDescriptor.itemCategory != ItemCategory.Propeller;
			int cpuRating = (int)info.persistentCubeData.cpuRating;
			if (!(cubeTopSpeed > 0f))
			{
				continue;
			}
			float itemCountModifier = GetItemCountModifier(itemDescriptor.itemCategory, value, partsPerCategory);
			if (flag || affectTopSpeedNodeCount == 0)
			{
				num += (float)cpuRating;
				num2 += cubeTopSpeed * (float)cpuRating * itemCountModifier;
				if (cubeTopSpeed < num4)
				{
					num4 = cubeTopSpeed * itemCountModifier;
				}
			}
			num3 += value.speedBoost;
		}
		if (num != 0f)
		{
			num2 /= num;
			num2 = Mathf.Lerp(num4, num2, _lerpValue);
		}
		num2 *= _measurementSystemMultiplier;
		baseSpeed = Mathf.RoundToInt(num2);
		baseSpeedPercent = num2 / _maxSpeed;
		speedBoostPercent = num3;
	}

	private float GetCubeTopSpeed(MachineCell cell, ItemDescriptor descriptor, MovementStatsData stats)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		if (descriptor.itemCategory == ItemCategory.Thruster)
		{
			Vector3 forward = cell.gameObject.get_transform().get_forward();
			if (forward.z < 0.95f)
			{
				num = 0f;
			}
		}
		else if (descriptor.itemCategory == ItemCategory.Propeller)
		{
			Vector3 up = cell.gameObject.get_transform().get_up();
			if (Mathf.Abs(up.z) < 0.95f)
			{
				num = 0f;
			}
		}
		else if (descriptor.itemCategory == ItemCategory.Wing || descriptor.itemCategory == ItemCategory.Rudder)
		{
			Vector3 up2 = cell.gameObject.get_transform().get_up();
			if (Mathf.Abs(up2.z) > 0.95f)
			{
				num = 0f;
			}
		}
		return num * stats.horizontalTopSpeed;
	}

	private float GetItemCountModifier(ItemCategory itemCategory, MovementStatsData stats, Dictionary<ItemCategory, int> partsPerCategory)
	{
		if (itemCategory == ItemCategory.SprinterLeg || itemCategory == ItemCategory.MechLeg)
		{
			itemCategory = ItemCategory.SprinterLeg;
		}
		if (partsPerCategory.TryGetValue(itemCategory, out int value))
		{
			return (value >= stats.minRequiredItems) ? 1f : stats.minRequiredItemsModifier;
		}
		return 1f;
	}

	private void SetMinMaxSpeed(IDictionary<int, MovementStatsData> statsData)
	{
		_movementStats = statsData;
		IEnumerator<KeyValuePair<int, MovementStatsData>> enumerator = statsData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MovementStatsData value = enumerator.Current.Value;
			if (value.horizontalTopSpeed > _maxSpeed)
			{
				_maxSpeed = value.horizontalTopSpeed;
			}
			if (value.horizontalTopSpeed < _minSpeed)
			{
				_minSpeed = value.horizontalTopSpeed;
			}
		}
		_maxSpeed *= _measurementSystemMultiplier;
		_minSpeed *= _measurementSystemMultiplier;
	}
}
