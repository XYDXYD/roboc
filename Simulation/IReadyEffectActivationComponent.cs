using Svelto.ECS;

namespace Simulation
{
	internal interface IReadyEffectActivationComponent
	{
		bool effectActive
		{
			get;
			set;
		}

		DispatchOnChange<bool> activateReadyEffect
		{
			get;
		}
	}
}
