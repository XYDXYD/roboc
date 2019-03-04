using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal class UIScrollBubbleRoot : MonoBehaviour, IScrollBubbleRoot, IChainListener
{
	public UISlider scrollbar;

	public float scrollSpeed = 1f;

	public UIScrollBubbleRoot()
		: this()
	{
	}

	public void Listen(object obj)
	{
		if (obj is UIScrollBubbleMessage)
		{
			UIScrollBubbleMessage uIScrollBubbleMessage = (UIScrollBubbleMessage)obj;
			UISlider obj2 = scrollbar;
			obj2.set_value(obj2.get_value() - uIScrollBubbleMessage.scroll * scrollSpeed);
		}
	}
}
