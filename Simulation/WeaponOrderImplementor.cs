using Services.Simulation;

namespace Simulation
{
	internal class WeaponOrderImplementor : IWeaponOrderComponent
	{
		public WeaponOrderSimulation weaponOrder
		{
			get;
			private set;
		}

		public WeaponOrderImplementor(WeaponOrderSimulation order)
		{
			weaponOrder = order;
		}
	}
}
