using Authentication;
using Mothership;
using Mothership.GarageSkins;
using Robocraft.GUI.Iteration2;
using Services.Analytics;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

internal sealed class GaragePresenter : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
{
	private Action _runWhenGarageLoaded = delegate
	{
	};

	private readonly Dictionary<int, GarageSlotDependency> _garageSlots = new Dictionary<int, GarageSlotDependency>();

	private readonly FasterList<GarageSlotDependency> _garageSlotsForSunkCubes = new FasterList<GarageSlotDependency>();

	private GarageSlotEdit _garageSlotEdit;

	private uint _newGarageSlotLimit;

	private bool _isGarageSlotLimitLoaded;

	private bool _isGarageDataLoaded;

	private bool _initialInventoryLoaded;

	private IGenericDialog _askTutorialDialog;

	private GarageView garageView;

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	public IMachineMap machineMap
	{
		private get;
		set;
	}

	[Inject]
	public ICubeInventory cubeInventory
	{
		private get;
		set;
	}

	[Inject]
	public IGUIInputControllerMothership guiInputController
	{
		private get;
		set;
	}

	[Inject]
	public IMachineBuilder machineBuilder
	{
		private get;
		set;
	}

	[Inject]
	public ICommandFactory commandFactory
	{
		private get;
		set;
	}

	[Inject]
	public ICPUPower cpuPower
	{
		private get;
		set;
	}

	[Inject]
	public GarageSlotOrderPresenter slotOrderPresenter
	{
		private get;
		set;
	}

	[Inject]
	public GarageSlotsPresenter garageSlotsPresenter
	{
		private get;
		set;
	}

	[Inject]
	public MachineEditorBuilder editorMachineBuilder
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
	public GarageChangedObservable garageObservervable
	{
		get;
		set;
	}

	[Inject]
	public GarageBaySkinSelectedObserver garageBaySkinSelectedObserver
	{
		get;
		set;
	}

	[Inject]
	public GarageExtraButtonsPresenter garageExtraButtonsPresenter
	{
		private get;
		set;
	}

	[Inject]
	public IAnalyticsRequestFactory analyticsRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	public IGUIInputControllerMothership inputController
	{
		private get;
		set;
	}

	[Inject]
	public RobotShopSubmissionController submissionController
	{
		private get;
		set;
	}

	public bool CurrentSlotCanBeRated
	{
		get
		{
			return _garageSlots[(int)currentGarageSlot].canBeRated;
		}
		set
		{
			_garageSlots[(int)currentGarageSlot].canBeRated = value;
		}
	}

	public string CurrentRobotName => GetCurrentName();

	public UniqueSlotIdentifier CurrentRobotIdentifier => _garageSlots[(int)currentGarageSlot].uniqueSlotId;

	public int CurrentRobotMastery => _garageSlots[(int)currentGarageSlot].masteryLevel;

	public uint CurrentTotalRobotCPU => _garageSlots[(int)currentGarageSlot].totalRobotCPU;

	public uint CurrentTotalRobotRanking => _garageSlots[(int)currentGarageSlot].totalRobotRanking;

	public FasterList<ItemCategory> CurrentMovementCategories => _garageSlots[(int)currentGarageSlot].movementCategories;

	public int GarageSlotCount => _garageSlots.Count;

	public int EditableGarageSlotCount => _garageSlots.Count;

	public uint initialSlot
	{
		get;
		private set;
	}

	public uint currentGarageSlot
	{
		get;
		private set;
	}

	public bool isReady => _isGarageDataLoaded && _isGarageSlotLimitLoaded && garageView != null;

	public bool isBusyBuilding
	{
		get;
		private set;
	}

	public event Action<string> OnCurrentGarageNameChange = delegate
	{
	};

	public event Action<GarageSlotDependency> OnCurrentGarageSlotChanged = delegate
	{
	};

	public GaragePresenter()
	{
		isBusyBuilding = false;
	}

