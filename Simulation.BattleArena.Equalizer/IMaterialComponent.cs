using UnityEngine;

namespace Simulation.BattleArena.Equalizer
{
	internal interface IMaterialComponent
	{
		Material material
		{
			get;
		}

		int healthPropertyId
		{
			get;
		}

		int crackPropertyId
		{
			get;
		}
	}
}
