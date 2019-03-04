using Svelto.UI.Comms.SignalChain;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
internal class ShowTextOnChatEvent : MonoBehaviour, IChainListener
{
	public ChatGUIEvent.Type eventType = ChatGUIEvent.Type.SetChannel;

	private UILabel _label;

	public ShowTextOnChatEvent()
		: this()
	{
	}

	public void Listen(object obj)
	{
		if (!(obj is ChatGUIEvent))
		{
			return;
		}
		ChatGUIEvent chatGUIEvent = (ChatGUIEvent)obj;
		if (chatGUIEvent.type == eventType)
		{
			if (_label == null)
			{
				_label = this.GetComponent<UILabel>();
			}
			_label.set_text(chatGUIEvent.text);
		}
	}
}
