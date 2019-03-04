using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilMachineImplementor : IMachineAerofoilComponent, IMachineAerofoilAudioComponent
	{
		public float rudderSpeed
		{
			get;
			set;
		}

		public float rudderSpeedHeavy
		{
			get;
			set;
		}

		public Vector3 lateralForceOffset
		{
			get;
			set;
		}

		public float bankSpeed
		{
			get;
			set;
		}

		public float bankSpeedHeavy
		{
			get;
			set;
		}

		public float barrelSpeed
		{
			get;
			set;
		}

		public float barrelSpeedHeavy
		{
			get;
			set;
		}

		public float elevationSpeed
		{
			get;
			set;
		}

		public float elevationSpeedHeavy
		{
			get;
			set;
		}

		public float stopperCapacityRatio
		{
			get;
			set;
		}

		public float thrust
		{
			get;
			set;
		}

		public float thrustHeavy
		{
			get;
			set;
		}

		public float vtolVelocity
		{
			get;
			set;
		}

		public float vtolVelocityHeavy
		{
			get;
			set;
		}

		public float liftCapacityRatio
		{
			get;
			set;
		}

		public float lateralCapacityRatio
		{
			get;
			set;
		}

		public float barrelCapacityRatio
		{
			get;
			set;
		}

		public float bankCapacityRatio
		{
			get;
			set;
		}

		public float rudderCapacityRatio
		{
			get;
			set;
		}

		public float speedRatio
		{
			get;
			set;
		}

		public float barrelOffset
		{
			get;
			set;
		}

		public float elevationOffset
		{
			get;
			set;
		}

		public float[] aDragParams
		{
			get;
			set;
		}

		public bool updateRequired
		{
			get;
			set;
		}

		public int numAerofoils
		{
			get;
			set;
		}

		public Vector3 aerofoilInput
		{
			get;
			set;
		}

		public string audioEvent
		{
			get;
			set;
		}

		public bool isAudioPlaying
		{
			get;
			set;
		}

		public float audioParamDistance
		{
			get;
			set;
		}

		public float audioParamLevel
		{
			get;
			set;
		}

		public float audioParamPower
		{
			get;
			set;
		}

		public float audioParamLift
		{
			get;
			set;
		}

		public Vector3 lastPos
		{
			get;
			set;
		}

		public AerofoilMachineImplementor(bool isLocal)
		{
			audioEvent = ((!isLocal) ? "Aerofoils_Timeline_Enemy" : "Aerofoils_Timeline");
		}
	}
}
