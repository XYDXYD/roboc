using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIButtonBroadcasterEnterPlanetCategory : MonoBehaviour
{
	public Transform listener;

	public EnterPlanetCategory sendValue;

	public UIButtonBroadcasterEnterPlanetCategory()
		: this()
	{
	}

	public void OnClick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		new SignalChain(listener).Send<EnterPlanetCategory>(sendValue);
	}
}
