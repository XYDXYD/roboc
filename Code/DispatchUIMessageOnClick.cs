using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class DispatchUIMessageOnClick : MonoBehaviour
{
	public string message;

	private BubbleSignal<Camera> _signal;

	public DispatchUIMessageOnClick()
		: this()
	{
	}

	private void Start()
	{
		_signal = new BubbleSignal<Camera>(this.get_transform());
	}

	private void OnClick()
	{
		_signal.DeepDispatch<string>(message);
	}
}
