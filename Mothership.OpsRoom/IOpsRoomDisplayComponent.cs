using Svelto.ECS;
using UnityEngine;

namespace Mothership.OpsRoom
{
	public interface IOpsRoomDisplayComponent
	{
		GameObject gameObject
		{
			get;
		}

		GameObject techTreeNotificationGO
		{
			get;
		}

		UILabel techTreeNotificationLabel
		{
			get;
		}

		DispatchOnChange<bool> techTreeClicked
		{
			get;
			set;
		}

		DispatchOnChange<bool> missionClicked
		{
			get;
			set;
		}

		DispatchOnChange<bool> tierRanksClicked
		{
			get;
			set;
		}

		void Show();

		void Hide();
	}
}
