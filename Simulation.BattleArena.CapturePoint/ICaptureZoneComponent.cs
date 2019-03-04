using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal interface ICaptureZoneComponent
	{
		Renderer sphereRenderer
		{
			get;
		}

		Renderer plateFxRenderer
		{
			get;
		}
	}
}
