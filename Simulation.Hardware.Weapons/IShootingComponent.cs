using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IShootingComponent
	{
		bool justFired
		{
			get;
			set;
		}

		float lastFireTime
		{
			get;
			set;
		}

		Dispatcher<IShootingComponent, int> shotIsReadyToFire
		{
			get;
		}

		Dispatcher<IShootingComponent, int> shotIsGoingToBeFired
		{
			get;
		}

		Dispatcher<IShootingComponent> shotCantBeFiredDueToLackOfMana
		{
			get;
		}
	}
}
