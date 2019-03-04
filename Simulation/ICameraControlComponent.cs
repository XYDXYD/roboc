using UnityEngine;

namespace Simulation
{
	internal interface ICameraControlComponent
	{
		bool controlScriptEnabled
		{
			get;
			set;
		}

		bool instantFollowEnabled
		{
			get;
			set;
		}

		bool activateProgressiveFollow
		{
			get;
			set;
		}

		float cameraTime
		{
			get;
			set;
		}

		Vector3 lastCameraPosition
		{
			get;
		}

		Vector3 finalExpectedCameraPosition
		{
			set;
		}
	}
}
