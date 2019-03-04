namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface ISteeringComponent
	{
		bool steerable
		{
			get;
			set;
		}

		float maxSteeringReduction
		{
			get;
			set;
		}

		float maxSteeringAngle
		{
			get;
			set;
		}

		float maxSteeringReductionKeyboard
		{
			get;
		}

		float maxSteeringAngleKeyboard
		{
			get;
		}

		float steeringSpeed
		{
			get;
		}

		float currentSteeringAngle
		{
			get;
			set;
		}

		bool steeringStraight
		{
			get;
			set;
		}

		float maxSteeringMultiplier
		{
			get;
			set;
		}

		float steeringForceMultiplier
		{
			get;
			set;
		}
	}
}
