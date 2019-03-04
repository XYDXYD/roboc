using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IFireOrderComponent
	{
		Dispatcher<int> nextElegibleWeaponToFire
		{
			get;
		}
	}
}
