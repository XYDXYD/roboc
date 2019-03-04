using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class PauseMenuWidgetPitMode : MonoBehaviour, IChainListener, IPauseMenuWidget
{
	public enum MenuType
	{
		Mothership,
		PracticeMode,
		TestMode,
		Multiplayer,
		Spectator,
		ClassicMode,
		PitMode
	}

	public GameObject quitConfirmDialog;

	public GameObject pitPauseMenuObject;

	private bool _isActive;

	[Inject]
	public IPauseMenuController pauseMenuController
	{
		private get;
		set;
	}

	public PauseMenuWidgetPitMode()
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
		if (state == MenuType.PitMode)
		{
			pitPauseMenuObject.SetActive(true);
		}
		quitConfirmDialog.SetActive(false);
	}

	public void Hide()
	{
		_isActive = false;
		quitConfirmDialog.SetActive(false);
		pitPauseMenuObject.SetActive(false);
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void ShowConfirmQuitDialog(bool show)
	{
		quitConfirmDialog.SetActive(show);
	}
}
