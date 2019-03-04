using Svelto.ECS;

namespace Mothership
{
	internal class PurchaseRequestEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static PurchaseRequestEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<ExecutePurchaseEntityView>()
			};
		}

		public PurchaseRequestEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
