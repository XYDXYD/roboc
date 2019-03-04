using Fabric;
using Mothership;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class GUIInputControllerSim : IGUIInputControllerSim, IGUIInputController
	{
		private readonly Dictionary<GuiScreens, IGUIDisplay> _guiScreens = new Dictionary<GuiScreens, IGUIDisplay>();

		private ShortCutMode _currentShortCutMode;

		private GuiScreens _currentActiveScreen;

		private readonly FloatingWidgetQuitStack _floatingWidgets = new FloatingWidgetQuitStack();

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
		public IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public bool isInMainScreen => _currentActiveScreen == GuiScreens.Undefined;

		public event Action OnScreenStateChange = delegate
		{
		};

		public event Action<GuiScreens> OnFailShowScreen = delegate
		{
		};

		public GUIInputControllerSim()
		{
			_currentActiveScreen = GuiScreens.Undefined;
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
			return _currentShortCutMode;
		}

		public bool AreHintsAllowed()
		{
			return cursorMode.currentMode == Mode.Lock;
		}

		public void CloseCurrentScreen(bool hideTopBar = true)
		{
			ChangeScreenState(GetActiveScreen(), GuiInputType.Hide);
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

		public IGUIDisplay GetControllerForScreen(GuiScreens screen)
		{
			return _guiScreens[screen];
		}

		public void ForceCloseJustThisScreen(GuiScreens whichScreen)
		{
			if (_guiScreens[whichScreen].IsActive())
			{
				_guiScreens[whichScreen].Hide();
			}
		}

		public void ToggleScreenViaShortcut(GuiScreens screen)
		{
			GuiInputType inputType = GuiInputType.Toggle;
			if (_currentShortCutMode == ShortCutMode.AllShortCuts)
			{
				ChangeScreenState(screen, inputType);
			}
			else if (_currentShortCutMode == ShortCutMode.OnlyGUINoSwitching)
			{
				if (screen != GuiScreens.BattleCountdown)
				{
					ChangeScreenState(screen, inputType);
				}
			}
			else if (_currentShortCutMode == ShortCutMode.OnlyEsc)
			{
				if (screen == GuiScreens.PauseMenu || GetActiveScreen() == GuiScreens.PauseMenu)
				{
					ChangeScreenState(screen, inputType);
				}
			}
			else if (_currentShortCutMode == ShortCutMode.AnyKeyClear)
			{
				CloseCurrentScreen();
			}
		}

		public bool IsInBuildMode()
		{
			return _currentActiveScreen == GuiScreens.BuildMode;
		}

		public GuiScreens GetActiveScreen()
		{
			using (Dictionary<GuiScreens, IGUIDisplay>.Enumerator enumerator = _guiScreens.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.Value.IsActive())
				{
				}
			}
			return _currentActiveScreen;
		}

		public void HandleQuitPressed()
		{
			if (!_floatingWidgets.HandleQuitPressed())
			{
				ToggleScreenViaShortcut(GuiScreens.PauseMenu);
			}
		}

		public bool IsMainScreen(GuiScreens screen)
		{
			return false;
		}

		private void ChangeScreenState(GuiScreens newScreen, GuiInputType inputType)
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
					flag = HideScreens();
					break;
				case GuiInputType.Toggle:
					flag = ((!_guiScreens[newScreen].IsActive()) ? ShowScreenInternal(newScreen) : HideScreens());
					break;
				}
				if (flag)
				{
					this.OnScreenStateChange();
				}
			}
		}

		public bool IsTutorialScreenActive()
		{
			return _guiScreens[GuiScreens.SimulationTutorialScreen].IsActive();
		}

		public bool ShouldShowSocialGUI()
		{
			if (_guiScreens.TryGetValue(GuiScreens.SimulationTutorialScreen, out IGUIDisplay value) && value.IsActive())
			{
				return false;
			}
			bool result = _currentActiveScreen != GuiScreens.PauseMenu;
			if (_currentActiveScreen == GuiScreens.ControlsScreen || _currentActiveScreen == GuiScreens.SettingsScreen)
			{
				result = false;
			}
			return result;
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

		private bool HideActiveScreens()
		{
			bool result = false;
			foreach (KeyValuePair<GuiScreens, IGUIDisplay> guiScreen in _guiScreens)
			{
				if (guiScreen.Value.IsActive() && !guiScreen.Value.doesntHideOnSwitch)
				{
					if (!guiScreen.Value.Hide())
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
				if (guiScreen.Value.IsActive() && !guiScreen.Value.doesntHideOnSwitch)
				{
					guiScreen.Value.Hide();
				}
			}
		}

		private bool ShouldHideCursor(GuiScreens screen)
		{
			return screen == GuiScreens.SimulationTutorialScreen;
		}

		private bool ShowScreenInternal(GuiScreens newScreen)
		{
			if (_guiScreens[newScreen].IsActive())
			{
				return false;
			}
			if (newScreen != GuiScreens.Chat)
			{
				HideAllActiveScreens();
			}
			if (_guiScreens[newScreen].Show() == GUIShowResult.Showed)
			{
				ChangeCursorMode((!ShouldHideCursor(newScreen)) ? Mode.Free : Mode.Lock);
				_currentActiveScreen = newScreen;
				battleHudStyleController.SetStyle(_guiScreens[newScreen].battleHudStyle);
				SetShortCutMode(_guiScreens[newScreen].shortCutMode);
				blurEffectController.EnableEffect(_guiScreens[newScreen].isScreenBlurred);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuOpen));
				return true;
			}
			this.OnFailShowScreen(newScreen);
			return false;
		}

		private bool HideScreens()
		{
			if (!HideActiveScreens())
			{
				return false;
			}
			ChangeCursorMode(Mode.Lock);
			SetShortCutMode(ShortCutMode.AllShortCuts);
			blurEffectController.EnableEffect(enable: false);
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIMenuClosed));
			battleHudStyleController.SetStyle(HudStyle.Full);
			_currentActiveScreen = GuiScreens.Undefined;
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
