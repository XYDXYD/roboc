using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class SkinnedMeshNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IRendererComponent rendererComponent;

		public IBonesComponent bonesComponent;

		public SkinnedMeshNode()
			: this()
		{
		}
	}
}
