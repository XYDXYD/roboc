using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal class OpsRoomShowTechTreeCTAEntityView : EntityView
	{
		public ITechTreeCTAComponent techTreeCTAComponent;

		public IOpsRoomCTAValuesComponent OpsRoomCTAValuesComponent;

		public OpsRoomShowTechTreeCTAEntityView()
			: this()
		{
		}
	}
}
