using Authentication;
using CustomGames;
using Robocraft.GUI;
using ServerStateServiceLayer;
using ServerStateServiceLayer.EventListeners.Photon;
using Services.Web;
using Services.Web.Photon;
using Simulation;
using Svelto.Command;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class CustomGameScreenController : IGUIDisplay, IInitialize, IWaitForFrameworkDestruction, IComponent
	{
		private CustomGameScreen _view;

		private IServiceEventContainer _serverStateEventContainer;

		private CustomGameOptionsDataSource _optionsDataSource;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IServerStateEventContainerFactory serverStateEventContainerFactory
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameStateObservable customGameStateObservable
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameGameModeObservable gameModeChangeObservable
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameGameModeObserver gameModeChangedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameStateObserver customGameStateObserver
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameOptionsFactory customGameOptionsFactory
		{
			private get;
			set;
		}

		public bool doesntHideOnSwitch => false;

		public GuiScreens screenType => GuiScreens.CustomGameScreen;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyGUINoSwitching;

		public HudStyle battleHudStyle => HudStyle.Full;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public unsafe void OnDependenciesInjected()
		{
			BindEventListeners();
			_optionsDataSource = new CustomGameOptionsDataSource(serviceFactory);
			customGameStateObserver.AddAction(new ObserverAction<CustomGameStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameModeChangedObserver.AddAction(new ObserverAction<GameModeType>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			customGameStateObserver.RemoveAction(new ObserverAction<CustomGameStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameModeChangedObserver.RemoveAction(new ObserverAction<GameModeType>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnCustomGameStateChanged(ref CustomGameStateDependency dependancy)
		{
			RefreshOptionsDataSourceAndDispatchMessage();
		}

		private void RefreshOptionsDataSourceAndDispatchMessage()
		{
			_optionsDataSource.RefreshData(delegate
			{
				_view.DeepBroadcastGenericMessage(MessageType.RefreshData, string.Empty);
			}, delegate
			{
				Console.Log("error refreshing options data");
			});
		}

		private void BindEventListeners()
		{
			_serverStateEventContainer = serverStateEventContainerFactory.Create();
			_serverStateEventContainer.ListenTo<ICustomGameConfigChangedEventListener, CustomGameConfigChangedData>(OnCustomGameConfigChangedEvent);
			_serverStateEventContainer.ListenTo<ICustomGameLeaderChangedEventListener, string>(OnCustomGameLeaderChangedEvent);
			_serverStateEventContainer.ListenTo<ICustomGameKickedFromSessionEventListener, KickedFromCustomGameSessionData>(HandleKickFromCustomGameEvent);
		}

		private void OnCustomGameLeaderChangedEvent(string newLeader)
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateDropDownSelectability);
		}

		private void HandleKickFromCustomGameEvent(KickedFromCustomGameSessionData data)
		{
			if (!data.WasInvited)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strYouWereKickedFromCustomGameTitle"), StringTableBase<StringTable>.Instance.GetString("strYouWereKickedFromCustomGameBody"), StringTableBase<StringTable>.Instance.GetString("strOK")));
			}
			if (guiInputController.GetActiveScreen() == GuiScreens.CustomGameScreen)
			{
				guiInputController.CloseCurrentScreen();
			}
			CustomGameStateDependency customGameStateDependency = new CustomGameStateDependency(null);
			customGameStateObservable.Dispatch(ref customGameStateDependency);
		}

		private void OnCustomGameConfigChangedEvent(CustomGameConfigChangedData data)
		{
			Console.Log("Received config changed event:" + data.FieldChanged + "," + data.NewValue);
			if (data.FieldChanged.CompareTo("GameMode") == 0)
			{
				_view.DeepBroadcast(CustomGameGUIEvent.Type.LeaderSetGameModeChoice, data.NewValue);
				_view.DeepBroadcast(CustomGameGUIEvent.Type.RefreshAndUpdateMapChoicesList);
			}
			else if (data.FieldChanged.CompareTo("MapChoice") == 0)
			{
				_view.DeepBroadcast(CustomGameGUIEvent.Type.LeaderSetMapChoice, data.NewValue);
			}
			else
			{
				RefreshOptionsDataSourceAndDispatchMessage();
			}
		}

		private void OnCustomGameModeChanged(ref GameModeType newGameMode)
		{
			Console.Log("game mode change detected: updating dependants! 1. map choices list");
			_view.DeepBroadcast(CustomGameGUIEvent.Type.RefreshAndUpdateMapChoicesList);
			Console.Log("game mode change detected: updating dependants! 2. toggle option visibility");
			if (newGameMode == GameModeType.Normal)
			{
				_view.DeepBroadcastGenericMessage(MessageType.Show, CustomGameOptionsDataSource.OptionEnum.CaptureSegmentMemoryYesNo.ToString());
			}
			else
			{
				_view.DeepBroadcastGenericMessage(MessageType.Hide, CustomGameOptionsDataSource.OptionEnum.CaptureSegmentMemoryYesNo.ToString());
			}
			if (newGameMode == GameModeType.SuddenDeath)
			{
				_view.DeepBroadcastGenericMessage(MessageType.Show, CustomGameOptionsDataSource.OptionEnum.CaptureEliminationTimeValue.ToString());
			}
			else
			{
				_view.DeepBroadcastGenericMessage(MessageType.Hide, CustomGameOptionsDataSource.OptionEnum.CaptureEliminationTimeValue.ToString());
			}
			if (newGameMode == GameModeType.Pit)
			{
				_view.DeepBroadcastGenericMessage(MessageType.Hide, CustomGameOptionsDataSource.OptionEnum.GameTimeValue.ToString());
			}
			else
			{
				_view.DeepBroadcastGenericMessage(MessageType.Show, CustomGameOptionsDataSource.OptionEnum.GameTimeValue.ToString());
			}
			_view.MenuOptionsVisibilityChanged();
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return _view.IsActive();
		}

		public GUIShowResult Show()
		{
			_view.Show();
			SetupOptions();
			return GUIShowResult.Showed;
		}

		private void SetupOptions()
		{
			_view.DeepBroadcast(CustomGameGUIEvent.Type.RefreshAndUpdateGameModeChoicesList);
			_view.DeepBroadcast(CustomGameGUIEvent.Type.RefreshAndUpdateMapChoicesList);
			RefreshOptionsDataSourceAndDispatchMessage();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateDropDownSelectability);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateChatDecorationVisibility);
		}

		private IEnumerator UpdateChatDecorationVisibility()
		{
			TaskService<ChatSettingsData> task = new TaskService<ChatSettingsData>(serviceFactory.Create<ILoadChatSettingsRequest>());
			yield return task;
			if (task.succeeded)
			{
				_view.SetChatDecorationVisible(task.result.chatEnabled);
			}
		}

		private IEnumerator UpdateDropDownSelectability()
		{
			IRetrieveCustomGameSessionRequest retrieveCustomGameRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveCustomGameInfoTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveCustomGameRequest);
			yield return retrieveCustomGameInfoTask;
			if (!retrieveCustomGameInfoTask.succeeded)
			{
				yield break;
			}
			RetrieveCustomGameSessionRequestData result = retrieveCustomGameInfoTask.result;
			if (result.Data != null)
			{
				_view.DeepBroadcast(CustomGameGUIEvent.Type.LeaderSet, result.Data.SessionLeader);
				if (result.Data.SessionLeader.CompareTo(User.Username) != 0)
				{
					_view.DeepBroadcastGenericMessage(MessageType.Disable, string.Empty);
				}
				else
				{
					_view.DeepBroadcastGenericMessage(MessageType.Enable, string.Empty);
				}
			}
		}

		public bool Hide()
		{
			_view.Hide();
			return true;
		}

		public void HandleGenericMessage(GenericComponentMessage message)
		{
			if (message.Message == MessageType.TickBoxSet)
			{
				CustomGameOptionsDataSource.OptionEnum optionChosen = (CustomGameOptionsDataSource.OptionEnum)Enum.Parse(typeof(CustomGameOptionsDataSource.OptionEnum), message.Originator);
				HandleUserSetsBooleanOption(optionChosen, setting: true);
			}
			if (message.Message == MessageType.TickBoxCleared)
			{
				CustomGameOptionsDataSource.OptionEnum optionChosen2 = (CustomGameOptionsDataSource.OptionEnum)Enum.Parse(typeof(CustomGameOptionsDataSource.OptionEnum), message.Originator);
				HandleUserSetsBooleanOption(optionChosen2, setting: false);
			}
			if (message.Message == MessageType.SliderValueAdjusting)
			{
				CustomGameOptionsDataSource.OptionEnum optionEnum = (CustomGameOptionsDataSource.OptionEnum)Enum.Parse(typeof(CustomGameOptionsDataSource.OptionEnum), message.Originator);
				float sliderNormalisedValue = (message.Data as SliderComponentDataContainer).UnpackData<float>();
				int num = _optionsDataSource.TranslateNormalisedValueToPercentageMultiplier(optionEnum, sliderNormalisedValue);
				string entry = CustomGameOptionsUtility.FormatHintTipForCustomGameOption<string>(optionEnum, num);
				TextLabelComponentDataContainer dataContainer = new TextLabelComponentDataContainer(entry);
				_view.DeepBroadcastGenericMessage(MessageType.SetData, message.Originator + "_valuelabel", dataContainer);
			}
			if (message.Message == MessageType.SliderValueConfirmed)
			{
				CustomGameOptionsDataSource.OptionEnum optionEnum2 = (CustomGameOptionsDataSource.OptionEnum)Enum.Parse(typeof(CustomGameOptionsDataSource.OptionEnum), message.Originator);
				float sliderNormalisedValue2 = (message.Data as SliderComponentDataContainer).UnpackData<float>();
				int settingValue = _optionsDataSource.TranslateNormalisedValueToPercentageMultiplier(optionEnum2, sliderNormalisedValue2);
				HandleUserSetsSliderValue(optionEnum2, settingValue);
			}
		}

		private void HandleUserSetsSliderValue(CustomGameOptionsDataSource.OptionEnum optionChosen, int settingValue)
		{
			switch (optionChosen)
			{
			case CustomGameOptionsDataSource.OptionEnum.HealthRegenYesNo:
			case CustomGameOptionsDataSource.OptionEnum.CaptureSegmentMemoryYesNo:
			case CustomGameOptionsDataSource.OptionEnum.BaseShieldsGoDownYesNo:
			case CustomGameOptionsDataSource.OptionEnum.PointsMultipliedByKillStreakOnOff:
				return;
			}
			CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig = CustomGameOptionsUtility.customGameObjectTypes[(int)optionChosen];
			string settingString = customGameOptionsConfig.SettingString;
			TaskRunner.get_Instance().Run(HandleChangeConfig(settingValue.ToString(), settingString));
		}

		private void HandleUserSetsBooleanOption(CustomGameOptionsDataSource.OptionEnum optionChosen, bool setting)
		{
			string newSetting = setting.ToString();
			switch (optionChosen)
			{
			case CustomGameOptionsDataSource.OptionEnum.HealthRegenYesNo:
				TaskRunner.get_Instance().Run(HandleChangeConfig(newSetting, "HealthRegen"));
				break;
			case CustomGameOptionsDataSource.OptionEnum.BaseShieldsGoDownYesNo:
				TaskRunner.get_Instance().Run(HandleChangeConfig(newSetting, "BaseShieldsGoDown"));
				break;
			case CustomGameOptionsDataSource.OptionEnum.CaptureSegmentMemoryYesNo:
				TaskRunner.get_Instance().Run(HandleChangeConfig(newSetting, "CaptureSegmentMemory"));
				break;
			case CustomGameOptionsDataSource.OptionEnum.PointsMultipliedByKillStreakOnOff:
				TaskRunner.get_Instance().Run(HandleChangeConfig(newSetting, "PointsKillStreakOnOff"));
				break;
			}
		}

		public void HandleCustomGameGUIMessage(CustomGameGUIEvent message)
		{
			if (message.type == CustomGameGUIEvent.Type.UserSetGameModeChoice)
			{
				TaskRunner.get_Instance().Run(HandleChangeConfigThenRefreshDependantsForGameMode(message.Data, "GameMode"));
			}
			if (message.type == CustomGameGUIEvent.Type.UserSetMapChoice)
			{
				TaskRunner.get_Instance().Run(HandleChangeConfig(message.Data, "MapChoice"));
			}
		}

		public IEnumerator HandleChangeConfigThenRefreshDependantsForGameMode(string newSetting, string configToChange)
		{
			yield return HandleChangeConfig(newSetting, configToChange);
			loadingIconPresenter.NotifyLoading("CustomGameScreen");
			IRetrieveCustomGameSessionRequest retrieveCustomGameRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			retrieveCustomGameRequest.ClearCache();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveCustomGameInfoTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveCustomGameRequest);
			yield return new HandleTaskServiceWithError(retrieveCustomGameInfoTask, delegate
			{
				loadingIconPresenter.NotifyLoading("CustomGameScreen");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			_view.DeepBroadcast(CustomGameGUIEvent.Type.RefreshAndUpdateMapChoicesList);
			if (configToChange == "GameMode")
			{
				GameModeType gameModeType = (GameModeType)Enum.Parse(typeof(GameModeType), newSetting, ignoreCase: false);
				gameModeChangeObservable.Dispatch(ref gameModeType);
			}
		}

		public IEnumerator HandleChangeConfig(string newSetting, string configToChange)
		{
			loadingIconPresenter.NotifyLoading("CustomGameScreen");
			Console.Log("Change config: " + configToChange + " to: " + newSetting);
			IAdjustCustomGameConfigRequest request = serviceFactory.Create<IAdjustCustomGameConfigRequest>();
			request.Inject(new AdjustCustomGameConfigRequestDependancy(configToChange, newSetting));
			TaskService<AdjustCustomGameConfigurationResponse> taskService = new TaskService<AdjustCustomGameConfigurationResponse>(request);
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
				loadingIconPresenter.NotifyLoading("CustomGameScreen");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			switch (taskService.result)
			{
			case AdjustCustomGameConfigurationResponse.UserIsNotSessionLeader:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameAdjustErrorMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameAdjustErrorNoSessionLeaderBody"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				break;
			case AdjustCustomGameConfigurationResponse.AdjustSessionError:
			case AdjustCustomGameConfigurationResponse.SessionDoesntExist:
			case AdjustCustomGameConfigurationResponse.InvalidOrUnknownField:
			case AdjustCustomGameConfigurationResponse.InvalidFieldChoice:
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameAdjustErrorMessageHeader"), StringTableBase<StringTable>.Instance.GetString("strCustomGameConfigAdjustRejectedByServer"), StringTableBase<StringTable>.Instance.GetString("strOK")));
				break;
			}
			RefreshOptionsDataSourceAndDispatchMessage();
		}

		public IEnumerator LoadGUIData()
		{
			loadingIconPresenter.NotifyLoading("CustomGameScreen");
			ICustomGameGetTeamSetupRequest teamSetupRequest = serviceFactory.Create<ICustomGameGetTeamSetupRequest>();
			teamSetupRequest.Inject(GameModeType.Normal);
			TaskService<int> getTeamSetupTaskService = new TaskService<int>(teamSetupRequest);
			yield return new HandleTaskServiceWithError(getTeamSetupTaskService, delegate
			{
				loadingIconPresenter.NotifyLoading("CustomGameScreen");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			}).GetEnumerator();
			ILoadCustomGamesAllowedMapsRequest loadCustomGameRequest = serviceFactory.Create<ILoadCustomGamesAllowedMapsRequest>();
			TaskService<CustomGamesAllowedMapsData> taskService = new TaskService<CustomGamesAllowedMapsData>(loadCustomGameRequest);
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
				loadingIconPresenter.NotifyLoading("CustomGameScreen");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			}).GetEnumerator();
			IRetrieveCustomGameSessionRequest retrieveCustomGameRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			retrieveCustomGameRequest.ClearCache();
			TaskService<RetrieveCustomGameSessionRequestData> retrieveCustomGameInfoTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveCustomGameRequest);
			yield return new HandleTaskServiceWithError(retrieveCustomGameInfoTask, delegate
			{
				loadingIconPresenter.NotifyLoading("CustomGameScreen");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			if (retrieveCustomGameInfoTask.succeeded)
			{
				yield return _optionsDataSource.RefreshData();
				RetrieveCustomGameSessionRequestData gameSessionRequestInfo = retrieveCustomGameInfoTask.result;
				if (gameSessionRequestInfo.Response == CustomGameSessionRetrieveResponse.SessionRetrieved)
				{
					Console.Log("player is in a custom game session:" + gameSessionRequestInfo.Data.SessionGUID);
					string value = retrieveCustomGameInfoTask.result.Data.Config["GameMode"];
					GameModeType gameModeType = (GameModeType)Enum.Parse(typeof(GameModeType), value);
					gameModeChangeObservable.Dispatch(ref gameModeType);
				}
				else if (gameSessionRequestInfo.Response == CustomGameSessionRetrieveResponse.PlayerIsInvitedOnly)
				{
					Console.Log("player has an outstanding invitation to a custom game session.");
				}
			}
			loadingIconPresenter.NotifyLoading("CustomGameScreen");
			ICustomGamePlayerStateChangedRequest updateStateRequest = serviceFactory.Create<ICustomGamePlayerStateChangedRequest>();
			updateStateRequest.Inject(CustomGamePlayerSessionStatus.Ready);
			TaskService updateStateTask = new TaskService(updateStateRequest);
			yield return new HandleTaskServiceWithError(updateStateTask, delegate
			{
				loadingIconPresenter.NotifyLoading("CustomGameScreen");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("CustomGameScreen");
		}

		public void SetView(CustomGameScreen view)
		{
			_view = view;
			customGameOptionsFactory.BuildAll(_view, _optionsDataSource);
			_view.ScrollView.ResetPosition();
		}

		internal void OnLeaveButtonClicked()
		{
			commandFactory.Build<LeaveCustomGameCommand>().Execute();
		}

		internal void OnReadyButtonClicked()
		{
			commandFactory.Build<ReadyForCustomGameCommand>().Execute();
		}

		internal void OnBackButtonClicked()
		{
			guiInputController.CloseCurrentScreen();
		}

		public void LeaveCustomGameScreen()
		{
			if (guiInputController.GetActiveScreen() == GuiScreens.CustomGameScreen)
			{
				guiInputController.CloseCurrentScreen();
				guiInputController.ShowScreen(GuiScreens.PlayScreen);
			}
		}
	}
}
