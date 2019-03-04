using UnityEngine;

namespace Simulation.Sight
{
	internal interface ISpotterComponent
	{
		Vector3 spotPositionWorld
		{
			get;
		}

		float spotRange
		{
			get;
		}

		float innerSpotRange
		{
			get;
		}
	}
}
