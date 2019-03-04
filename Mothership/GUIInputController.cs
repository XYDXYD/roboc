using Fabric;
using Simulation;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class GUIInputController : IGUIInputControllerMothership, IInitialize, IGUIInputController
	{
		private readonly GuiScreens[] LOBBY_CONTEXT_SCREENS = new GuiScreens[6]
		{
			GuiScreens.BuildMode,
			GuiScreens.Garage,
			GuiScreens.PlayScreen,
			GuiScreens.CustomGameScreen,
			GuiScreens.InventoryScreen,
			GuiScreens.RobotShop
		};

		private readonly Dictionary<GuiScreens, IGUIDisplay> _guiScreens = new Dictionary<GuiScreens, IGUIDisplay>();

		private readonly FloatingWidgetQuitStack _floatingWidgets = new FloatingWidgetQuitStack();

		private ITopBarDisplay _topBarDisplay;

		private GuiScreens _activeMainGUIScreen = GuiScreens.Garage;

		private GuiScreens _currentActiveScreen;

		private ShortCutMode _currentShortCutMode;

		[Inject]
		public BlurEffectController blurEffectController
		{
			private get;
			set;
		}

		[Inject]
		public ICursorMode cursorMode
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
		public IDispatchWorldSwitching worldSwitch
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

		public bool isInMainScreen => _activeMainGUIScreen == _currentActiveScreen;

		public event Action OnScreenStateChange = delegate
		{
		};

		public event Action<GuiScreens> OnFailShowScreen = delegate
		{
		};

		public void OnDependenciesInjected()
		{
			worldSwitch.OnWorldJustSwitched += WorldSwitch_OnWorldJustSwitched;
		}

		private void WorldSwitch_OnWorldJustSwitched(WorldSwitchMode current)
		{
			_currentActiveScreen = ((current != 0) ? GuiScreens.Garage : GuiScreens.BuildMode);
			_activeMainGUIScreen = _currentActiveScreen;
			worldSwitch.OnWorldJustSwitched -= WorldSwitch_OnWorldJustSwitched;
		}

		public void MothershipFlowCompleted()
		{
			ChangeScreenState(_activeMainGUIScreen, GuiInputType.Show);
		}

		public bool IsActiveScreenFullHUDStyle()
		{
			GuiScreens activeScreen = GetActiveScreen();
			if (activeScreen == GuiScreens.Undefined)
			{
				return true;
			}
			if (_guiScreens.TryGetValue(activeScreen, out IGUIDisplay value))
			{
				return value.battleHudStyle == HudStyle.Full;
			}
			return false;
		}

		public void AddDisplayScreens(IGUIDisplay[] displays)
		{
			foreach (IGUIDisplay iGUIDisplay in displays)
			{
				ITopBarDisplay topBarDisplay = iGUIDisplay as ITopBarDisplay;
				if (topBarDisplay != null)
				{
					_topBarDisplay = topBarDisplay;
				}
				_guiScreens.Add(iGUIDisplay.screenType, iGUIDisplay);
			}
		}

		public void UpdateShortCutMode()
		{
			if (_guiScreens.ContainsKey(_currentActiveScreen))
			{
				SetShortCutMode(_guiScreens[_currentActiveScreen].shortCutMode);
			}
		}

		public void SetShortCutMode(ShortCutMode shortCutMode)
		{
			_currentShortCutMode = shortCutMode;
		}

		public ShortCutMode GetShortCutMode()
		{
			if (loadingIconPresenter.isLoading)
			{
				return ShortCutMode.NoKeyboardInputAllowed;
			}
			return _currentShortCutMode;
		}

		public bool AreHintsAllowed()
		{
			if (cursorMode.currentMode == Mode.Lock)
			{
				return true;
			}
			if (WorldSwitching.IsMultiplayer())
			{
				return GetActiveScreen() == GuiScreens.TopBar;
			}
			return GetActiveScreen() == GuiScreens.BuildMode;
		}

		public void ShowScreen(GuiScreens newScreen)
		{
			ChangeScreenState(newScreen, GuiInputType.Show);
		}

		public void ToggleScreen(GuiScreens newScreen)
		{
			ChangeScreenState(newScreen, GuiInputType.Toggle);
		}

		public void ToggleCurrentScreen()
		{
			ChangeScreenState(GetActiveScreen(), GuiInputType.Toggle);
		}

		public void CloseCurrentScreen(bool hideTopBar = true)
		{
			if (!_floatingWidgets.HandleQuitPressed())
			{
				ChangeScreenState(GetActiveScreen(), GuiInputType.Hide, hideTopBar);
			}
		}

		public IGUIDisplay GetControllerForScreen(GuiScreens screen)
		{
			return _guiScreens[screen];
		}

		public void ToggleScreenViaShortcut(GuiScreens screen)
		{
			if (WorldSwitching.IsInBuildMode() && screen == GuiScreens.InventoryScreen && currentToolMode.currentBuildTool == CurrentToolMode.ToolMode.Paint)
			{
				screen = GuiScreens.PaintColorSelector;
			}
			ChangeScreenState(screen, GuiInputType.Toggle);
		}

		public void HandleQuitPressed()
		{
			if (!_floatingWidgets.HandleQuitPressed())
			{
				if (IsMainScreen(GetActiveScreen()) || GetActiveScreen() == GuiScreens.Undefined)
				{
					ChangeScreenState(GuiScreens.PauseMenu, GuiInputType.Show);
				}
				else
				{
					CloseCurrentScreen();
				}
			}
		}

		public GuiScreens GetActiveScreen()
		{
			return _currentActiveScreen;
		}

		public string GetActiveMainScreenName()
		{
			return _activeMainGUIScreen.ToString();
		}

		public bool IsMainScreen(GuiScreens screen)
		{
			if (WorldSwitching.IsInBuildMode())
			{
				return screen == GuiScreens.BuildMode;
			}
			for (int i = 0; i < LOBBY_CONTEXT_SCREENS.Length; i++)
			{
				GuiScreens guiScreens = LOBBY_CONTEXT_SCREENS[i];
				if (guiScreens == screen)
				{
					return true;
				}
			}
			return false;
		}

		private void ChangeScreenState(GuiScreens newScreen, GuiInputType inputType, bool hideTopBar = true)
		{
			if (!ErrorWindow.IsErrorWindowOpen())
			{
				bool flag = false;
				switch (inputType)
				{
				case GuiInputType.Show:
					flag = ShowScreenInternal(newScreen);
					break;
				case GuiInputType.Hide:
					flag = HideScreens(hideTopBar);
					break;
				case GuiInputType.Toggle:
					flag = ((!_guiScreens[newScreen].IsActive() || IsMainScreen(newScreen)) ? ShowScreenInternal(newScreen) : HideScreens());
					break;
				}
				if (flag)
				{
					SafeEvent.SafeRaise(this.OnScreenStateChange);
				}
			}
		}

		public bool IsTutorialScreenActive()
		{
			return _guiScreens[GuiScreens.MothershipTutorialScreen].IsActive();
		}

		public bool ShouldShowSocialGUI()
		{
			if (_guiScreens[GuiScreens.MothershipTutorialScreen].IsActive())
			{
				return false;
			}
			return _currentActiveScreen == GuiScreens.Garage || _currentActiveScreen == GuiScreens.BuildMode || _currentActiveScreen == GuiScreens.CustomGameScreen || _currentActiveScreen == GuiScreens.BattleCountdown || _currentActiveScreen == GuiScreens.AvatarSelection;
		}

		private void ChangeCursorMode(Mode newMode)
		{
			if (newMode != cursorMode.currentMode)
			{
				if (newMode == Mode.Free)
				{
					cursorMode.PushFreeMode();
				}
				else
				{
					cursorMode.PopFreeMode();
				}
			}
		}

		public void ForceCloseJustThisScreen(GuiScreens whichScreen)
		{
			if (_guiScreens.ContainsKey(whichScreen) && _guiScreens[whichScreen].IsActive())
			{
				_guiScreens[whichScreen].Hide();
			}
		}

		private bool HideActiveScreens()
		{
			bool result = false;
			foreach (KeyValuePair<GuiScreens, IGUIDisplay> guiScreen in _guiScreens)
			{
				IGUIDisplay value = guiScreen.Value;
				if (!value.doesntHideOnSwitch && value.IsActive())
				{
					if (!value.Hide())
					{
						return false;
					}
					result = true;
				}
			}
			return result;
		}

		private void HideAllActiveScreens()
		{
			foreach (KeyValuePair<GuiScreens, IGUIDisplay> guiScreen in _guiScreens)
			{
				IGUIDisplay value = guiScreen.Value;
				if (value.IsActive() && !value.doesntHideOnSwitch)
				{
					value.Hide();
				}
			}
		}

		private bool ShowScreenInternal(GuiScreens newScreen)
		{
			IGUIDisplay iGUIDisplay = _guiScreens[newScreen];
			if (iGUIDisplay.IsActive())
			{
				return false;
			}
			if (newScreen != GuiScreens.Chat && newScreen != GuiScreens.GenericPopUp)
			{
				HideAllActiveScreens();
				if (_guiScreens[_activeMainGUIScreen].hasBackground)
				{
					if (iGUIDisplay.hasBackground)
					{
						_guiScreens[_activeMainGUIScreen].EnableBackground(enable: false);
					}
					else if (_guiScreens.ContainsKey(_activeMainGUIScreen))
					{
						_guiScreens[_activeMainGUIScreen].EnableBackground(enable: true);
					}
				}
			}
			GUIShowResult gUIShowResult = iGUIDisplay.Show();
			if (gUIShowResult == GUIShowResult.Showed)
			{
				if (IsMainScreen(newScreen))
				{
					_activeMainGUIScreen = newScreen;
				}
				_currentActiveScreen = newScreen;
				if (_topBarDisplay != null && iGUIDisplay.topBarStyle != TopBarStyle.SameAsPrevious)
				{
					_topBarDisplay.SetDisplayStyle(iGUIDisplay.topBarStyle);
				}
				SetShortCutMode(iGUIDisplay.shortCutMode);
				blurEffectController.EnableEffect(iGUIDisplay.isScreenBlurred);
				ChangeCursorMode((!ShouldHideCursor(newScreen)) ? Mode.Free : Mode.Lock);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuOpen));
				return true;
			}
			this.OnFailShowScreen(newScreen);
			if (gUIShowResult == GUIShowResult.NotShowed)
			{
				CloseCurrentScreen();
			}
			return false;
		}

		private bool ShouldHideCursor(GuiScreens screen)
		{
			return screen == GuiScreens.BuildMode;
		}

		private bool HideScreens(bool hideTopBar = true)
		{
			if (WorldSwitching.IsInBuildMode())
			{
				_activeMainGUIScreen = GuiScreens.BuildMode;
			}
			if (!HideActiveScreens())
			{
				ChangeCursorMode((!ShouldHideCursor(_activeMainGUIScreen)) ? Mode.Free : Mode.Lock);
				return false;
			}
			if (_topBarDisplay != null)
			{
				if (hideTopBar)
				{
					_topBarDisplay.SetDisplayStyle(TopBarStyle.OffScreen);
				}
			}
			else
			{
				ChangeCursorMode(Mode.Lock);
			}
			SetShortCutMode(ShortCutMode.AllShortCuts);
			blurEffectController.EnableEffect(enable: false);
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuClosed));
			if (_topBarDisplay != null)
			{
				ChangeScreenState(_activeMainGUIScreen, GuiInputType.Show);
				return false;
			}
			return true;
		}

		public void AddFloatingWidget(IFloatingWidget w)
		{
			_floatingWidgets.Add(w);
		}

		public void RemoveFloatingWidget(IFloatingWidget w)
		{
			_floatingWidgets.Remove(w);
		}
	}
}
