using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIButtonBroadcasterString : MonoBehaviour
{
	public Transform listener;

	public string sendValue;

	public UIButtonBroadcasterString()
		: this()
	{
	}

	public void OnClick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		new SignalChain(listener).Send<string>(sendValue);
	}
}
