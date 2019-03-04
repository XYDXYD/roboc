public class ThresholdData
{
	public readonly string Name;

	public readonly string LocalisedName;

	public readonly string HtmlColor;

	public readonly int VotesRequired;

	public ThresholdData(string Name_, string LocalisedName_, string HtmlColor_, int VotesRequired_)
	{
		Name = Name_;
		LocalisedName = LocalisedName_;
		HtmlColor = HtmlColor_;
		VotesRequired = VotesRequired_;
	}
}
