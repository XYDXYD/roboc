using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal class CloseWindowListener : MonoBehaviour, IChainListener
{
	public CloseWindowListener()
		: this()
	{
	}

	public void Listen(object message)
	{
		if (message is ButtonType && (ButtonType)message == ButtonType.Close)
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
