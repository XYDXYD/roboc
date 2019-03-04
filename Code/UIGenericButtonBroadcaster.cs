using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIGenericButtonBroadcaster : MonoBehaviour
{
	[SerializeField]
	private Transform listener;

	public ButtonType buttonType;

	private SignalChain _signal;

	public UIGenericButtonBroadcaster()
		: this()
	{
	}

	public void OnClick()
	{
		_signal.Send<ButtonType>(buttonType);
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		_signal = new SignalChain(listener);
	}
}
