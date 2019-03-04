namespace Simulation.Hardware.Movement
{
	internal interface ISpeedModifierComponent
	{
		float speedModifier
		{
			get;
		}

		float minItemsModifier
		{
			get;
			set;
		}
	}
}
