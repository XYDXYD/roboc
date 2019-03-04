using Svelto.ECS;

namespace Simulation
{
	internal interface IAliveStateComponent
	{
		DispatchOnChange<bool> isAlive
		{
			get;
		}
	}
}
