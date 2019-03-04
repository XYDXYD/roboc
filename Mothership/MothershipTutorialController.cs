using Mothership.GUI;
using Mothership.GUI.Inventory;
using PlayMaker;
using PlayMaker.Tutorial;
using Simulation;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class MothershipTutorialController : TutorialControllerBase, IGUIDisplay, IWaitForFrameworkDestruction, IInitialize, IComponent
	{
		private MothershipTutorialScreen _tutorialDisplay;

		private IPlayMakerDataRequest _request;

		private BuildModeShortcutHintsObserveable _buildModeObserveable;

		private GhostCube _ghostcube;

		[Inject]
		public IAutoSaveController autoSaveController
		{
			private get;
			set;
		}

		[Inject]
		public CurrentToolMode currentToolMode
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal GhostCubeController ghostCubeController
		{
			private get;
			set;
		}

		[Inject]
		internal ITutorialCubePlacementController tutorialCubePlacementController
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeHolder selectedCube
		{
			private get;
			set;
		}

		[Inject]
		internal CurrentCubeSelectorCategory currentCategory
		{
			private get;
			set;
		}

		[Inject]
		internal CubeSelectHighlighter cubeSelectorHighlighter
		{
			private get;
			set;
		}

		[Inject]
		internal MirrorMode mirrorMode
		{
			private get;
			set;
		}

		[Inject]
		internal AdvancedRobotEditSettings advancedRobotEditSettings
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.MothershipTutorialScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public bool isScreenBlurred => false;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => true;

		public bool isReady => _tutorialDisplay != null;

		public HudStyle battleHudStyle => HudStyle.Full;

		public MothershipTutorialController(TutorialTipObservable tipsobserveable, BuildModeShortcutHintsObserveable buildmodeHintsObserveable)
			: base(tipsobserveable)
		{
			_buildModeObserveable = buildmodeHintsObserveable;
		}

		public void EnableBackground(bool enable)
		{
		}

		public void OnDependenciesInjected()
		{
			ghostCubeController.OnGhostCubeInitialized += HandleOnGhostCubeInitialized;
		}

		private void HandleOnGhostCubeInitialized(GhostCube cube)
		{
			_ghostcube = cube;
		}

		public void GetGhostCubeDebugInfo(ref Vector3 ghostCubePos, ref int orientation)
		{
			if (_ghostcube != null)
			{
				Int3 finalGridPos = _ghostcube.finalGridPos;
				float num = finalGridPos.x;
				Int3 finalGridPos2 = _ghostcube.finalGridPos;
				float num2 = finalGridPos2.y;
				Int3 finalGridPos3 = _ghostcube.finalGridPos;
				ghostCubePos.Set(num, num2, (float)finalGridPos3.z);
				orientation = _ghostcube.rotation;
			}
			else
			{
				ghostCubePos.Set(0f, 0f, 0f);
				orientation = 0;
			}
		}

		protected override void RegisterContextSpecificRequestHandlers(Action<Type, Action<IPlayMakerDataRequest>> RegisterPlayMakerRequestHandler)
		{
			RegisterPlayMakerRequestHandler(typeof(CheckHasFilledAllCubeLocationsRequest), HandleCheckHasFilledAllCubeLocationsRequest);
			RegisterPlayMakerRequestHandler(typeof(CheckActiveScreensRequest), HandleCheckActiveScreensRequest);
			RegisterPlayMakerRequestHandler(typeof(CheckCurrentSelectedCategoryRequest), HandleCheckCurrentSelectedCategory);
			RegisterPlayMakerRequestHandler(typeof(GetCurrentCubeSelectedPlaymakerRequest), HandleGetCurrentSelectedCube);
			RegisterPlayMakerRequestHandler(typeof(SaveTutorialBayPlaymakerRequest), HandleSaveTutorialBayRequest);
			RegisterPlayMakerRequestHandler(typeof(CheckInBuildModeRequest), HandleCheckIsInBuildModeRequest);
		}

		protected override void RegisterContextSpecificCommandHandlers(Action<Action<IPlaymakerCommandInputParameters>, Type> RegisterPlayMakerCommandHandlerAction)
		{
			RegisterPlayMakerCommandHandlerAction(HandleLaunchIntoTestModeCommandExecution, typeof(LaunchIntoTestModeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleSwitchIntoBuildModeCommandExecution, typeof(SwitchIntoBuildModeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleChangeToolModeCommandExecution, typeof(ChangeToolModeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleSwitchingLockNodeCommandExecution, typeof(SwitchingLockToolModeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleBlockAToolModeCommandExecution, typeof(BlockAToolModeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleAdjustCameraPosCommandExecution, typeof(AdjustCameraPositionNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleToggleShortcutVisibilityCommandExecution, typeof(ToggleShortcutHintsVisibilityCommandParameters));
			RegisterPlayMakerCommandHandlerAction(HandleSetupValidCubeLocationsCommandExecution, typeof(SetupValidCubeLocationsListInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleSelectSpecificTypeOfCubeCommandExecution, typeof(SelectSpecificTypeOfCubeNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleCubeCategoryAvailabilitiesNodeCommandExecution, typeof(ToggleCubeCategoryAvailabilitiesNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleToggleCubeHighlightingCommandExecution, typeof(ToggleCubeHighlightingNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleToggleCategoryHighlightCommandExecution, typeof(ToggleCategoryHighlightNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleRebuildTutorialRobotCommandExecution, typeof(RebuildTutorialRobotNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleHideTutorialRobotCommandExecution, typeof(HideTutorialRobotNodeInputParameters));
			RegisterPlayMakerCommandHandlerAction(HandleShowLoadingScreenCommandExecution, typeof(ShowFullscreenLoadingScreenNodeCommandParameters));
			RegisterPlayMakerCommandHandlerAction(HandleHideLoadingScreenCommandExecution, typeof(HideFullscreenLoadingScreenNodeCommandParameters));
		}

		private void HandleSaveTutorialBayRequest(IPlayMakerDataRequest dataProvided)
		{
			ActivateLoadingGUI();
			_request = dataProvided;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SaveThenOtherStuff);
		}

		private IEnumerator SaveThenOtherStuff()
		{
			yield return autoSaveController.PerformSave();
			DeactivateLoadingGUI();
			_request.Execute();
		}

		private void HandleHideTutorialRobotCommandExecution(IPlaymakerCommandInputParameters obj)
		{
			MachineBoard.Instance.board.get_gameObject().SetActive(false);
			base.commandFactory.Build<ZeroHUDCPUGaugesCommand>().Inject(lockToZero: true).Execute();
		}

		private void HandleShowLoadingScreenCommandExecution(IPlaymakerCommandInputParameters obj)
		{
			base.loadingIconPresenter.forceOpaque = true;
			base.loadingIconPresenter.NotifyLoading("Loadingtutorial");
		}

		private void HandleHideLoadingScreenCommandExecution(IPlaymakerCommandInputParameters obj)
		{
			base.loadingIconPresenter.forceOpaque = false;
			base.loadingIconPresenter.NotifyLoadingDone("Loadingtutorial");
		}

		private void HandleRebuildTutorialRobotCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			MachineBoard.Instance.board.get_gameObject().SetActive(true);
			base.commandFactory.Build<ZeroHUDCPUGaugesCommand>().Inject(lockToZero: false).Execute();
			base.garage.BuildRobotInMothershipWithoutLoadingScreen();
		}

		private void HandleToggleCategoryHighlightCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			CubeCategory inputParameters2 = inputParameters.GetInputParameters<CubeCategory>();
			bool inputParameters3 = inputParameters.GetInputParameters<bool>();
			currentCategory.ChangeCategoryStatusInfo(inputParameters2, availability: true, inputParameters3);
		}

		private void HandleToggleCubeHighlightingCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			CubeTypeID inputParameters2 = inputParameters.GetInputParameters<CubeTypeID>();
			bool inputParameters3 = inputParameters.GetInputParameters<bool>();
			cubeSelectorHighlighter.ToggleCubeHighlighting(inputParameters2, inputParameters3);
		}

		private void HandleGetCurrentSelectedCube(IPlayMakerDataRequest request)
		{
			CubeTypeID selectedCubeID = this.selectedCube.selectedCubeID;
			string selectedCube = Convert.ToString(selectedCubeID.ID, 16).PadLeft(8, '0');
			request.AssignResults(new GetCurrentCubeSelectedPlaymakerRequestResult(selectedCube));
			request.Execute();
		}

		private void HandleCheckCurrentSelectedCategory(IPlayMakerDataRequest request)
		{
			CubeCategory selectedCategory = currentCategory.selectedCategory;
			request.AssignResults(new CheckCurrentSelectedCategoryRequestReturn(selectedCategory));
			request.Execute();
		}

		private void HandleCheckActiveScreensRequest(IPlayMakerDataRequest request)
		{
			bool inventoryScreen_ = base.guiInputController.GetActiveScreen() == GuiScreens.InventoryScreen;
			request.AssignResults(new CheckActiveScreensRequestReturn(inventoryScreen_));
			request.Execute();
		}

		private void HandleCheckHasFilledAllCubeLocationsRequest(IPlayMakerDataRequest request)
		{
			bool hasFilledAllLocations_ = tutorialCubePlacementController.CheckIfAllCubeLocationsHaveBeenFilled();
			request.AssignResults(new CheckHasFilledAllCubeLocationsRequestReturn(hasFilledAllLocations_));
			request.Execute();
		}

		private void HandleCheckIsInBuildModeRequest(IPlayMakerDataRequest request)
		{
			bool hasFilledAllLocations_ = worldSwitching.CurrentWorld == WorldSwitchMode.BuildMode;
			request.AssignResults(new CheckInBuildModeRequestReturn(hasFilledAllLocations_));
			request.Execute();
		}

		private void HandleCubeCategoryAvailabilitiesNodeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			bool[] inputParameters2 = inputParameters.GetInputParameters<bool[]>();
			for (int i = 0; i < inputParameters2.Length; i++)
			{
				currentCategory.ChangeCategoryStatusInfo((CubeCategory)i, inputParameters2[i], highlighted: false);
			}
		}

		private void HandleToggleShortcutVisibilityCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			BuildModeHintEvent buildModeHintEvent = (!inputParameters.GetInputParameters<bool>()) ? BuildModeHintEvent.HideHints : BuildModeHintEvent.ShowHints;
			_buildModeObserveable.Dispatch(ref buildModeHintEvent);
		}

		private void HandleAdjustCameraPosCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			AdjustCameraPositionNodeInputParameters adjustCameraPositionNodeInputParameters = inputParameters as AdjustCameraPositionNodeInputParameters;
			Vector3 inputParameters2 = adjustCameraPositionNodeInputParameters.GetInputParameters<Vector3>(0);
			Vector3 inputParameters3 = adjustCameraPositionNodeInputParameters.GetInputParameters<Vector3>(1);
			worldSwitching.SetAvatarPositionAndRotation(inputParameters2, inputParameters3);
		}

		private void HandleSelectSpecificTypeOfCubeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			string inputParameters2 = inputParameters.GetInputParameters<string>();
			uint id = Convert.ToUInt32(inputParameters2, 16);
			CubeTypeID selectedCubeID = new CubeTypeID(id);
			selectedCube.selectedCubeID = selectedCubeID;
		}

		private void HandleSetupValidCubeLocationsCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			List<ValidCubeLocationOrientations> inputParameters2 = inputParameters.GetInputParameters<List<ValidCubeLocationOrientations>>();
			List<Vector3> inputParameters3 = inputParameters.GetInputParameters<List<Vector3>>();
			string inputParameters4 = inputParameters.GetInputParameters<string>();
			if (inputParameters.GetInputParameters<bool>())
			{
				tutorialCubePlacementController.ResetList();
			}
			uint id = Convert.ToUInt32(inputParameters4, 16);
			CubeTypeID cubeTypeID = new CubeTypeID(id);
			float num = 0f;
			for (int i = 0; i < inputParameters3.Count; i++)
			{
				num = 0f;
				bool anyOrientationPermitted = false;
				Quaternion val = default(Quaternion);
				switch (inputParameters2[i])
				{
				case ValidCubeLocationOrientations.Orientation_0:
					num = 0f;
					break;
				case ValidCubeLocationOrientations.Orientation_90:
					num = 90f;
					break;
				case ValidCubeLocationOrientations.Orientation_180:
					num = 180f;
					break;
				case ValidCubeLocationOrientations.Orientation_270:
					num = 270f;
					break;
				case ValidCubeLocationOrientations.Any:
					anyOrientationPermitted = true;
					break;
				}
				val = Quaternion.AngleAxis(num, Vector3.get_up());
				tutorialCubePlacementController.AddCubeLocationsToPlace(inputParameters3[i], val, cubeTypeID, anyOrientationPermitted);
			}
		}

		private void HandleChangeToolModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			CurrentToolMode.ToolMode inputParameters2 = inputParameters.GetInputParameters<CurrentToolMode.ToolMode>();
			currentToolMode.ForceImmediateModeChange(inputParameters2);
		}

		private void HandleBlockAToolModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			CurrentToolMode.ToolMode inputParameters2 = inputParameters.GetInputParameters<CurrentToolMode.ToolMode>();
			if (inputParameters.GetInputParameters<bool>())
			{
				currentToolMode.BlockMode(inputParameters2);
			}
			else
			{
				currentToolMode.UnblockMode(inputParameters2);
			}
		}

		private void HandleSwitchingLockNodeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			if (inputParameters.GetInputParameters<bool>())
			{
				currentToolMode.ObtainSwitchingLock(CurrentToolMode.SwitchingLockTypes.TutorialNode);
			}
			else
			{
				currentToolMode.ReleaseSwitchingLock(CurrentToolMode.SwitchingLockTypes.TutorialNode);
			}
		}

		private void HandleLaunchIntoTestModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			DateTime utcNow = DateTime.UtcNow;
			double totalHours = (utcNow - _signupDate).TotalHours;
			bool isNewPlayer_ = false;
			if (totalHours < 24.0)
			{
				isNewPlayer_ = true;
			}
			SwitchTutorialTestWorldDependency dependency = new SwitchTutorialTestWorldDependency("TestRobot", isRanked_: false, GameModeType.TestMode, isNewPlayer_);
			base.commandFactory.Build<SwitchToTutorialTestPlanetCommand>().Inject(dependency).Execute();
		}

		private void HandleSwitchIntoBuildModeCommandExecution(IPlaymakerCommandInputParameters inputParameters)
		{
			SwitchWorldDependency dependency = new SwitchWorldDependency("RC_BuildMode", _fastSwitch: false);
			base.commandFactory.Build<SwitchToBuildModeCommand>().Inject(dependency).Execute();
		}

		public override void ActivateLoadingGUI()
		{
			base.loadingIconPresenter.NotifyLoading("TutorialGUILoading");
		}

		public override void DeactivateLoadingGUI()
		{
			base.loadingIconPresenter.NotifyLoadingDone("TutorialGUILoading");
		}

		private void OnLoadTutorialStatusData(LoadTutorialStatusData answer)
		{
			Console.Log("Load tutorial status data complete: " + answer.inProgress + " , " + answer.skipped + " , " + answer.completed);
		}

		private void OnLoadFailed(ServiceBehaviour behaviour)
		{
			Console.Log("Failed loading tutorial status data");
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_tutorialDisplay = null;
		}

		public override bool IsActive()
		{
			return _tutorialDisplay.IsActive();
		}

		public override void ShowTutorialScreenAndActivateFSM()
		{
			base.guiInputController.ShowScreen(GuiScreens.MothershipTutorialScreen);
			advancedRobotEditSettings.TurnOffAdvancedEditSettings();
		}

		public override void SetDisplay(TutorialScreenBase display)
		{
			_tutorialDisplay = (display as MothershipTutorialScreen);
		}

		public override void HideTutorialScreen()
		{
			mirrorMode.SwitchMode(forceOff: true);
			base.guiInputController.ForceCloseJustThisScreen(GuiScreens.MothershipTutorialScreen);
			_tutorialDisplay.HideScreen();
		}

		public GUIShowResult Show()
		{
			_tutorialDisplay.Show();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_tutorialDisplay.HideScreen();
			return true;
		}
	}
}
