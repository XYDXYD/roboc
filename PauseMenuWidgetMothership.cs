using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;
using Utility;

internal sealed class PauseMenuWidgetMothership : MonoBehaviour, IChainListener, IPauseMenuWidget
{
	public enum MenuType
	{
		Mothership,
		BuildMode,
		BuildModeTutorial
	}

	public GameObject pauseMenuObjectMothership;

	public GameObject pauseMenuObjectBuildMode;

	public GameObject pauseMenuObjectBuildModeTutorial;

	public GameObject quitConfirmDialog;

	public GameObject enterPromoCodeButton;

	private bool _isActive;

	[Inject]
	public IPauseMenuController pauseMenuController
	{
		private get;
		set;
	}

	public PauseMenuWidgetMothership()
		: this()
	{
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			pauseMenuController.Clicked(buttonType);
		}
	}

	private void Start()
	{
		pauseMenuController.SetWidget(this);
		Hide();
	}

	public void Show(MenuType state)
	{
		_isActive = true;
		switch (state)
		{
		case MenuType.Mothership:
			pauseMenuObjectMothership.SetActive(true);
			break;
		case MenuType.BuildMode:
			pauseMenuObjectBuildMode.SetActive(true);
			break;
		case MenuType.BuildModeTutorial:
			pauseMenuObjectBuildModeTutorial.SetActive(true);
			break;
		}
		CheckIfLoadingScreenIsActive();
		quitConfirmDialog.SetActive(false);
	}

	private static void CheckIfLoadingScreenIsActive()
	{
		GenericLoadingScreen genericLoadingScreen = Object.FindObjectOfType<GenericLoadingScreen>();
		if (genericLoadingScreen != null)
		{
			Console.LogError("Loading Icon Still Opened when setting menu requested, esc shouldn't be available if loading icon is opened");
			Console.LogError("Loading Screen: " + genericLoadingScreen.get_gameObject().get_name());
			genericLoadingScreen.get_gameObject().SetActive(false);
		}
	}

	public void Hide()
	{
		_isActive = false;
		pauseMenuObjectBuildMode.SetActive(false);
		pauseMenuObjectBuildModeTutorial.SetActive(false);
		pauseMenuObjectMothership.SetActive(false);
		quitConfirmDialog.SetActive(false);
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void ShowConfirmQuitDialog(bool show)
	{
		pauseMenuObjectMothership.SetActive(!show);
		quitConfirmDialog.SetActive(show);
	}
}
