using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class PauseMenuWidgetSimulation : MonoBehaviour, IChainListener, IPauseMenuWidget
{
	public enum MenuType
	{
		PracticeMode,
		TestMode,
		TestModeTutorial,
		Spectator,
		NormalMode,
		RankedNormalMode,
		BasicMode,
		BrawlEliminationMode,
		PitMode
	}

	public GameObject SurrenderCoolDownNormalModeObject;

	public GameObject SurrenderCoolDownNormalRankedModeObject;

	public GameObject SurrenderCoolDownBasicModeObject;

	public GameObject quitConfirmDialog;

	public GameObject classicSelfDestructDialog;

	public GameObject practiceModePauseMenuObject;

	public GameObject testModePauseMenuObject;

	public GameObject testModeTutorialPauseMenuObject;

	public GameObject spectatorPauseMenuObject;

	public GameObject normalModePauseMenuObject;

	public GameObject normalRankedModePauseMenuObject;

	public GameObject basicModePauseMenuObject;

	public GameObject brawlEliminationPauseMenuObject;

	public GameObject pitModeMenuObject;

	[SerializeField]
	private GameObject selfDestructClassicButton;

	private bool _isActive;

	[Inject]
	public IPauseMenuController pauseMenuController
	{
		private get;
		set;
	}

	public UIButton[] selfDestructClassicButtons
	{
		get;
		private set;
	}

	public PauseMenuWidgetSimulation()
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
		selfDestructClassicButtons = selfDestructClassicButton.GetComponents<UIButton>();
		for (int i = 0; i < selfDestructClassicButtons.Length; i++)
		{
			selfDestructClassicButtons[i].set_isEnabled(false);
		}
		Hide();
	}

	public void Show(MenuType state)
	{
		_isActive = true;
		switch (state)
		{
		case MenuType.RankedNormalMode:
			normalRankedModePauseMenuObject.SetActive(true);
			break;
		case MenuType.NormalMode:
			normalModePauseMenuObject.SetActive(true);
			break;
		case MenuType.BasicMode:
			basicModePauseMenuObject.SetActive(true);
			break;
		case MenuType.PracticeMode:
			practiceModePauseMenuObject.SetActive(true);
			break;
		case MenuType.TestMode:
			testModePauseMenuObject.SetActive(true);
			break;
		case MenuType.TestModeTutorial:
			testModeTutorialPauseMenuObject.SetActive(true);
			break;
		case MenuType.Spectator:
			spectatorPauseMenuObject.SetActive(true);
			break;
		case MenuType.BrawlEliminationMode:
			brawlEliminationPauseMenuObject.SetActive(true);
			break;
		case MenuType.PitMode:
			pitModeMenuObject.SetActive(true);
			break;
		}
		quitConfirmDialog.SetActive(false);
		classicSelfDestructDialog.SetActive(false);
	}

	public void Hide()
	{
		_isActive = false;
		normalRankedModePauseMenuObject.SetActive(false);
		normalModePauseMenuObject.SetActive(false);
		basicModePauseMenuObject.SetActive(false);
		practiceModePauseMenuObject.SetActive(false);
		testModePauseMenuObject.SetActive(false);
		testModeTutorialPauseMenuObject.SetActive(false);
		spectatorPauseMenuObject.SetActive(false);
		quitConfirmDialog.SetActive(false);
		brawlEliminationPauseMenuObject.SetActive(false);
		pitModeMenuObject.SetActive(false);
		classicSelfDestructDialog.SetActive(false);
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void ShowConfirmQuitDialog(bool show)
	{
		quitConfirmDialog.SetActive(show);
	}

	public void ShowClassicConfirmSelfDestructDialog(bool show)
	{
		classicSelfDestructDialog.SetActive(show);
	}
}
