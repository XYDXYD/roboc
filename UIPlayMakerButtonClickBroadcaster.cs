using Svelto.UI.Comms.SignalChain;
using UnityEngine;

public sealed class UIPlayMakerButtonClickBroadcaster : MonoBehaviour
{
	public string eventName;

	private BubbleSignal<IChainRoot> _bubbleSignal;

	public UIPlayMakerButtonClickBroadcaster()
		: this()
	{
	}

	private void Start()
	{
		_bubbleSignal = new BubbleSignal<IChainRoot>(this.get_transform());
	}

	public void OnClick()
	{
		_bubbleSignal.Dispatch<UIPlayMakerButtonClickBroadcaster>(this);
	}
}
