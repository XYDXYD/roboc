using System.Text;

internal class SocialEventMessage : ChatMessage
{
	public override bool PlaySound => false;

	public override bool ShouldFilterProfanity => false;

	public SocialEventMessage(string rawText)
		: base(rawText, null)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChatColours.GetColourString(ChatColours.ChatColour.SocialEvent));
		stringBuilder.Append(base.RawText);
		return stringBuilder.ToString();
	}
}
