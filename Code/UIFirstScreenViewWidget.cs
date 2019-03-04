using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIFirstScreenViewWidget : MonoBehaviour, IChainListener
{
	public string saveCode;

	public UIFirstScreenViewWidget()
		: this()
	{
	}

	private void Start()
	{
		if (PlayerPrefs.HasKey(saveCode))
		{
			this.get_gameObject().SetActive(false);
		}
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.HideAPanel)
			{
				this.get_gameObject().SetActive(false);
				PlayerPrefs.SetInt(saveCode, 1);
			}
		}
	}
}
