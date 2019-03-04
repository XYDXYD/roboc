namespace Simulation.Hardware.Movement
{
	internal interface IStrafingCustomAngleToStraightComponent
	{
		bool customAngleUsed
		{
			get;
			set;
		}

		float angleToStraight
		{
			get;
			set;
		}
	}
}
