public class TutorialMessageData
{
	public readonly bool justHideExistingMessages;

	public readonly string text;

	public readonly float timeToShow;

	public TutorialMessageData(string text_, float timeToShow_, bool hide_)
	{
		justHideExistingMessages = hide_;
		text = text_;
		timeToShow = timeToShow_;
	}
}
