using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal class OpsRoomDisplayEntityView : EntityView
	{
		public IOpsRoomDisplayComponent displayComponent;

		public OpsRoomDisplayEntityView()
			: this()
		{
		}
	}
}
