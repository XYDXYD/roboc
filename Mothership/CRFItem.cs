using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class CRFItem
	{
		public RobotShopItem robotShopItem
		{
			get;
			private set;
		}

		public bool isMyRobot
		{
			get;
			set;
		}

		public bool isMegabot
		{
			get;
			set;
		}

		public string TierStr
		{
			get;
			set;
		}

		public uint robotCPUToPlayer
		{
			get;
			set;
		}

		public RobotShopCommunityItemView view
		{
			get;
			set;
		}

		public Texture2D thumbnail
		{
			get;
			set;
		}

		public bool playerOwnAllCubes => LockedCubes.Count == 0;

		public bool isExpired => (robotShopItem.submissionExpiryDate - DateTime.UtcNow).TotalMilliseconds < 0.0;

		public List<uint> LockedCubes
		{
			get;
			set;
		}

		public CRFItem(RobotShopItem robotShopItem)
		{
			this.robotShopItem = robotShopItem;
			isMyRobot = false;
			view = null;
			thumbnail = null;
		}
	}
}
