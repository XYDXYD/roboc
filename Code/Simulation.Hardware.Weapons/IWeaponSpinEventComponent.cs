using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	public interface IWeaponSpinEventComponent
	{
		Dispatcher<int> spinStarted
		{
			get;
		}

		Dispatcher<int> spinStopped
		{
			get;
		}
	}
}
