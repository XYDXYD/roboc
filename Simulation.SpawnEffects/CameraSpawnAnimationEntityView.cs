using Svelto.ECS;

namespace Simulation.SpawnEffects
{
	internal class CameraSpawnAnimationEntityView : EntityView
	{
		public ISpawnAnimationComponent spawnAnimationComponent;

		public CameraSpawnAnimationEntityView()
			: this()
		{
		}
	}
}
