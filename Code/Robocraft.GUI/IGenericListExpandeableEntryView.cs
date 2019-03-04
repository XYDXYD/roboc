namespace Robocraft.GUI
{
	public interface IGenericListExpandeableEntryView : IGenericListEntryView
	{
		bool IsHeaderEntry
		{
			get;
			set;
		}
	}
}
