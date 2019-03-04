using Svelto.ECS;

namespace Simulation.SpawnEffects
{
	internal class MachinePlaySpawnEffectEntityView : EntityView
	{
		public ISpawnEffectComponent spawnEffectComponent;

		public ISpawnEffectDependenciesComponent spawnEffectDependenciesComponent;

		public MachinePlaySpawnEffectEntityView()
			: this()
		{
		}
	}
}
