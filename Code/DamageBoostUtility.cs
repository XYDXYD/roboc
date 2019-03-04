using UnityEngine;

internal sealed class DamageBoostUtility
{
	private DamageBoostDeserialisedData _damageBoostDisplayData;

	public DamageBoostUtility(DamageBoostDeserialisedData damageBoostData)
	{
		_damageBoostDisplayData = damageBoostData;
	}

	public float CurrentRobotDamageBoostPercentage(uint totalCPU)
	{
		float num = _damageBoostDisplayData.CalculateNearestBoost(totalCPU);
		float maxBoost = _damageBoostDisplayData.MaxBoost;
		float minBoost = _damageBoostDisplayData.MinBoost;
		return Mathf.InverseLerp(minBoost, maxBoost, num);
	}
}
