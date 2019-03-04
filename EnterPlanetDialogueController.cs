using Mothership;
using Mothership.GUI;
using Simulation;
using SinglePlayerCampaign.GUI;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using UnityEngine;

internal sealed class EnterPlanetDialogueController : IGUIDisplay, IInitialize, IWaitForFrameworkInitialization, IComponent
{
	private BattleAvailabilityDependancyData _battleAvailabilityDependancy;

	private EnterPlanetDialogue _view;

	[Inject]
	public NormalBattleAvailability normalModeAvailability
	{
		private get;
		set;
	}

	[Inject]
	public TeamDeathMatchAvailability teamDeathMatchAvailability
	{
		private get;
		set;
	}

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	public IGUIInputControllerMothership guiController
	{
		private get;
		set;
	}

	[Inject]
	public LoadingIconPresenter loadingIconPresenter
	{
		private get;
		set;
	}

	[Inject]
	private MachineEditorBuilder machineBuilder
	{
		get;
		set;
	}

	[Inject]
	public GaragePresenter garagePresenter
	{
		private get;
		set;
	}

	[Inject]
	public IEntityFactory entityFactory
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.PlayScreen;

	public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

	public ShortCutMode shortCutMode => ShortCutMode.AllShortCuts;

	public bool isScreenBlurred => false;

	public bool hasBackground => false;

	public HudStyle battleHudStyle => HudStyle.Full;

	public bool doesntHideOnSwitch => false;

	public event Action OnEnterBattleDialogue = delegate
	{
	};

	public void OnDependenciesInjected()
	{
		machineBuilder.OnMachineBuilt += OnRobotFinishedBuilding;
	}

	public void OnFrameworkInitialized()
	{
		GameModePreferencesScreenFactory.Build(entityFactory, _view.gameModePreferencesMenu.get_gameObject());
		_view.gameModePreferencesMenu.isShown.NotifyOnValueSet((Action<int, bool>)OnGameModePreferencesVisibilityChanged);
		SinglePlayerCampaignScreenFactory.BuildMothershipUI(entityFactory, _view.singlePlayerCampaignListScreen.get_gameObject(), serviceFactory);
		entityFactory.BuildEntity<TierHighlightWidgetEntityDescriptor>(_view.tierDisplayer.get_gameObject().GetInstanceID(), new object[1]
		{
			_view.tierDisplayer
		});
	}

	private void OnGameModePreferencesVisibilityChanged(int entityId, bool shown)
	{
		if (!shown)
		{
			_view.ChangeCategory(EnterPlanetCategory.None);
		}
	}

	private void OnRobotFinishedBuilding(uint garageSlot)
	{
		if (IsActive())
		{
			UpdateModeAvailability();
		}
	}

	public unsafe void SetView(EnterPlanetDialogue view)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		_view = view;
		_view.AddBackButtonEvent(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public void EnableBackground(bool enable)
	{
	}

	public bool IsActive()
	{
		if (_view == null)
		{
			return false;
		}
		return _view.IsActive();
	}

	public bool IsModeAvailable(GameModeType gameMode)
	{
		switch (gameMode)
		{
		case GameModeType.Normal:
			return normalModeAvailability.GetBattleAvailability(_battleAvailabilityDependancy) == GameModeAvailabilityState.Enabled;
		case GameModeType.TeamDeathmatch:
			return teamDeathMatchAvailability.GetBattleAvailability(_battleAvailabilityDependancy) == GameModeAvailabilityState.Enabled;
		case GameModeType.EditMode:
			return GetEditModeAvailability() == GameModeAvailabilityState.Enabled;
		default:
			return false;
		}
	}

	public GUIShowResult Show()
	{
		_view.ChangeCategory(EnterPlanetCategory.None);
		_view.Show();
		_view.SetNormalModeAvailability(GameModeAvailabilityState.PlayerLevelTooLow);
		_view.SetCustomGamesAvailability(GameModeAvailabilityState.Enabled);
		this.OnEnterBattleDialogue();
		UpdateModeAvailability();
		return GUIShowResult.Showed;
	}

	public IEnumerator LoadData()
	{
		loadingIconPresenter.NotifyLoading("EnterPlanetDialog");
		int playerLevel = 0;
		ILoadLeagueBattleParametersRequest loadleagueBattleParameters = serviceFactory.Create<ILoadLeagueBattleParametersRequest>();
		TaskService<uint[]> loadLeaguesTask = new TaskService<uint[]>(loadleagueBattleParameters);
		yield return new HandleTaskServiceWithError(loadLeaguesTask, delegate
		{
			loadingIconPresenter.NotifyLoading("EnterPlanetDialog");
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("EnterPlanetDialog");
		}).GetEnumerator();
		ILoadTotalXPRequest experienceRequest = serviceFactory.Create<ILoadTotalXPRequest>();
		TaskService<uint[]> loadExperienceTask = new TaskService<uint[]>(experienceRequest);
		yield return new HandleTaskServiceWithError(loadExperienceTask, delegate
		{
			loadingIconPresenter.NotifyLoading("EnterPlanetDialog");
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("EnterPlanetDialog");
		}).GetEnumerator();
		bool loadPlayerLevelSuccess = false;
		yield return PlayerLevelHelper.LoadCurrentPlayerLevel(serviceFactory, delegate(PlayerLevelAndProgress playerLevelData)
		{
			playerLevel = (int)playerLevelData.playerLevel;
			loadPlayerLevelSuccess = true;
		}, delegate
		{
			loadPlayerLevelSuccess = false;
		});
		if (!loadExperienceTask.succeeded || !loadPlayerLevelSuccess || !loadLeaguesTask.succeeded)
		{
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strNetworkError"), StringTableBase<StringTable>.Instance.GetString("strUnableLoadPlayerLevelCloud"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				Application.Quit();
			}));
		}
		uint[] leagueParameters = loadLeaguesTask.result;
		_battleAvailabilityDependancy = new BattleAvailabilityDependancyData(maxLevelForLeague_: leagueParameters[0], playerLevel_: playerLevel);
		loadingIconPresenter.NotifyLoadingDone("EnterPlanetDialog");
	}

	public bool Hide()
	{
		_view.Hide();
		return true;
	}

	internal void UpdateModeAvailability()
	{
		_view.SetNormalModeAvailability(normalModeAvailability.GetBattleAvailability(_battleAvailabilityDependancy));
	}

	private GameModeAvailabilityState GetEditModeAvailability()
	{
		return GameModeAvailabilityState.Enabled;
	}

	internal void Listen(object message)
	{
		if (message.GetType() == typeof(ButtonType))
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.Cancel)
			{
				guiController.CloseCurrentScreen();
			}
		}
		else if (message.GetType() == typeof(EnterPlanetCategory))
		{
			EnterPlanetCategory newCategory = (EnterPlanetCategory)message;
			_view.ChangeCategory(newCategory);
		}
	}
}
