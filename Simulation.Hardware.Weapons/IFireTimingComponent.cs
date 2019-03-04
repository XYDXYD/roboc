using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IFireTimingComponent
	{
		float delayBetweenShots
		{
			get;
			set;
		}

		float[] groupFirePeriod
		{
			get;
			set;
		}

		Dispatcher<IFireTimingComponent, ItemDescriptor> timingsLoaded
		{
			get;
		}
	}
}
