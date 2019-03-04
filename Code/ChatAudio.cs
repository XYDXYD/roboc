using Fabric;

internal class ChatAudio
{
	private const string ENTER_CHAT_MODE = "GUI_ChatMode_Enter";

	private const string LEAVE_CHAT_MODE = "GUI_ChatMode_Exit";

	private const string MESSAGE_SENT = "GUI_Chat_Message_Sent";

	private const string MESSAGE_RECEIVED_ALLY = "GUI_Chat_Message_Received_SameTeam";

	private const string MESSAGE_RECEIVED_ENEMY = "GUI_Chat_Message_Received_EnemyTeam";

	private const string CHARACTER_INPUT = "GUI_Chat_Char_Input";

	public void EnterChatMode()
	{
		PlaySound("GUI_ChatMode_Enter");
	}

	public void LeaveChatMode()
	{
		PlaySound("GUI_ChatMode_Exit");
	}

	public void MessageSent()
	{
		PlaySound("GUI_Chat_Message_Sent");
	}

	public void MessageReceived(bool ally)
	{
		PlaySound((!ally) ? "GUI_Chat_Message_Received_EnemyTeam" : "GUI_Chat_Message_Received_SameTeam");
	}

	private void PlaySound(string soundEvent)
	{
		EventManager.get_Instance().PostEvent(soundEvent, 0);
	}
}
