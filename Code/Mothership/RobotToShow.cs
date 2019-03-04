namespace Mothership
{
	internal class RobotToShow
	{
		public byte[] robotData
		{
			get;
			private set;
		}

		public ColorPaletteData palette
		{
			get;
			private set;
		}

		public byte[] colorData
		{
			get;
			private set;
		}

		public string name
		{
			get;
			private set;
		}

		public int robotid
		{
			get;
			private set;
		}

		public uint robotCpu
		{
			get;
			private set;
		}

		public RobotToShow(byte[] robotData, byte[] colorData, string name, int robotID, uint cpu)
		{
			this.robotData = robotData;
			this.colorData = colorData;
			this.name = name;
			robotid = robotID;
			robotCpu = cpu;
		}
	}
}
