using UnityEngine;

namespace Simulation
{
	internal interface IRailStateGraphicsData
	{
		GameObject targetingLaserPrefab
		{
			get;
		}

		GameObject targetingLaserPrefabAlly
		{
			get;
		}

		GameObject targetingLaserPrefabEnemy
		{
			get;
		}

		float lasersLength
		{
			get;
		}
	}
}
