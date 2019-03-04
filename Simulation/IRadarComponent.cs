using Svelto.ECS;

namespace Simulation
{
	internal interface IRadarComponent
	{
		DispatchOnChange<bool> isRadarActive
		{
			get;
		}

		float radarRemainingTime
		{
			get;
			set;
		}
	}
}
