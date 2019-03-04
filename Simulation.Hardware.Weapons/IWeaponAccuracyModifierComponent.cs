namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponAccuracyModifierComponent
	{
		float totalAccuracy
		{
			get;
			set;
		}

		float crosshairAccuracyModifier
		{
			get;
			set;
		}

		float repeatFiringModifier
		{
			get;
			set;
		}

		float movementAccuracyModifier
		{
			get;
			set;
		}
	}
}
