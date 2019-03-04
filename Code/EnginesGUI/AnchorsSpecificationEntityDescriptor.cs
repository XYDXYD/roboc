using Svelto.ECS;

namespace EnginesGUI
{
	internal class AnchorsSpecificationEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static AnchorsSpecificationEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<AnchorsManipulationNode>()
			};
		}

		public AnchorsSpecificationEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
