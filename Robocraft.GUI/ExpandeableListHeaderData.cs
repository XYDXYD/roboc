namespace Robocraft.GUI
{
	public class ExpandeableListHeaderData
	{
		public int positionInData;

		public bool expandedStatus;

		public string headerText;

		public int colorIndex;

		public ExpandeableListHeaderData(int position_, bool expandedStatus_, string headerText_, int colorIndex_)
		{
			positionInData = position_;
			expandedStatus = expandedStatus_;
			headerText = headerText_;
			colorIndex = colorIndex_;
		}
	}
}
