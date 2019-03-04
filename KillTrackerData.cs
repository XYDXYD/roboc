using UnityEngine;

internal sealed class KillTrackerData : MonoBehaviour
{
	public float multiplayerRewardBonus = 2f;

	public int maxKillCount = 100;

	public int rewardMaxKillCount = 25;

	public KillTrackerData()
		: this()
	{
	}
}
