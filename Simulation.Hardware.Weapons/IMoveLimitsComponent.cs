namespace Simulation.Hardware.Weapons
{
	internal interface IMoveLimitsComponent
	{
		float minHorizAngle
		{
			get;
		}

		float maxHorizAngle
		{
			get;
		}

		float minVerticalAngle
		{
			get;
		}

		float maxVerticalAngle
		{
			get;
		}

		float secondVerticalJointAngle
		{
			get;
		}

		bool enableWorldSpaceLimit
		{
			get;
		}

		float maxVerticalAngleWorld
		{
			get;
		}

		float minVerticalAngleWorld
		{
			get;
		}

		float verticalAngleOffset
		{
			get;
		}

		float verticalAngleMultiplier
		{
			get;
		}
	}
}
