using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal interface IColorComponent
	{
		Color teamColor
		{
			get;
		}

		Color enemyColor
		{
			get;
		}
	}
}
