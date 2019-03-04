using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIToggleButtonBroadcaster : MonoBehaviour
{
	public Transform listenerParent;

	public GameObject toggleObject;

	public UIToggleButtonBroadcaster()
		: this()
	{
	}

	private void Awake()
	{
		if (toggleObject == null)
		{
			toggleObject = this.get_gameObject();
		}
	}

	public void OnClick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		new SignalChain(listenerParent).Broadcast<GameObject>(toggleObject);
	}
}
