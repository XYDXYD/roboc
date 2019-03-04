using UnityEngine;

namespace Simulation
{
	internal interface ICloakAudioObjectsComponent
	{
		GameObject soundObjectActive
		{
			get;
		}

		GameObject soundObjectInactive
		{
			get;
		}
	}
}
