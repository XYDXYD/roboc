namespace Robocraft.GUI
{
	public class ListHeaderInfo
	{
		public string HeaderName;

		public int ColorIndex;

		public bool ExpandedStatus;

		public ListHeaderInfo(string headerName, int colorIndex, bool expandedStatus)
		{
			HeaderName = headerName;
			ColorIndex = colorIndex;
			ExpandedStatus = expandedStatus;
		}
	}
}
