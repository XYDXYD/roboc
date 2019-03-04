using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class OpenURLOnButtonConfirm : MonoBehaviour, IChainListener
{
	public string url = string.Empty;

	public OpenURLOnButtonConfirm()
		: this()
	{
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.Confirm && url != string.Empty)
			{
				Application.OpenURL(url);
			}
		}
	}
}
