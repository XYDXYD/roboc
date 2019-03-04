using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIHidePanelListener : MonoBehaviour, IChainListener
{
	public UIHidePanelListener()
		: this()
	{
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.HideAPanel)
			{
				this.get_gameObject().SetActive(false);
			}
		}
	}
}
