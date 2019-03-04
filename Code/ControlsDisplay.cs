using Services.Requests.Interfaces;
using Simulation;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

internal sealed class ControlsDisplay : IGUIDisplay, ITickable, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IComponent, ITickableBase
{
	private Dictionary<int, ButtonActionMap> _buttonToActionMap = new Dictionary<int, ButtonActionMap>();

	private Dictionary<string, string> _stringToMouseActionMap = new Dictionary<string, string>();

	private Dictionary<string, string> _stringToJoystickActionMap = new Dictionary<string, string>();

	private int _pollingCategoryId = -1;

	private int _pollingButtonId = -1;

	private bool _waitingForNewKey;

	private bool _isShopAvailable;

	private IInputRemappingSaveData _save;

	private ButtonRemapHelperRewired _buttonRemapHelper;

	private InputRemapPollerRewired _poller;

	private ControlsScreen controlsScreen;

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal ControlsChangedObserver controlsChangedObserver
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingPresenter
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
	public IPauseManager pauseManager
	{
		private get;
		set;
	}

	public GuiScreens screenType => GuiScreens.ControlsScreen;

	public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

	public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

	public bool isScreenBlurred => true;

	public bool hasBackground => false;

	public bool doesntHideOnSwitch => false;

	public HudStyle battleHudStyle => HudStyle.HideAll;

	public event Action OnShowControlsScreen = delegate
	{
	};

	public void EnableBackground(bool enable)
	{
	}

	public void SetScreen(ControlsScreen screen)
	{
		controlsScreen = screen;
	}

