using HutongGames.PlayMaker;
using InputMask;
using PlayMaker;
using PlayMaker.Tutorial;
using Svelto.Command;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

internal abstract class TutorialControllerBase : IPlaymakerCommandProvider, IPlaymakerDataProvider, ITutorialController
{
	internal class TutorialServiceException : Exception
	{
		public TutorialServiceException(string serviceName)
			: base(serviceName)
		{
		}
	}

	private const int MAX_TUTORIAL_STAGES = 5;

	protected DateTime _signupDate;

	private LoadTutorialStatusData _tutorialStatus;

	private TutorialTipObservable _tutorialTipObserveable;

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		protected get;
		set;
	}

	[Inject]
	public IGUIInputController guiInputController
	{
		protected get;
		set;
	}

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		get;
		set;
	}

	[Inject]
	internal ICubeFactory cubeFactory
	{
		private get;
		set;
	}

	[Inject]
	public ICubeList cubeList
	{
		protected get;
		set;
	}

	[Inject]
	internal ICubeInventory cubeInventory
	{
		private get;
		set;
	}

	[Inject]
	internal GaragePresenter garage
	{
		get;
		set;
	}

	[Inject]
	protected ICommandFactory commandFactory
	{
		get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal PremiumMembership premiumMembership
	{
		private get;
		set;
	}

	[Inject]
	internal IInputActionMask inputActionMask
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingIconPresenter
	{
		get;
		private set;
	}

	public TutorialControllerBase(TutorialTipObservable tutorialTipObserveable)
	{
		_tutorialTipObserveable = tutorialTipObserveable;
	}

	void IPlaymakerCommandProvider.RegisterPlaymakerCommandHandlers(Action<Action<IPlaymakerCommandInputParameters>, Type> RegisterPlayMakerCommandHandlerAction)
	{
		RegisterPlayMakerCommandHandlerAction(HandleShowTutorialDialogCommandExecution, typeof(ShowTutorialDialogCommandParameters));
		RegisterPlayMakerCommandHandlerAction(HandleShowTutorialCubeGunPickUpCommandExecution, typeof(ShowTutorialCubeGunPickUpCommandParameters));
		RegisterPlayMakerCommandHandlerAction(HandleHideTutorialDialogCommandExecution, typeof(HideTutorialDialogCommandParameters));
		RegisterPlayMakerCommandHandlerAction(HandleCreateGameObjectWithInject, typeof(CreateGameObjectWithInjectCommandParameters));
		RegisterPlayMakerCommandHandlerAction(HandleToggleFunctionalityCommandExecution, typeof(ToggleFunctionalityNodeInputParameters));
		RegisterPlayMakerCommandHandlerAction(HandleSendAnalyticsEventCommandExecution, typeof(SendAnalyticEventNodeInputParameters));
		RegisterContextSpecificCommandHandlers(RegisterPlayMakerCommandHandlerAction);
	}

	void IPlaymakerDataProvider.RegisterPlaymakerRequestHandlers(Action<Type, Action<IPlayMakerDataRequest>> RegisterPlayMakerRequestHandler)
	{
		RegisterPlayMakerRequestHandler(typeof(GetTutorialStagePlaymakerRequest), HandleGetTutorialStageRequest);
		RegisterPlayMakerRequestHandler(typeof(SaveTutorialStagePlaymakerRequest), HandleSaveTutorialStageRequest);
		RegisterPlayMakerRequestHandler(typeof(ReloadTutorialRobotPlaymakerRequest), HandleReloadTutorialRobotRequest);
		RegisterPlayMakerRequestHandler(typeof(ResetTutorialRobotPlaymakerRequest), HandleResetTutorialRobotRequest);
		RegisterPlayMakerRequestHandler(typeof(CheckInTestModeContextRequest), HandleTestModeCheckRequest);
		RegisterContextSpecificRequestHandlers(RegisterPlayMakerRequestHandler);
	}

	public abstract void HideTutorialScreen();

	public IEnumerator RequestTutorialStatusData()
	{
		loadingIconPresenter.NotifyLoading("TutorialSimulationFlow");
		SerialTaskCollection requestTutorialTasks = new SerialTaskCollection();
		ILoadSignupDate loadSignupDateRequest = serviceFactory.Create<ILoadSignupDate>();
		ILoadTutorialStatusRequest loadStatusDataRequest = serviceFactory.Create<ILoadTutorialStatusRequest>();
		TaskService<DateTime> loadSignupDateTask = new TaskService<DateTime>(loadSignupDateRequest);
		TaskService<LoadTutorialStatusData> loadStatusDataTask = new TaskService<LoadTutorialStatusData>(loadStatusDataRequest);
		HandleTaskServiceWithError loadSignupDateTaskWithError = new HandleTaskServiceWithError(loadSignupDateTask, delegate
		{
			loadingIconPresenter.NotifyLoading("TutorialSimulationFlow");
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("TutorialSimulationFlow");
		});
		HandleTaskServiceWithError loadStatusDataTaskWithError = new HandleTaskServiceWithError(loadStatusDataTask, delegate
		{
			loadingIconPresenter.NotifyLoading("TutorialSimulationFlow");
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("TutorialSimulationFlow");
		});
		requestTutorialTasks.Add(loadSignupDateTaskWithError.GetEnumerator());
		requestTutorialTasks.Add(loadStatusDataTaskWithError.GetEnumerator());
		yield return requestTutorialTasks;
		if (loadSignupDateTask.succeeded)
		{
			_signupDate = loadSignupDateTask.result;
		}
		if (loadStatusDataTask.succeeded)
		{
			_tutorialStatus = loadStatusDataTask.result;
		}
		else
		{
			_tutorialStatus = new LoadTutorialStatusData(inProgress_: false, skipped_: false, completed_: false);
		}
		loadingIconPresenter.NotifyLoadingDone("TutorialSimulationFlow");
	}

	public abstract void SetDisplay(TutorialScreenBase display);

	public abstract void ShowTutorialScreenAndActivateFSM();

	public abstract bool IsActive();

	bool ITutorialController.TutorialInProgress()
	{
		if (_tutorialStatus.inProgress)
		{
			return true;
		}
		return false;
	}

	public void HandleCreateGameObjectWithInject(IPlaymakerCommandInputParameters commandParams)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		string inputParameters = commandParams.GetInputParameters<string>();
		FsmGameObject inputParameters2 = commandParams.GetInputParameters<FsmGameObject>();
		GameObject val = gameObjectFactory.Build(inputParameters);
		val.get_transform().set_parent(null);
		val.get_transform().set_position(commandParams.GetInputParameters<Vector3>());
		val.get_transform().set_rotation(commandParams.GetInputParameters<Quaternion>());
		inputParameters2.set_Value(val);
	}

	public abstract void ActivateLoadingGUI();

	public abstract void DeactivateLoadingGUI();

	protected abstract void RegisterContextSpecificCommandHandlers(Action<Action<IPlaymakerCommandInputParameters>, Type> RegisterPlayMakerCommandHandlerAction);

	protected abstract void RegisterContextSpecificRequestHandlers(Action<Type, Action<IPlayMakerDataRequest>> RegisterPlayMakerRequestHandler);

	private void HandleSendAnalyticsEventCommandExecution(IPlaymakerCommandInputParameters inputParameters)
	{
	}

	private void HandleToggleFunctionalityCommandExecution(IPlaymakerCommandInputParameters inputParameters)
	{
		ToggleFunctionalityNodeType inputParameters2 = inputParameters.GetInputParameters<ToggleFunctionalityNodeType>();
		bool inputParameters3 = inputParameters.GetInputParameters<bool>();
		switch (inputParameters2)
		{
		case ToggleFunctionalityNodeType.DEPRECATED_CanAccessForgeAndRecyclerWithKeyboard:
			break;
		case ToggleFunctionalityNodeType.RejectKeyboardInputByDefault:
			inputActionMask.RejectAllInputByDefault(inputParameters3);
			break;
		case ToggleFunctionalityNodeType.CanUseMirrorModeWithKeyboard:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.EditingInputAxis, 2);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.EditingInputAxis, 2);
			}
			break;
		case ToggleFunctionalityNodeType.CanShiftRobotPositionWithKeyboard:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.EditingInputAxis, 4);
				inputActionMask.AcceptInputAction(UserInputCategory.EditingInputAxis, 5);
				inputActionMask.AcceptInputAction(UserInputCategory.EditingInputAxis, 6);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.EditingInputAxis, 4);
				inputActionMask.RejectInputAction(UserInputCategory.EditingInputAxis, 5);
				inputActionMask.RejectInputAction(UserInputCategory.EditingInputAxis, 6);
			}
			break;
		case ToggleFunctionalityNodeType.CanShowAdvancedBuildInfo:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.EditingInputAxis, 13);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.EditingInputAxis, 13);
			}
			break;
		case ToggleFunctionalityNodeType.CanDeleteCubesWithRMB:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.BuildModeInputAxis, 1);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.BuildModeInputAxis, 1);
			}
			break;
		case ToggleFunctionalityNodeType.CanPlaceCubesWithLMB:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.BuildModeInputAxis, 0);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.BuildModeInputAxis, 0);
			}
			break;
		case ToggleFunctionalityNodeType.CanOpenInventoryWithKeyboard:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.GUIShortcutInputAxis, 1);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.GUIShortcutInputAxis, 1);
			}
			break;
		case ToggleFunctionalityNodeType.CanCloseAnythingWithTheEscapeKey:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.GUIShortcutInputAxis, 0);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.GUIShortcutInputAxis, 0);
			}
			break;
		case ToggleFunctionalityNodeType.CanGoToTestZoneWithKeyboard:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.WorldSwitchingInputAxis, 1);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.WorldSwitchingInputAxis, 1);
			}
			break;
		case ToggleFunctionalityNodeType.CanGoToPracticeModeWithKeyboard:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.WorldSwitchingInputAxis, 0);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.WorldSwitchingInputAxis, 0);
			}
			break;
		case ToggleFunctionalityNodeType.CanGoToAnyMultiplayerGameWithKeyboard:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.WorldSwitchingInputAxis, 4);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.WorldSwitchingInputAxis, 4);
			}
			break;
		case ToggleFunctionalityNodeType.CanUseTheGarageSocialOrRobotShopGUIShortcutKeys:
			if (inputParameters3)
			{
				inputActionMask.AcceptInputAction(UserInputCategory.GUIShortcutInputAxis, 4);
			}
			else
			{
				inputActionMask.RejectInputAction(UserInputCategory.GUIShortcutInputAxis, 4);
			}
			break;
		case ToggleFunctionalityNodeType.FiringDisabledInTestMode:
			if (inputParameters3)
			{
				inputActionMask.RejectInputAction(UserInputCategory.SimulationInputAxis, 0);
			}
			else
			{
				inputActionMask.AcceptInputAction(UserInputCategory.SimulationInputAxis, 0);
			}
			break;
		}
	}

	private void HandleHideTutorialDialogCommandExecution(IPlaymakerCommandInputParameters inputParams)
	{
		TutorialMessageData tutorialMessageData = new TutorialMessageData(string.Empty, 0f, hide_: true);
		_tutorialTipObserveable.Dispatch(ref tutorialMessageData);
	}

	private void HandleShowTutorialCubeGunPickUpCommandExecution(IPlaymakerCommandInputParameters inputParams)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = gameObjectFactory.Build(inputParams.GetInputParameters<GameObject>());
		val.get_transform().set_position(inputParams.GetInputParameters<FsmVector3>().get_Value());
		val.get_transform().set_rotation(inputParams.GetInputParameters<FsmQuaternion>().get_Value());
		inputParams.GetInputParameters<FsmGameObject>().set_Value(val);
		TutorialCubeGunPickupView component = val.GetComponent<TutorialCubeGunPickupView>();
		TutorialCubeGunPickupPresenter tutorialCubeGunPickupPresenter = new TutorialCubeGunPickupPresenter(loadingIconPresenter, serviceRequestFactory);
		tutorialCubeGunPickupPresenter.RegisterView(component);
	}

	private void HandleShowTutorialDialogCommandExecution(IPlaymakerCommandInputParameters inputParams)
	{
		string inputParameters = inputParams.GetInputParameters<string>();
		int inputParameters2 = inputParams.GetInputParameters<int>();
		TutorialMessageData tutorialMessageData = new TutorialMessageData(inputParameters, inputParameters2, hide_: false);
		_tutorialTipObserveable.Dispatch(ref tutorialMessageData);
	}

	private void HandleReloadTutorialRobotRequest(IPlayMakerDataRequest dataProvided)
	{
		garage.LoadAndBuildRobotInMothership(delegate
		{
			dataProvided.AssignResults(new ReloadTutorialRobotPlaymakerRequestReturn());
			dataProvided.Execute();
		});
	}

	private void HandleTestModeCheckRequest(IPlayMakerDataRequest dataProvided)
	{
		bool flag = false;
		flag = ((WorldSwitching.GetGameModeType() == GameModeType.TestMode) ? true : false);
		dataProvided.AssignResults(new CheckInTestModeContextRequestReturn(flag));
		dataProvided.Execute();
	}

	private void HandleResetTutorialRobotRequest(IPlayMakerDataRequest dataProvided)
	{
		ActivateLoadingGUI();
		ResetTutorialRobotRequestInputParameters inputParameters = dataProvided.GetInputParameters<ResetTutorialRobotRequestInputParameters>();
		int stage = inputParameters.stage;
		IResetTutorialRobotRequest resetTutorialRobotRequest = serviceFactory.Create<IResetTutorialRobotRequest, ResetTutorialRobotDependancy>(new ResetTutorialRobotDependancy(stage));
		resetTutorialRobotRequest.SetAnswer(new ServiceAnswer<uint>(delegate
		{
			DeactivateLoadingGUI();
			dataProvided.AssignResults(new ResetTutorialRobotPlaymakerRequestReturn());
			dataProvided.Execute();
		}, delegate(ServiceBehaviour behaviour)
		{
			HandleServiceRequestFailed(behaviour, "strTutorialResetRobotRequestFailed");
		}));
		resetTutorialRobotRequest.Execute();
	}

	private void HandleGetTutorialStageRequest(IPlayMakerDataRequest dataProvided)
	{
		ActivateLoadingGUI();
		ILoadTutorialStageRequest loadTutorialStageRequest = serviceFactory.Create<ILoadTutorialStageRequest>();
		loadTutorialStageRequest.SetAnswer(new ServiceAnswer<LoadTutorialStageData>(delegate(LoadTutorialStageData data)
		{
			DeactivateLoadingGUI();
			dataProvided.AssignResults(new GetTutorialStagePlaymakerRequestResult(data.stage));
			dataProvided.Execute();
		}, delegate(ServiceBehaviour behaviour)
		{
			HandleServiceRequestFailed(behaviour, "strLoadTutorialStatusRequestFailed");
		}));
		loadTutorialStageRequest.Execute();
	}

	private void HandleSaveTutorialStageRequest(IPlayMakerDataRequest dataProvided)
	{
		SaveTutorialStagePlaymakerRequestInputParameters inputParameters = dataProvided.GetInputParameters<SaveTutorialStagePlaymakerRequestInputParameters>();
		ActivateLoadingGUI();
		IUpdateTutorialStageRequest updateTutorialStageRequest = serviceFactory.Create<IUpdateTutorialStageRequest>();
		updateTutorialStageRequest.Inject(new UpdateTutorialStageData(inputParameters.stage));
		updateTutorialStageRequest.SetAnswer(new ServiceAnswer<bool>(delegate
		{
			DeactivateLoadingGUI();
			dataProvided.AssignResults(new SaveTutorialStagePlaymakerRequestReturn());
			dataProvided.Execute();
		}, delegate(ServiceBehaviour behaviour)
		{
			HandleServiceRequestFailed(behaviour, "strUpdateTutorialStatusRequestFailed");
		}));
		updateTutorialStageRequest.Execute();
	}

	private void HandleServiceRequestFailed(ServiceBehaviour behaviour, string strErrorTitle)
	{
		DeactivateLoadingGUI();
		Console.Log("Service request call to " + strErrorTitle + " failed, aborting application");
		throw new TutorialServiceException(StringTableBase<StringTable>.Instance.GetString("strTutorialServiceRequestFailed"));
	}
}
