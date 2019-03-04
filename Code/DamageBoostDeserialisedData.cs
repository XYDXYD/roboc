using System;
using System.Collections.Generic;
using UnityEngine;

internal class DamageBoostDeserialisedData : ICloneable
{
	private SortedList<uint, float> DamageBoostPerCPU;

	private List<uint> DamageBoostKeys;

	private uint MaxCPU;

	private uint MinCPU;

	public readonly float MaxBoost;

	public readonly float MinBoost;

	private DamageBoostDeserialisedData(DamageBoostDeserialisedData other)
	{
		DamageBoostPerCPU = other.DamageBoostPerCPU;
		DamageBoostKeys = other.DamageBoostKeys;
		MaxCPU = other.MaxCPU;
		MinCPU = other.MinCPU;
		MaxBoost = other.MaxBoost;
		MinBoost = other.MinBoost;
	}

	public DamageBoostDeserialisedData(Dictionary<string, object> damageBoostValues)
	{
		MaxCPU = 0u;
		MinCPU = 1000000u;
		MaxBoost = 0f;
		MinBoost = 10000f;
		DamageBoostPerCPU = new SortedList<uint, float>();
		foreach (KeyValuePair<string, object> damageBoostValue in damageBoostValues)
		{
			uint num = Convert.ToUInt32(damageBoostValue.Key);
			if (num > MaxCPU)
			{
				MaxCPU = num;
			}
			if (num < MinCPU)
			{
				MinCPU = num;
			}
			float num2 = Convert.ToSingle(damageBoostValue.Value);
			if (num2 < MinBoost)
			{
				MinBoost = num2;
			}
			if (num2 > MaxBoost)
			{
				MaxBoost = num2;
			}
			float value = Convert.ToSingle(damageBoostValue.Value);
			DamageBoostPerCPU.Add(num, value);
		}
		DamageBoostKeys = new List<uint>(DamageBoostPerCPU.Keys);
	}

	public object Clone()
	{
		return new DamageBoostDeserialisedData(this);
	}

	public float CalculateNearestBoost(uint cpu)
	{
		if (cpu > MaxCPU)
		{
			cpu = MaxCPU;
		}
		int num = DamageBoostKeys.BinarySearch(cpu);
		if (num < 0)
		{
			int num2 = ~num;
			int index = num2 - 1;
			uint num3 = DamageBoostKeys[num2];
			uint num4 = DamageBoostKeys[index];
			float num5 = Mathf.InverseLerp((float)(double)num4, (float)(double)num3, (float)(double)cpu);
			float num6 = DamageBoostPerCPU[num4];
			float num7 = DamageBoostPerCPU[num3];
			return Mathf.Lerp(num6, num7, num5);
		}
		return DamageBoostPerCPU[cpu];
	}
}
