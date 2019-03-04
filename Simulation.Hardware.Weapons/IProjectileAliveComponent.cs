using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileAliveComponent
	{
		bool justFired
		{
			get;
			set;
		}

		bool active
		{
			get;
			set;
		}

		Dispatcher<IProjectileAliveComponent, int> resetProjectile
		{
			get;
		}
	}
}
