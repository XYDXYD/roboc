using Svelto.ECS;

namespace Simulation.Hardware
{
	internal interface ISpawnableComponent
	{
		DispatchOnSet<bool> isSpawning
		{
			get;
		}
	}
}
