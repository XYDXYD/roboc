using System;

namespace Mothership
{
	internal class RobotShopTransaction
	{
		public CRFItem communityItem
		{
			get;
			private set;
		}

		public DateTime expiryDate
		{
			get;
			set;
		}

		public RobotShopTransaction(CRFItem _item, DateTime _expiryDate = default(DateTime))
		{
			communityItem = _item;
			expiryDate = _expiryDate;
		}
	}
}
