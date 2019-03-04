using Svelto.DataStructures;

namespace Services.Analytics
{
	internal class LogRobotShopDownloadedDependency
	{
		public uint cost
		{
			get;
			private set;
		}

		public uint cpu
		{
			get;
			private set;
		}

		public FasterList<ItemCategory> movements
		{
			get;
			private set;
		}

		public FasterList<ItemCategory> weapons
		{
			get;
			private set;
		}

		public LogRobotShopDownloadedDependency(uint cost_, uint cpu_, FasterList<ItemCategory> movements_, FasterList<ItemCategory> weapons_)
		{
			cost = cost_;
			cpu = cpu_;
			movements = movements_;
			weapons = weapons_;
		}
	}
}
