using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class SpawnableImplementor : ISpawnableComponent
	{
		public DispatchOnSet<bool> isSpawning
		{
			get;
			private set;
		}

		public SpawnableImplementor(int entityId)
		{
			isSpawning = new DispatchOnSet<bool>(entityId);
		}
	}
}
