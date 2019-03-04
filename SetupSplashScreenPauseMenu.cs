using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class SetupSplashScreenPauseMenu : MonoBehaviour, IChainListener
{
	public GameObject pauseMenuObject;

	public SetupSplashScreenPauseMenu()
		: this()
	{
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.Confirm)
			{
				Application.Quit();
			}
			if (buttonType == ButtonType.Cancel)
			{
				pauseMenuObject.SetActive(false);
			}
		}
	}

	public void Enable()
	{
		pauseMenuObject.SetActive(true);
	}
}
