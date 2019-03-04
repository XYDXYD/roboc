namespace Mothership
{
	public class ChangeTabTypeData
	{
		public int tabIndex;

		public ClanSectionType typeToChangeTo;

		public ChangeTabTypeData(int tabIndex_, ClanSectionType sectionType)
		{
			tabIndex = tabIndex_;
			typeToChangeTo = sectionType;
		}
	}
}
