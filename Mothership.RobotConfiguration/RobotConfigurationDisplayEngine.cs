using Mothership.GarageSkins;
using Services.Web.Photon;
using Simulation;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership.RobotConfiguration
{
	internal class RobotConfigurationDisplayEngine : MultiEntityViewsEngine<RobotConfigurationDisplayEntityView, RobotConfigListDisplayEntityView, RobotConfigListItemDisplayEntityView>, IGUIDisplay, IQueryingEntityViewEngine, IComponent, IEngine
	{
		public struct ListItemDisplayData
		{
			public string identifier;

			public string strKey;

			public string imagePath;

			public bool isSelected;

			public ListGroupSelection itemCategory;

			public ListItemDisplayData(string identifier_, string imagePath_, string strkey_, bool isSelected_, ListGroupSelection itemCategory_)
			{
				identifier = identifier_;
				imagePath = imagePath_;
				strKey = strkey_;
				isSelected = isSelected_;
				itemCategory = itemCategory_;
			}
		}

		private IRobotConfigurationShowHideComponent _displayScreenComponent;

		private IRobotControlCustomisationsComponent _customisationOptions;

		private RobotConfigurationDataSource _dataSource;

		private IRobotConfigListDisplayComponent _listComponent;

		private ControlSettings _currentControlSettings;

		private uint _currentGarageSlotIndex;

		private UniqueSlotIdentifier _currentRobotUniqueIdentifier;

		private string _currentBaySkin;

		private string _currentSpawnEffect;

		private string _currentDeathEffect;

		private ListGroupSelection _currentListGroupSelection;

		private RobotConfigurationGUIFactory _factory;

		private LoadingIconPresenter _loadingIconPresenter;

		private ICommandFactory _commandFactory;

		private IServiceRequestFactory _serviceFactory;

		private GarageBaySkinSelectedObservable _garageBaySkinSelectedObservable;

		public GuiScreens screenType => GuiScreens.RobotConfigurationScreen;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyGUINoSwitching;

		public bool isScreenBlurred => false;

		public bool hasBackground => true;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public RobotConfigurationDisplayEngine(RobotConfigurationDataSource dataSource, RobotConfigurationGUIFactory factory, LoadingIconPresenter loadingIconPresenter, ICommandFactory commandFactory, IServiceRequestFactory serviceFactory, GarageBaySkinSelectedObservable garageBaySkinSelectedObservable)
		{
			_dataSource = dataSource;
			_factory = factory;
			_loadingIconPresenter = loadingIconPresenter;
			_commandFactory = commandFactory;
			_serviceFactory = serviceFactory;
			_garageBaySkinSelectedObservable = garageBaySkinSelectedObservable;
		}

		public void EnableBackground(bool enable)
		{
		}

		public void Ready()
		{
		}

		public GUIShowResult Show()
		{
			_displayScreenComponent.Show();
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SetupDataFromGarageBay);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_displayScreenComponent.Hide();
			return true;
		}

		public bool IsActive()
		{
			if (_displayScreenComponent != null)
			{
				return _displayScreenComponent.gameObject.get_activeSelf();
			}
			return false;
		}

		private IEnumerator SetupDataFromGarageBay()
		{
			IEnumerator loadDataSource = _dataSource.LoadData();
			yield return loadDataSource;
			GetRobotBayCustomisationsResponse result = (GetRobotBayCustomisationsResponse)loadDataSource.Current;
			_currentBaySkin = result.BaySkinId;
			_currentSpawnEffect = result.SpawnEffectId;
			_currentDeathEffect = result.DeathEffectId;
			ToggleListGroupSelection(ListGroupSelection.MothershipBaySkin);
			BuildNewListItemsForCategory(ListGroupSelection.MothershipBaySkin);
			BuildNewListItemsForCategory(ListGroupSelection.SpawnEffects);
			BuildNewListItemsForCategory(ListGroupSelection.DeathEffects);
			ControlSettings _currentControlSettings = default(ControlSettings);
			_loadingIconPresenter.NotifyLoading("RobotConfiguration");
			TaskService<GetRobotControlsResult> getCurrentRobotControlsRequestasTaskService = _serviceFactory.Create<IGetCurrentRobotControlsRequest>().AsTask();
			yield return getCurrentRobotControlsRequestasTaskService;
			_loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			if (getCurrentRobotControlsRequestasTaskService.succeeded)
			{
				GetRobotControlsResult result2 = getCurrentRobotControlsRequestasTaskService.result;
				_currentControlSettings = result2.controls;
				_currentGarageSlotIndex = result2.garageSlotIndex;
			}
			else
			{
				HandleServiceError(getCurrentRobotControlsRequestasTaskService.behaviour);
			}
			_customisationOptions.cameraRelativeTiltCheckbox = _currentControlSettings.verticalStrafing;
			_customisationOptions.sideWaysDrivingCheckbox = _currentControlSettings.sidewaysDriving;
			_customisationOptions.tankTracksTurnToFaceCheckbox = _currentControlSettings.tracksTurnOnSpot;
			_customisationOptions.controlTypeInDropDown = _currentControlSettings.controlType;
		}

		private void BuildNewListItemsForCategory(ListGroupSelection category)
		{
			List<CustomisationsEntry> list = null;
			List<CustomisationsEntry> list2 = null;
			switch (category)
			{
			case ListGroupSelection.MothershipBaySkin:
				list = _dataSource.BaySkinCustomisations;
				list2 = _dataSource.OwnedBaySkinCustomisations;
				break;
			case ListGroupSelection.SpawnEffects:
				list = _dataSource.SpawnEfffectCustomisations;
				list2 = _dataSource.OwnedSpawnEfffectCustomisations;
				break;
			case ListGroupSelection.DeathEffects:
				list = _dataSource.DeathEfffectCustomisations;
				list2 = _dataSource.OwnedDeathEfffectCustomisations;
				break;
			}
			int num = 0;
			RobotConfigListItemDisplayEntityView[] array = entityViewsDB.QueryEntityViewsAsArray<RobotConfigListItemDisplayEntityView>(ref num);
			List<string> list3 = new List<string>();
			for (int i = 0; i < num; i++)
			{
				list3.Add(array[i].setupComponent.identifier);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				if (list3.Contains(list2[j].id))
				{
					continue;
				}
				ListItemDisplayData entityData = default(ListItemDisplayData);
				for (int k = 0; k < list.Count; k++)
				{
					if (list[k].id == list2[j].id)
					{
						entityData = new ListItemDisplayData(list[k].id, list[k].previewImageName, list[k].localisedName, isSelected_: false, category);
					}
				}
				if (entityData.identifier != null)
				{
					_factory.BuildSingleItem(entityData, _listComponent, _currentListGroupSelection == entityData.itemCategory, SelectedItem);
				}
				else
				{
					Console.Log("Warning: player owns item: " + _dataSource.OwnedBaySkinCustomisations[j].id + " but this item is not defined.");
				}
			}
		}

		private void HandleServiceError(ServiceBehaviour behaviour)
		{
			_commandFactory.Build<ToggleConfigurationScreenCommand>().Execute();
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
			{
			}, delegate
			{
			});
			ErrorWindow.ShowErrorWindow(error);
		}

		protected override void Add(RobotConfigListDisplayEntityView entityView)
		{
			_listComponent = entityView.listComponent;
			TryToBuildUIList();
		}

		protected override void Remove(RobotConfigListDisplayEntityView entityView)
		{
			_listComponent = null;
		}

		protected override void Add(RobotConfigurationDisplayEntityView entityView)
		{
			_displayScreenComponent = entityView.robotConfigurationDisplayComponent;
			_customisationOptions = entityView.controlCustomisationComponent;
			_factory.Build(_displayScreenComponent.gameObject);
			entityView.dialogChoiceComponent.validatePressed.NotifyOnValueSet((Action<int, bool>)OnOKButtonPressed);
			entityView.dialogChoiceComponent.cancelPressed.NotifyOnValueSet((Action<int, bool>)OnCancelButtonPressed);
			entityView.controlCustomisationComponent.controlTypeDropDownChosen.NotifyOnValueSet((Action<int, ControlType>)OnControlsDropDownChanged);
			entityView.controlCustomisationComponent.cameraRelativeTiltCheckboxSet.NotifyOnValueSet((Action<int, bool>)OnCameraRelativeCheckboxSettingSet);
			entityView.controlCustomisationComponent.sideWaysDrivingCheckboxSet.NotifyOnValueSet((Action<int, bool>)OnSidewaysDrivingCheckboxSettingSet);
			entityView.controlCustomisationComponent.tankTracksTurnToFaceCheckboxSet.NotifyOnValueSet((Action<int, bool>)OnTankTracksTurnCheckboxSettingSet);
			entityView.controlCustomisationComponent.mothershipSkinTabPressed.NotifyOnValueSet((Action<int, bool>)delegate(int id, bool waspressed)
			{
				if (waspressed)
				{
					ToggleListGroupSelection(ListGroupSelection.MothershipBaySkin);
				}
			});
			entityView.controlCustomisationComponent.spawnEffectsTabPressed.NotifyOnValueSet((Action<int, bool>)delegate(int id, bool waspressed)
			{
				if (waspressed)
				{
					ToggleListGroupSelection(ListGroupSelection.SpawnEffects);
				}
			});
			entityView.controlCustomisationComponent.deathEffectsTabPressed.NotifyOnValueSet((Action<int, bool>)delegate(int id, bool waspressed)
			{
				if (waspressed)
				{
					ToggleListGroupSelection(ListGroupSelection.DeathEffects);
				}
			});
			TryToBuildUIList();
		}

		private void ToggleListGroupSelection(ListGroupSelection selection)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			_currentListGroupSelection = selection;
			_listComponent.listDisplayMode = selection;
			FasterListEnumerator<RobotConfigListItemDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<RobotConfigListItemDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RobotConfigListItemDisplayEntityView current = enumerator.get_Current();
					current.setupComponent.listTypeSelected = selection;
					switch (selection)
					{
					case ListGroupSelection.MothershipBaySkin:
						current.setupComponent.selected = (current.setupComponent.identifier == _currentBaySkin);
						break;
					case ListGroupSelection.SpawnEffects:
						current.setupComponent.selected = (current.setupComponent.identifier == _currentSpawnEffect);
						break;
					case ListGroupSelection.DeathEffects:
						current.setupComponent.selected = (current.setupComponent.identifier == _currentDeathEffect);
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private void TryToBuildUIList()
		{
			if (_listComponent != null && _displayScreenComponent != null)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAndBuildUIList);
			}
		}

		private IEnumerator LoadAndBuildUIList()
		{
			IEnumerator loadDataSource = _dataSource.LoadData();
			yield return loadDataSource;
			GetRobotBayCustomisationsResponse result = (GetRobotBayCustomisationsResponse)loadDataSource.Current;
			_currentBaySkin = result.BaySkinId;
			_currentSpawnEffect = result.SpawnEffectId;
			_currentDeathEffect = result.DeathEffectId;
			List<ListItemDisplayData> bays = new List<ListItemDisplayData>();
			foreach (CustomisationsEntry ownedBaySkinCustomisation in _dataSource.OwnedBaySkinCustomisations)
			{
				bays.Add(new ListItemDisplayData(isSelected_: _currentBaySkin == ownedBaySkinCustomisation.id, identifier_: ownedBaySkinCustomisation.id, imagePath_: ownedBaySkinCustomisation.previewImageName, strkey_: ownedBaySkinCustomisation.localisedName, itemCategory_: ListGroupSelection.MothershipBaySkin));
			}
			foreach (CustomisationsEntry ownedSpawnEfffectCustomisation in _dataSource.OwnedSpawnEfffectCustomisations)
			{
				bays.Add(new ListItemDisplayData(isSelected_: _currentSpawnEffect == ownedSpawnEfffectCustomisation.id, identifier_: ownedSpawnEfffectCustomisation.id, imagePath_: ownedSpawnEfffectCustomisation.previewImageName, strkey_: ownedSpawnEfffectCustomisation.localisedName, itemCategory_: ListGroupSelection.SpawnEffects));
			}
			foreach (CustomisationsEntry ownedDeathEfffectCustomisation in _dataSource.OwnedDeathEfffectCustomisations)
			{
				bays.Add(new ListItemDisplayData(isSelected_: _currentDeathEffect == ownedDeathEfffectCustomisation.id, identifier_: ownedDeathEfffectCustomisation.id, imagePath_: ownedDeathEfffectCustomisation.previewImageName, strkey_: ownedDeathEfffectCustomisation.localisedName, itemCategory_: ListGroupSelection.DeathEffects));
			}
			_factory.BuildList(bays, _listComponent, ListGroupSelection.MothershipBaySkin, SelectedItem);
		}

		private void SelectedItem(int buttonId, string SelectedData)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			RobotConfigListItemDisplayEntityView robotConfigListItemDisplayEntityView = entityViewsDB.QueryEntityView<RobotConfigListItemDisplayEntityView>(buttonId);
			ListGroupSelection listTypeSelected = robotConfigListItemDisplayEntityView.setupComponent.listTypeSelected;
			switch (listTypeSelected)
			{
			case ListGroupSelection.MothershipBaySkin:
				if (_currentBaySkin != SelectedData)
				{
					_currentBaySkin = SelectedData;
				}
				break;
			case ListGroupSelection.SpawnEffects:
				_currentSpawnEffect = SelectedData;
				break;
			case ListGroupSelection.DeathEffects:
				_currentDeathEffect = SelectedData;
				break;
			}
			FasterListEnumerator<RobotConfigListItemDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<RobotConfigListItemDisplayEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RobotConfigListItemDisplayEntityView current = enumerator.get_Current();
					if (current.get_ID() != buttonId && current.setupComponent.listTypeSelected == listTypeSelected)
					{
						current.setupComponent.selected = false;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		protected override void Remove(RobotConfigurationDisplayEntityView entityView)
		{
			_displayScreenComponent = null;
			entityView.dialogChoiceComponent.validatePressed.StopNotify((Action<int, bool>)OnOKButtonPressed);
			entityView.dialogChoiceComponent.cancelPressed.StopNotify((Action<int, bool>)OnOKButtonPressed);
			entityView.controlCustomisationComponent.controlTypeDropDownChosen.StopNotify((Action<int, ControlType>)OnControlsDropDownChanged);
			entityView.controlCustomisationComponent.cameraRelativeTiltCheckboxSet.StopNotify((Action<int, bool>)OnCameraRelativeCheckboxSettingSet);
			entityView.controlCustomisationComponent.sideWaysDrivingCheckboxSet.StopNotify((Action<int, bool>)OnSidewaysDrivingCheckboxSettingSet);
			entityView.controlCustomisationComponent.tankTracksTurnToFaceCheckboxSet.StopNotify((Action<int, bool>)OnTankTracksTurnCheckboxSettingSet);
		}

		private void OnOKButtonPressed(int sender, bool pressed)
		{
			if (pressed)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)SaveAndExit);
			}
		}

		private IEnumerator SaveAndExit()
		{
			_loadingIconPresenter.NotifyLoading("RobotConfiguration");
			ISetGarageSlotControlsRequest setGarageSlotControlsRequest = _serviceFactory.Create<ISetGarageSlotControlsRequest, GarageSlotControlsDependency>(new GarageSlotControlsDependency(_currentGarageSlotIndex, _currentControlSettings));
			TaskService setControlsTask = new TaskService<ControlSettings>(setGarageSlotControlsRequest);
			yield return new HandleTaskServiceWithError(setControlsTask, delegate
			{
				_loadingIconPresenter.NotifyLoading("RobotConfiguration");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			}).GetEnumerator();
			IGetGarageBayUniqueIdRequest getRobotUniqueIdRequest = _serviceFactory.Create<IGetGarageBayUniqueIdRequest>();
			TaskService<UniqueSlotIdentifier> getRobotUniqueIdTask = new TaskService<UniqueSlotIdentifier>(getRobotUniqueIdRequest);
			yield return new HandleTaskServiceWithError(getRobotUniqueIdTask, delegate
			{
				_loadingIconPresenter.NotifyLoading("RobotConfiguration");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			}).GetEnumerator();
			UniqueSlotIdentifier identifier = getRobotUniqueIdTask.result;
			SetRobotCustomisationDependency saveDependancy = new SetRobotCustomisationDependency(_currentGarageSlotIndex, identifier.ToString(), _currentBaySkin, _currentSpawnEffect, _currentDeathEffect);
			RobotBaySkinDependency dispatchedDependancy = new RobotBaySkinDependency
			{
				BaySkinID = _currentBaySkin,
				SlotID = (int)_currentGarageSlotIndex
			};
			_garageBaySkinSelectedObservable.Dispatch(ref dispatchedDependancy);
			ISetRobotCustomisationsRequest setRobotCustomisationsRequest = _serviceFactory.Create<ISetRobotCustomisationsRequest, SetRobotCustomisationDependency>(saveDependancy);
			TaskService setCustomisationTask = new TaskService<SetRobotCustomisationDependency>(setRobotCustomisationsRequest);
			yield return new HandleTaskServiceWithError(setCustomisationTask, delegate
			{
				if (setCustomisationTask.behaviour.errorCode == 146)
				{
					Console.Log("failed to set customisation for robot because you do not own the selected customisation");
				}
				else
				{
					Console.Log("error occured setting customisations");
				}
			}, delegate
			{
				Console.Log("error occured setting customisations");
			}).GetEnumerator();
			_loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			_commandFactory.Build<ToggleConfigurationScreenCommand>().Execute();
		}

		private void OnControlsDropDownChanged(int sender, ControlType controlType)
		{
			_currentControlSettings.controlType = controlType;
		}

		private void OnCameraRelativeCheckboxSettingSet(int sender, bool setting)
		{
			_currentControlSettings.verticalStrafing = setting;
		}

		private void OnSidewaysDrivingCheckboxSettingSet(int sender, bool setting)
		{
			_currentControlSettings.sidewaysDriving = setting;
		}

		private void OnTankTracksTurnCheckboxSettingSet(int sender, bool setting)
		{
			_currentControlSettings.tracksTurnOnSpot = setting;
		}

		private void OnCancelButtonPressed(int sender, bool pressed)
		{
			if (pressed)
			{
				_commandFactory.Build<ToggleConfigurationScreenCommand>().Execute();
			}
		}

		protected override void Add(RobotConfigListItemDisplayEntityView entityView)
		{
			entityView.setupComponent.itemSelectedCallback.NotifyOnValueSet((Action<int, string>)SelectedItem);
		}

		protected override void Remove(RobotConfigListItemDisplayEntityView entityView)
		{
			entityView.setupComponent.itemSelectedCallback.StopNotify((Action<int, string>)SelectedItem);
		}
	}
}
