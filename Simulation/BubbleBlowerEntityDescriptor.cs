using Svelto.ECS;

namespace Simulation
{
	internal class BubbleBlowerEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static BubbleBlowerEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<PlaySoundWhenParticleDestroyedNode>()
			};
		}

		public BubbleBlowerEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
