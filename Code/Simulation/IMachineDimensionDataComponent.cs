using UnityEngine;

namespace Simulation
{
	internal interface IMachineDimensionDataComponent
	{
		Vector3 machineSize
		{
			get;
		}

		Vector3 machineCenter
		{
			get;
		}
	}
}