	unsafe void IWaitForFrameworkInitialization.OnFrameworkInitialized()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		_save = new InputRemappingSaveDataRewired();
		_buttonRemapHelper = new ButtonRemapHelperRewired();
		_save.Load();
		if (!_save.HasSavedSettings())
		{
			_save.Save();
		}
		TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadPlatformConfigurationValues);
		UICamera.onScreenResize = Delegate.Combine((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		UICamera.onScreenResize = Delegate.Remove((Delegate)UICamera.onScreenResize, (Delegate)new OnScreenResize((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public GUIShowResult Show()
	{
		_stringToMouseActionMap = _buttonRemapHelper.GenerateMouseStringsToAxisName();
		_stringToJoystickActionMap = _buttonRemapHelper.GenerateJoystickStringsToAxisName();
		controlsScreen.Show(OnRemapButtonPressed, OnRemapListChanged);
		this.OnShowControlsScreen();
		Initialise();
		InputRemapper.Instance.RegisterOnControllerConnected(OnControllerConnected);
		InputRemapper.Instance.RegisterOnControllerDisconnected(OnControllerDisconnected);
		pauseManager.Pause(pause: true);
		return GUIShowResult.Showed;
	}

	public bool Hide()
	{
		InputRemapper.Instance.UnregisterOnControllerConnected(OnControllerConnected);
		InputRemapper.Instance.UnregisterOnControllerDisconnected(OnControllerDisconnected);
		pauseManager.Pause(pause: false);
		controlsScreen.Hide();
		return true;
	}

	public bool IsActive()
	{
		if (controlsScreen == null)
		{
			return false;
		}
		return controlsScreen.IsActive();
	}

	private void OnControllerConnected()
	{
		StopPollingForNewKey();
		Populate();
	}

	private void OnControllerDisconnected()
	{
		StopPollingForNewKey();
		Populate();
	}

	private void Initialise()
	{
		_pollingCategoryId = -1;
		_pollingButtonId = -1;
		_waitingForNewKey = false;
		Populate();
	}

	private void Populate()
	{
		_buttonToActionMap = _buttonRemapHelper.GenerateButtonActionMaps(_isShopAvailable);
		FasterList<string> mouseAxisNames = _buttonRemapHelper.GetMouseAxisNames();
		FasterList<string> mouseElementNamesSplitAxes = _buttonRemapHelper.GetMouseElementNamesSplitAxes();
		FasterList<string> joypadAxisNames = _buttonRemapHelper.GetJoypadAxisNames();
		FasterList<string> joypadElementNamesSplitAxes = _buttonRemapHelper.GetJoypadElementNamesSplitAxes();
		controlsScreen.Populate(_buttonToActionMap, mouseAxisNames, joypadAxisNames, mouseElementNamesSplitAxes, joypadElementNamesSplitAxes);
	}

	private void OnRemapButtonPressed(int categoryId, int buttonId, InputAssignmentButton.InputType inputType)
	{
		if (!_waitingForNewKey)
		{
			ButtonReassignData buttonReassignData = _buttonToActionMap[categoryId].map[buttonId];
			if (buttonReassignData.CanRebind())
			{
				StartPollingForNewKey(categoryId, buttonId, inputType);
			}
		}
	}

	private void OnRemapListChanged(int categoryId, int buttonId, InputAssignmentButton.InputType inputType, string localisedInputName)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		_buttonToActionMap[categoryId].map[buttonId].RemoveElementMapsForAction();
		if (localisedInputName == null)
		{
			return;
		}
		string value = string.Empty;
		if (inputType == InputAssignmentButton.InputType.Mouse)
		{
			if (!_stringToMouseActionMap.TryGetValue(localisedInputName, out value))
			{
				value = localisedInputName;
				Console.LogError(string.Format("Localised Mouse Axis Not Found: ", localisedInputName));
			}
		}
		else if (!_stringToJoystickActionMap.TryGetValue(localisedInputName, out value))
		{
			value = localisedInputName;
			Console.LogError(string.Format("Localised Joystick Axis Not Found: ", localisedInputName));
		}
		_buttonToActionMap[categoryId].map[buttonId].ReplaceOrCreateElementMap(value, ButtonRemapHelperRewired.GetRewiredType(inputType));
		Populate();
	}

	public void ButtonClicked(ButtonType buttonType)
	{
		switch (buttonType)
		{
		case ButtonType.Confirm:
			_save.Save();
			controlsChangedObserver.ControlsChanged();
			guiInputController.CloseCurrentScreen();
			break;
		case ButtonType.Cancel:
			_save.Load();
			guiInputController.CloseCurrentScreen();
			break;
		case ButtonType.Reset:
			_save.Revert();
			controlsChangedObserver.ControlsChanged();
			Populate();
			break;
		}
	}

	public void StartPollingForNewKey(int categoryId, int buttonId, InputAssignmentButton.InputType inputType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_poller = new InputRemapPollerRewired(ButtonRemapHelperRewired.GetRewiredType(inputType));
		_pollingCategoryId = categoryId;
		_pollingButtonId = buttonId;
		_waitingForNewKey = true;
		controlsScreen.SetPressAnyKeyDialogActive(active: true);
	}

	public void Tick(float deltaTime)
	{
		if (_waitingForNewKey)
		{
			UpdatePolling(deltaTime);
		}
	}

	private void UpdatePolling(float deltaTime)
	{
		if (_poller.PollControllerForAssignment())
		{
			OnPollingSucceeded();
		}
	}

	private void OnPollingSucceeded()
	{
		_buttonToActionMap[_pollingCategoryId].map[_pollingButtonId].RemoveElementMapsForAction();
		_buttonToActionMap[_pollingCategoryId].map[_pollingButtonId].ReplaceOrCreateElementMap(_poller);
		StopPollingForNewKey();
		Populate();
	}

	private void StopPollingForNewKey()
	{
		_pollingCategoryId = -1;
		_pollingButtonId = -1;
		_waitingForNewKey = false;
		controlsScreen.SetPressAnyKeyDialogActive(active: false);
	}

	private IEnumerator LoadPlatformConfigurationValues()
	{
		loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
		ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> task = request.AsTask();
		yield return new HandleTaskServiceWithError(task, delegate
		{
			loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
		}, delegate
		{
			loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
		}).GetEnumerator();
		loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
		if (task.succeeded)
		{
			_isShopAvailable = task.result.MainShopButtonAvailable;
		}
		else
		{
			OnLoadingFailed(task.behaviour);
		}
	}

	private void OnLoadingFailed(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}

	private void Refresh()
	{
		controlsScreen.ResetScrollbar();
		Populate();
	}
}
