using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal class UIScrollBubble : MonoBehaviour
{
	private BubbleSignal<IScrollBubbleRoot> _bubble;

	public UIScrollBubble()
		: this()
	{
	}

	private void OnScroll(float scroll)
	{
		_bubble = new BubbleSignal<IScrollBubbleRoot>(this.get_transform());
		_bubble.Dispatch<UIScrollBubbleMessage>(new UIScrollBubbleMessage(scroll));
	}
}
