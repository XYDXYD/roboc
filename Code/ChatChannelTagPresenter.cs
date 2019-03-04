using UnityEngine;

internal class ChatChannelTagPresenter
{
	internal ChatChannelTagView view
	{
		private get;
		set;
	}

	public void OnGUIEvent(ChatGUIEvent ev)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (ev.type == ChatGUIEvent.Type.SetChannel)
		{
			IChatChannel channel = ev.channel;
			if (channel != null)
			{
				view.label.set_text(FormatText(channel.VisibleName));
				view.label.set_color(Color32.op_Implicit(ChatColours.GetColour(channel.ChatChannelType)));
			}
			else
			{
				view.label.set_text(string.Empty);
				view.label.set_color(Color32.op_Implicit(ChatColours.GetColour(ChatChannelType.None)));
			}
		}
	}

	private string FormatText(string channelName)
	{
		return "[" + channelName + "]: ";
	}
}
