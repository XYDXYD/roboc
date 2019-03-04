using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	public interface IMachineAerofoilAudioComponent
	{
		string audioEvent
		{
			get;
			set;
		}

		bool isAudioPlaying
		{
			get;
			set;
		}

		float audioParamDistance
		{
			get;
			set;
		}

		float audioParamLevel
		{
			get;
			set;
		}

		float audioParamPower
		{
			get;
			set;
		}

		float audioParamLift
		{
			get;
			set;
		}

		Vector3 lastPos
		{
			get;
			set;
		}
	}
}
