using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIPanelShowListener : MonoBehaviour, IChainListener
{
	public UIPanel panel;

	public UIPanelShowListener()
		: this()
	{
	}

	private void Start()
	{
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.ShowAPanel)
			{
				panel.get_gameObject().SetActive(true);
			}
			if (buttonType == ButtonType.HideAPanel)
			{
				panel.get_gameObject().SetActive(false);
			}
		}
	}
}
