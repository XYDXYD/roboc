using Svelto.ECS;

namespace EnginesGUI
{
	internal class RetargetableParticleEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RetargetableParticleEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<RetargetableParticleNode>()
			};
		}

		public RetargetableParticleEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
