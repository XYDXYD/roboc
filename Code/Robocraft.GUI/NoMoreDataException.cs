using System;

namespace Robocraft.GUI
{
	internal class NoMoreDataException : Exception
	{
		public NoMoreDataException(int dataIndex1, int dataIndex2, string originatingDataSource)
			: base("could not provide data at index:" + dataIndex1.ToString() + ", " + dataIndex2.ToString() + " in " + originatingDataSource)
		{
		}
	}
}
