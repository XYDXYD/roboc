using UnityEngine;

namespace Simulation.BattleArena.CapturePoint
{
	internal interface IPropComponent
	{
		Renderer propRenderer
		{
			get;
		}

		Material[] bluMaterials
		{
			get;
		}

		Material[] redMaterials
		{
			get;
		}
	}
}
