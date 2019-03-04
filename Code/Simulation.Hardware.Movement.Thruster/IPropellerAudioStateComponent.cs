using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IPropellerAudioStateComponent
	{
		bool isPlaying
		{
			get;
			set;
		}

		float lastSpinParam
		{
			get;
			set;
		}

		float lastTurnParam
		{
			get;
			set;
		}

		float lastLiftParam
		{
			get;
			set;
		}

		int maxLevelPlaying
		{
			get;
			set;
		}

		float cameraDistance
		{
			get;
			set;
		}

		Vector3 previousForward
		{
			get;
			set;
		}
	}
}
