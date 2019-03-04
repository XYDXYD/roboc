using System;

namespace Robocraft.GUI
{
	internal class InvalidDataIndexException : Exception
	{
		public InvalidDataIndexException(int dataIndex1, int dataIndex2, string originatingDataSource)
			: base("Attempted access of invalid data index: " + dataIndex1.ToString() + "," + dataIndex2.ToString() + " in " + originatingDataSource)
		{
		}
	}
}
