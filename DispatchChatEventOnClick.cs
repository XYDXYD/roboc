using Svelto.UI.Comms.SignalChain;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
internal class DispatchChatEventOnClick : MonoBehaviour
{
	public ChatGUIEvent.Type type;

	public string text;

	private BubbleSignal<IChainRoot> _bubble;

	public DispatchChatEventOnClick()
		: this()
	{
	}

	private void Start()
	{
		_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
	}

	private void OnClick()
	{
		_bubble.Dispatch<ChatGUIEvent>(new ChatGUIEvent(type, text));
	}
}
