using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal class OpsRoomCTAValuesImplementor : IOpsRoomCTAValuesComponent
	{
		public DispatchOnSet<int> unspentTP
		{
			get;
			private set;
		}

		public DispatchOnSet<int> newQuests
		{
			get;
			private set;
		}

		public OpsRoomCTAValuesImplementor(int entityId)
		{
			unspentTP = new DispatchOnSet<int>(entityId);
			newQuests = new DispatchOnSet<int>(entityId);
		}
	}
}
