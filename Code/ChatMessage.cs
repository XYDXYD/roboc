using Authentication;
using UnityEngine;

internal abstract class ChatMessage
{
	internal struct MessageSender
	{
		public readonly string UserName;

		public readonly string DisplayName;

		public readonly bool IsDev;

		public readonly bool IsMod;

		public readonly bool IsAdmin;

		public MessageSender(string userName, string displayName, bool isDev, bool isMod, bool isAdmin)
		{
			UserName = userName;
			DisplayName = displayName;
			IsDev = isDev;
			IsMod = isMod;
			IsAdmin = isAdmin;
		}
	}

	public readonly float Created;

	public virtual bool PlaySound => true;

	public string ChannelName
	{
		get;
		private set;
	}

	public virtual string VisibleChannelName => ChannelName;

	public string RawText
	{
		get;
		private set;
	}

	public abstract bool ShouldFilterProfanity
	{
		get;
	}

	protected ChatMessage(string rawText, string channelName)
	{
		ChannelName = channelName;
		RawText = rawText;
		Created = Time.get_time();
	}

	internal static string GetLocalUserName()
	{
		return User.Username;
	}

	internal static string GetLocalDisplayName()
	{
		return User.DisplayName;
	}

	internal void SetText(string text)
	{
		RawText = text;
	}

	public virtual string ToString(bool withTeams)
	{
		return ToString();
	}
}
