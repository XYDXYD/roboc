namespace Simulation.Hardware.Movement
{
	internal interface IMaxSpeedStatsComponent
	{
		float horizontalMaxSpeed
		{
			get;
			set;
		}

		float verticalMaxSpeed
		{
			get;
			set;
		}

		float speedBoost
		{
			get;
			set;
		}

		int minRequiredItems
		{
			get;
			set;
		}

		float minRequiredItemsModifier
		{
			get;
			set;
		}
	}
}
