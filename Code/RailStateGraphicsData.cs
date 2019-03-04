using Simulation;
using UnityEngine;

internal sealed class RailStateGraphicsData : MonoBehaviour, IRailStateGraphicsData
{
	public GameObject targetingLaserPrefabMe;

	public GameObject targetingLaserPrefabOtherPlayer;

	public GameObject targetingLaserPrefabOtherTeam;

	public float laserLength = 40f;

	public Vector3 drivingSeatOffset = new Vector3(0f, 1f, 0f);

	GameObject IRailStateGraphicsData.targetingLaserPrefab
	{
		get
		{
			return targetingLaserPrefabMe;
		}
	}

	GameObject IRailStateGraphicsData.targetingLaserPrefabAlly
	{
		get
		{
			return targetingLaserPrefabOtherPlayer;
		}
	}

	GameObject IRailStateGraphicsData.targetingLaserPrefabEnemy
	{
		get
		{
			return targetingLaserPrefabOtherTeam;
		}
	}

	float IRailStateGraphicsData.lasersLength
	{
		get
		{
			return laserLength;
		}
	}

	public RailStateGraphicsData()
		: this()
	{
	}//IL_001b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0020: Unknown result type (might be due to invalid IL or missing references)

}
