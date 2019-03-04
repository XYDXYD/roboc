using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal class OpsRoomDisplayEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static OpsRoomDisplayEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<OpsRoomDisplayEntityView>(),
				new EntityViewBuilder<OpsRoomShowTechTreeCTAEntityView>(),
				new EntityViewBuilder<OpsRoomShowQuestsCTAEntityView>(),
				new EntityViewBuilder<OpsRoomShowOpsRoomCTAEntityView>()
			};
		}

		public OpsRoomDisplayEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
