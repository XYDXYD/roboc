using System.Text;

internal class HelpMessage : ChatMessage
{
	public override bool PlaySound => false;

	public override bool ShouldFilterProfanity => false;

	public HelpMessage(string rawText)
		: base(rawText, null)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChatColours.GetColourString(ChatColours.ChatColour.HelpText));
		stringBuilder.Append(base.RawText);
		return stringBuilder.ToString();
	}
}
