using Svelto.ECS;

namespace Simulation.Sight
{
	internal interface ISpottableComponent
	{
		DispatchOnChange<bool> isSpotted
		{
			get;
		}

		float spotLastTimeUpdated
		{
			get;
			set;
		}
	}
}
