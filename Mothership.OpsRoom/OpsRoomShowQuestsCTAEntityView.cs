using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal class OpsRoomShowQuestsCTAEntityView : EntityView
	{
		public IQuestsCTAComponent questsCTAComponent;

		public IOpsRoomCTAValuesComponent opsRoomCTAValuesComponent;

		public OpsRoomShowQuestsCTAEntityView()
			: this()
		{
		}
	}
}
