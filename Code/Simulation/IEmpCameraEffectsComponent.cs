using UnityEngine;

namespace Simulation
{
	internal interface IEmpCameraEffectsComponent
	{
		bool enableEffectsScripts
		{
			get;
			set;
		}

		GameObject cameraStunSoundObject
		{
			get;
		}
	}
}
