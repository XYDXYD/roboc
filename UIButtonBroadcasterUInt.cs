using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIButtonBroadcasterUInt : MonoBehaviour
{
	public Transform listener;

	public uint sendValue;

	public UIButtonBroadcasterUInt()
		: this()
	{
	}

	public void OnClick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		new SignalChain(listener).Send<uint>(sendValue);
	}
}
