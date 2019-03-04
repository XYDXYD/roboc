using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IMisfireComponent
	{
		Dispatcher<IMisfireComponent, int> weaponMisfired
		{
			get;
		}

		float coolDownPenalty
		{
			get;
			set;
		}

		int misfireDebuffMaxStacks
		{
			get;
			set;
		}

		float misfireDebuffDuration
		{
			get;
			set;
		}
	}
}
