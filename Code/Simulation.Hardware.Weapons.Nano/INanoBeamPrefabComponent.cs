using UnityEngine;

namespace Simulation.Hardware.Weapons.Nano
{
	internal interface INanoBeamPrefabComponent
	{
		GameObject beamPrefab
		{
			get;
		}

		GameObject beamPrefabEnemy
		{
			get;
		}
	}
}
