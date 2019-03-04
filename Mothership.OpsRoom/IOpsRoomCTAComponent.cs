using UnityEngine;

namespace Mothership.OpsRoom
{
	internal interface IOpsRoomCTAComponent
	{
		GameObject gameObject
		{
			get;
		}

		UILabel label
		{
			get;
		}
	}
}
