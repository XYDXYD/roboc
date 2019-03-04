using Svelto.ECS;

namespace Mothership.OpsRoom
{
	internal interface IOpsRoomCTAValuesComponent
	{
		DispatchOnSet<int> unspentTP
		{
			get;
		}

		DispatchOnSet<int> newQuests
		{
			get;
		}
	}
}
