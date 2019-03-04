using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal interface IStackDamageComponent
	{
		int currentStackIndex
		{
			get;
			set;
		}

		float buffStackExpireTime
		{
			get;
			set;
		}

		int buffDamagePerStack
		{
			get;
			set;
		}

		int buffMaxStacks
		{
			get;
			set;
		}

		DispatchOnSet<bool> stackableHit
		{
			get;
		}
	}
}