	public unsafe void OnFrameworkInitialized()
	{
		cubeInventory.RegisterInventoryLoadedCallback(OnInventoryLoadedCallback);
		garageBaySkinSelectedObserver.AddAction(new ObserverAction<RobotBaySkinDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public unsafe void OnFrameworkDestroyed()
	{
		cpuPower.UnregisterOnCPULoadChanged(UpdateRobotCpu);
		cubeInventory.DeRegisterInventoryLoadedCallback(OnInventoryLoadedCallback);
		garageBaySkinSelectedObserver.RemoveAction(new ObserverAction<RobotBaySkinDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public void SetView(GarageView view)
	{
		garageView = view;
		_askTutorialDialog = GenericWidgetFactory.BuildDialogExisting(view.askTutorialDialogTemplate, inputController);
	}

	public void ResetView()
	{
		garageView = null;
		_askTutorialDialog = null;
	}

	public void AddCurrentGarageNameChangedListener(Action<string> onChangeFunction)
	{
		OnCurrentGarageNameChange += onChangeFunction;
		if (_isGarageDataLoaded)
		{
			onChangeFunction(GetCurrentName());
		}
	}

	public void RemoveCurrentGarageNameChangedListener(Action<string> onChangeFunction)
	{
		OnCurrentGarageNameChange -= onChangeFunction;
	}

	public void ClearCurrentSlotCrfid()
	{
		_garageSlots[(int)currentGarageSlot].crfId = 0u;
	}

	public GUIShowResult Show()
	{
		if (!isReady)
		{
			return GUIShowResult.NotShowed;
		}
		initialSlot = currentGarageSlot;
		garageSlotsPresenter.ShowSlots();
		UpdateRobotData();
		garageExtraButtonsPresenter.Show();
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		if (!IsActive())
		{
			return false;
		}
		bool flag = false;
		if (initialSlot != currentGarageSlot)
		{
			currentGarageSlot = initialSlot;
			flag = true;
		}
		garageSlotsPresenter.HideSlots();
		if (flag)
		{
			machineBuilder.RemoveAllCubesGarage();
			LoadAndBuildRobot();
			UpdateCurrentName();
		}
		return true;
	}

	public void LoadGarageData()
	{
		_isGarageDataLoaded = false;
		TaskRunner.get_Instance().Run(LoadGarages());
	}

	public IEnumerator RefreshGarageData()
	{
		LoadGarageDataRequestResponse response = null;
		yield return LoadAllGarages("GarageRefreshLoadingIcon", delegate(LoadGarageDataRequestResponse result)
		{
			response = result;
		});
		if (response != null)
		{
			initialSlot = response.currentGarageSlot;
			OnGarageDataLoaded(response);
		}
	}

	private IEnumerator LoadAllGarages(string loadingIcon, Action<LoadGarageDataRequestResponse> onComplete)
	{
		loadingIconPresenter.NotifyLoading(loadingIcon);
		yield return cpuPower.IsLoadedEnumerator();
		ILoadGarageDataRequest loadGarageRequest = serviceFactory.Create<ILoadGarageDataRequest>();
		TaskService<LoadGarageDataRequestResponse> loadGarageTask = new TaskService<LoadGarageDataRequestResponse>(loadGarageRequest);
		yield return new HandleTaskServiceWithError(loadGarageTask, delegate
		{
			loadingIconPresenter.NotifyLoading(loadingIcon);
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone(loadingIcon);
		}).GetEnumerator();
		LoadGarageDataRequestResponse result = loadGarageTask.result;
		onComplete(result);
		loadingIconPresenter.NotifyLoadingDone(loadingIcon);
	}

	public void ShowGarageSlots()
	{
		slotOrderPresenter.ShowOrderedGarageSlots(_garageSlots, currentGarageSlot, _newGarageSlotLimit);
		UpdateRobotData();
		this.OnCurrentGarageNameChange(GetCurrentName());
	}

	public void HandleSlotDeleted(uint deletedSlot)
	{
		slotOrderPresenter.GarageSlotDeletedSuccess(deletedSlot);
		ShowGarageSlots();
	}

	public void SetGarageSlot(uint slot)
	{
		if (currentGarageSlot != slot)
		{
			currentGarageSlot = slot;
		}
	}

	public IEnumerator AskForTutorial(Action<GenericDialogChoice> onChoice)
	{
		yield return _askTutorialDialog.Prompt(onChoice);
	}

	public void AddNewGarageSlot(GarageSlotDependency newGarageSlot)
	{
		_garageSlots.Add((int)newGarageSlot.garageSlot, newGarageSlot);
		ShowGarageSlots();
		SetGarageSlot(newGarageSlot.garageSlot);
		UpdateCurrentName();
		LoadAndBuildRobot();
		SelectCurrentGarageSlot();
	}

	public void SunkCubesInTheInventory()
	{
		GarageSlotDependency garageSlotDependency = _garageSlots[(int)currentGarageSlot];
		garageObservervable.Dispatch(ref garageSlotDependency);
	}

	public bool IsActive()
	{
		return garageView.get_gameObject().get_activeSelf();
	}

	public void UpdateCurrentName()
	{
		SetCurrentName(GetCurrentName());
	}

	public bool SetCurrentName(string name)
	{
		int currentGarageSlot = (int)this.currentGarageSlot;
		bool flag = !_garageSlots[currentGarageSlot].name.Equals(name);
		_garageSlots[currentGarageSlot].name = name;
		if (flag)
		{
			garageSlotsPresenter.SetCurrentGarageData(this.currentGarageSlot, _garageSlots[currentGarageSlot]);
		}
		this.OnCurrentGarageNameChange(name);
		return flag;
	}

	public void BuildInitialRobot()
	{
		isBusyBuilding = true;
		loadingIconPresenter.NotifyLoading("GarageLoadingIcon");
		BuildGarageSlotMachine(currentGarageSlot, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("GarageLoadingIcon");
			isBusyBuilding = false;
			UpdateRobotData();
			this.OnCurrentGarageSlotChanged(_garageSlots[(int)currentGarageSlot]);
		}, initialMachine: true);
	}

	public void LoadAndBuildRobot()
	{
		if (isReady)
		{
			isBusyBuilding = true;
			loadingIconPresenter.NotifyLoading("GarageLoadingIcon");
			BuildGarageSlotMachine(currentGarageSlot, delegate
			{
				isBusyBuilding = false;
				loadingIconPresenter.NotifyLoadingDone("GarageLoadingIcon");
				SunkCubesInTheInventory();
				UpdateRobotData();
				this.OnCurrentGarageSlotChanged(_garageSlots[(int)currentGarageSlot]);
			});
		}
	}

	public void LoadAndBuildRobotInMothership(Action onCompleteCallback = null)
	{
		isBusyBuilding = true;
		loadingIconPresenter.NotifyLoading("GarageLoadingIcon");
		BuildGarageSlotMachine(currentGarageSlot, delegate
		{
			isBusyBuilding = false;
			loadingIconPresenter.NotifyLoadingDone("GarageLoadingIcon");
			SunkCubesInTheInventory();
			garageSlotsPresenter.SetCurrentGarageData(currentGarageSlot, _garageSlots[(int)currentGarageSlot]);
			SafeEvent.SafeRaise(onCompleteCallback);
			this.OnCurrentGarageSlotChanged(_garageSlots[(int)currentGarageSlot]);
		});
	}

	public void BuildRobotInMothershipWithoutLoadingScreen()
	{
		BuildGarageSlotMachine(currentGarageSlot, delegate
		{
			SunkCubesInTheInventory();
			garageSlotsPresenter.SetCurrentGarageData(currentGarageSlot, _garageSlots[(int)currentGarageSlot]);
			this.OnCurrentGarageSlotChanged(_garageSlots[(int)currentGarageSlot]);
		});
	}

	public void SelectCurrentGarageSlot()
	{
		guiInputController.ShowScreen(GuiScreens.Garage);
		initialSlot = currentGarageSlot;
		loadingIconPresenter.NotifyLoading("GarageLoadingIcon");
		ISetSelectedRobotRequest setSelectedRobotRequest = serviceFactory.Create<ISetSelectedRobotRequest, SetSelectedRobotDependency>(new SetSelectedRobotDependency(currentGarageSlot));
		setSelectedRobotRequest.SetAnswer(new ServiceAnswer<SetSelectedRobotDependency>(delegate
		{
			loadingIconPresenter.NotifyLoadingDone("GarageLoadingIcon");
			garageSlotsPresenter.SetCurrentGarageData(currentGarageSlot, _garageSlots[(int)currentGarageSlot]);
		}, OnGarageOperationFailed));
		setSelectedRobotRequest.Execute();
	}

	public IEnumerator RefreshCurrentRobotDataEnumerator()
	{
		yield return null;
		UpdateRobotData();
	}

	public void HandleUIMessage(object message)
	{
		if (message is ButtonType)
		{
			switch ((ButtonType)message)
			{
			case ButtonType.Upload:
				OnUploadButtonPressed();
				break;
			case ButtonType.CustomiseScreen:
				OnCustomiseButtonPressed();
				break;
			case ButtonType.Dismantle:
				OnDismantleButtonPressed();
				break;
			case ButtonType.Edit:
				OnEditRobotButtonClicked();
				break;
			case ButtonType.Test:
				OnTestRobotButtonClicked();
				break;
			case ButtonType.CopyRobot:
				OnCopyRobotButtonPressed();
				break;
			case ButtonType.OpenNewRobotOptions:
				OnOpenNewRobotButtonPressed();
				break;
			}
		}
		else if (message is uint)
		{
			garageSlotsPresenter.SwitchSlot((uint)message);
		}
	}

	private void OnEditRobotButtonClicked()
	{
		SwitchWorldDependency dependency = new SwitchWorldDependency("RC_BuildMode", _fastSwitch: false);
		commandFactory.Build<SwitchToBuildModeCommand>().Inject(dependency).Execute();
	}

	private void OnTestRobotButtonClicked()
	{
		SwitchWorldDependency dependency = new SwitchWorldDependency("TestRobot", isRanked_: false, isBrawl_: false, isCustomGame_: false, GameModeType.TestMode);
		commandFactory.Build<SwitchToTestPlanetCommand>().Inject(dependency).Execute();
	}

	private void OnUploadButtonPressed()
	{
		submissionController.StartUploadRobot();
	}

	private void OnDismantleButtonPressed()
	{
		commandFactory.Build<DeleteGarageSlotCommand>().Inject(currentGarageSlot).Execute();
	}

	private void OnCopyRobotButtonPressed()
	{
		commandFactory.Build<CopyRobotFromGarageCommand>().Inject((int)currentGarageSlot).Execute();
	}

	private void OnCustomiseButtonPressed()
	{
		commandFactory.Build<ToggleConfigurationScreenCommand>().Execute();
	}

	private void OnOpenNewRobotButtonPressed()
	{
		commandFactory.Build<OpenNewGarageCommand>().Execute();
	}

	private void BuildGarageSlotMachine(uint garageSlot, Action onSuccess, bool initialMachine = false)
	{
		TaskRunner.get_Instance().Run(BuildGarageSlotMachineAsTask(garageSlot, onSuccess, initialMachine));
	}

	private IEnumerator BuildGarageSlotMachineAsTask(uint garageSlot, Action onSuccess, bool initialMachine)
	{
		ILoadMachineRequest loadMachineRequest = serviceFactory.Create<ILoadMachineRequest>();
		if (!initialMachine)
		{
			loadMachineRequest.Inject(garageSlot);
		}
		TaskService<LoadMachineResult> loadMachineTask = new TaskService<LoadMachineResult>(loadMachineRequest);
		yield return loadMachineTask;
		if (!loadMachineTask.succeeded)
		{
			OnGarageOperationFailed(loadMachineTask.behaviour);
			yield break;
		}
		LoadMachineResult loadMachineResult = loadMachineTask.result;
		ILoadMachineColorMapRequest loadRobotColorInfo = serviceFactory.Create<ILoadMachineColorMapRequest, LoadMachineColorMapDependancy>(new LoadMachineColorMapDependancy(User.Username, garageSlot));
		TaskService<byte[]> loadRobotColorTask = new TaskService<byte[]>(loadRobotColorInfo);
		yield return loadRobotColorTask;
		if (!loadRobotColorTask.succeeded)
		{
			OnGarageOperationFailed(loadRobotColorTask.behaviour);
			yield break;
		}
		loadMachineResult.model.SetColorData(loadRobotColorTask.result);
		yield return cubeInventory.RefreshAndWait();
		try
		{
			machineBuilder.RemoveObsoleteCubes(loadMachineResult.model);
			machineBuilder.RemoveAllCubesGarage();
			editorMachineBuilder.BuildMachineInGarage(loadMachineResult.model, onSuccess, garageSlot);
		}
		catch (Exception ex)
		{
			onSuccess();
			Console.LogException(ex);
		}
	}

	private IEnumerator LoadGarages()
	{
		yield return LoadAllGarages("GarageLoadingIcon", delegate(LoadGarageDataRequestResponse data)
		{
			OnGarageDataLoaded(data);
			_003CLoadGarages_003Ec__Iterator5 garagePresenter = this;
			((GaragePresenter)garagePresenter)._runWhenGarageLoaded = (Action)Delegate.Combine(((GaragePresenter)garagePresenter)._runWhenGarageLoaded, (Action)delegate
			{
				_garageSlotEdit = new GarageSlotEdit((GaragePresenter)this, serviceFactory, loadingIconPresenter, analyticsRequestFactory);
				garageSlotsPresenter.garageSlotEdit = _garageSlotEdit;
				ShowGarageSlots();
			});
			RunGarageEventIfLoaded();
		});
	}

	private void OnInventoryLoadedCallback()
	{
		if (!_initialInventoryLoaded)
		{
			_initialInventoryLoaded = true;
			LoadGarageData();
			LoadGarageSlotLimit();
		}
	}

	private void LoadGarageSlotLimit()
	{
		ILoadGarageSlotLimitRequest loadGarageSlotLimitRequest = serviceFactory.Create<ILoadGarageSlotLimitRequest>();
		loadGarageSlotLimitRequest.SetAnswer(new ServiceAnswer<uint>(OnGarageSlotLimitLoaded, OnGarageOperationFailed));
		loadGarageSlotLimitRequest.Execute();
	}

	private void OnGarageDataLoaded(LoadGarageDataRequestResponse data)
	{
		currentGarageSlot = data.currentGarageSlot;
		_garageSlots.Clear();
		_garageSlotsForSunkCubes.Clear();
		for (int i = 0; i < data.garageSlots.get_Count(); i++)
		{
			GarageSlotDependency garageSlotDependency = data.garageSlots.get_Item(i);
			_garageSlotsForSunkCubes.Add(garageSlotDependency);
			if (!garageSlotDependency.tutorialRobot || currentGarageSlot == garageSlotDependency.garageSlot)
			{
				_garageSlots.Add((int)garageSlotDependency.garageSlot, garageSlotDependency);
			}
		}
		if (!_garageSlots.ContainsKey((int)currentGarageSlot))
		{
			using (Dictionary<int, GarageSlotDependency>.Enumerator enumerator = _garageSlots.GetEnumerator())
			{
				enumerator.MoveNext();
				currentGarageSlot = enumerator.Current.Value.garageSlot;
			}
		}
		SunkCubesInTheInventory();
		cpuPower.RegisterOnCPULoadChanged(UpdateRobotCpu);
		_isGarageDataLoaded = true;
		slotOrderPresenter.OnGarageDataLoaded(data.garageSlotOrder);
	}

	private void UpdateRobotCpu(uint cpu)
	{
		GarageSlotDependency garageSlotDependency = _garageSlots[(int)currentGarageSlot];
		garageSlotDependency.totalRobotCPU = cpuPower.TotalCPUCurrentRobot;
		garageSlotDependency.totalCosmeticCPU = cpuPower.TotalCosmeticCPUCurrentRobot;
	}

	private void OnGarageSlotLimitLoaded(uint limit)
	{
		_isGarageSlotLimitLoaded = true;
		_newGarageSlotLimit = limit;
		RunGarageEventIfLoaded();
	}

	private void RunGarageEventIfLoaded()
	{
		if (isReady)
		{
			_runWhenGarageLoaded();
			_runWhenGarageLoaded = delegate
			{
			};
		}
	}

	private void UpdateRobotData()
	{
		int currentGarageSlot = (int)this.currentGarageSlot;
		HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
		uint num = 0u;
		foreach (InstantiatedCube item in allInstantiatedCubes)
		{
			num = (uint)((int)num + item.persistentCubeData.cubeRanking);
		}
		_garageSlots[currentGarageSlot].totalRobotRanking = num;
		_garageSlots[currentGarageSlot].numberCubes = (uint)allInstantiatedCubes.Count;
		garageSlotsPresenter.SetCurrentGarageData(this.currentGarageSlot, _garageSlots[currentGarageSlot]);
		UpdateCurrentName();
	}

	private void OnGarageOperationFailed(ServiceBehaviour behaviour)
	{
		loadingIconPresenter.NotifyLoadingDone("GarageLoadingIcon");
		ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
		{
			loadingIconPresenter.NotifyLoading("GarageLoadingIcon");
			behaviour.MainBehaviour();
		}, behaviour.Alternative));
	}

	private string GetCurrentName()
	{
		GarageSlotDependency garageSlotDependency = _garageSlots[(int)currentGarageSlot];
		if (garageSlotDependency.tutorialRobot)
		{
			return StringTableBase<StringTable>.Instance.GetString("strTutorialRobotName");
		}
		return garageSlotDependency.name;
	}

	private void SaveSelectedGarageSkin(ref RobotBaySkinDependency selectedBaySkin)
	{
		_garageSlots[selectedBaySkin.SlotID].baySkinID = selectedBaySkin.BaySkinID;
	}
}
