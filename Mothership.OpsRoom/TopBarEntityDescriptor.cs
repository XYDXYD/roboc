using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal class TopBarEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TopBarEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<TopBarShowOpsRoomCTAEntityView>()
			};
		}

		public TopBarEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
