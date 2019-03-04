using PlayMaker;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class NGUIEventRedirector : MonoBehaviour, IChainListener, IChainRoot
{
	[Inject]
	internal IPlayMakerStateMachineBridge playMakerStateMachineBridge
	{
		private get;
		set;
	}

	public NGUIEventRedirector()
		: this()
	{
	}

	void IChainListener.Listen(object message)
	{
		if (message is UIPlayMakerButtonClickBroadcaster)
		{
			UIPlayMakerButtonClickBroadcaster uIPlayMakerButtonClickBroadcaster = message as UIPlayMakerButtonClickBroadcaster;
			playMakerStateMachineBridge.SendPlaymakerEventToAllFSM(PlaymakerEventType.ButtonClick, uIPlayMakerButtonClickBroadcaster.eventName);
		}
	}
}
