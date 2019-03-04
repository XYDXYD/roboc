using Svelto.DataStructures;

namespace Services.Analytics
{
	internal class LogRobotShopUploadedDependency
	{
		public uint tier
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

		public LogRobotShopUploadedDependency(uint tier_, uint cpu_, FasterList<ItemCategory> movements_, FasterList<ItemCategory> weapons_)
		{
			tier = tier_;
			cpu = cpu_;
			movements = movements_;
			weapons = weapons_;
		}
	}
}
