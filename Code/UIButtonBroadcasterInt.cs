using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIButtonBroadcasterInt : MonoBehaviour
{
	public Transform listener;

	public int sendValue;

	public UIButtonBroadcasterInt()
		: this()
	{
	}

	public void OnClick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		new SignalChain(listener).Send<int>(sendValue);
	}
}
