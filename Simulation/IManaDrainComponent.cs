using Svelto.ECS.Legacy;

namespace Simulation
{
	internal interface IManaDrainComponent
	{
		bool draining
		{
			get;
			set;
		}

		float drainRate
		{
			get;
			set;
		}

		Dispatcher<IManaDrainComponent, ManaDrainingActivationData> activateManaDraining
		{
			get;
		}

		Dispatcher<IManaDrainComponent, int> manaDrained
		{
			get;
		}
	}
}
