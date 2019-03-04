using Simulation;
using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class PauseMenuControllerMothershipTutorial : IGUIDisplay, IPauseMenuController, IComponent
	{
		private PauseMenuWidgetMothership _pauseMenu;

		[Inject]
		public IPauseManager pauseManager
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
		internal ICommandFactory commandFactory
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

		public GuiScreens screenType => GuiScreens.PauseMenu;

		public TopBarStyle topBarStyle
		{
			get
			{
				if (WorldSwitching.IsInBuildMode())
				{
					return TopBarStyle.OffScreen;
				}
				return TopBarStyle.Default;
			}
		}

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public PauseMenuWidgetMothership pauseMenu
		{
			set
			{
				_pauseMenu = value;
			}
		}

		public event Action OnShowPauseMenu = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
		}

		public void SetWidget(IPauseMenuWidget widget)
		{
			pauseMenu = (widget as PauseMenuWidgetMothership);
		}

		public GUIShowResult Show()
		{
			PauseMenuWidgetMothership.MenuType state = WorldSwitching.IsInBuildMode() ? PauseMenuWidgetMothership.MenuType.BuildModeTutorial : PauseMenuWidgetMothership.MenuType.Mothership;
			_pauseMenu.Show(state);
			pauseManager.Pause(pause: true);
			this.OnShowPauseMenu();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_pauseMenu.Hide();
			pauseManager.Pause(pause: false);
			return true;
		}

		public bool IsActive()
		{
			if (_pauseMenu == null)
			{
				return false;
			}
			return _pauseMenu.IsActive();
		}

		private void QuitGame()
		{
			_pauseMenu.ShowConfirmQuitDialog(show: true);
		}

		private void ConfirmQuit()
		{
			Application.Quit();
		}

		private void ReturnToMothership()
		{
			commandFactory.Build<SkipTutorialCommand>().Execute();
		}

		private void CancelQuit()
		{
			_pauseMenu.ShowConfirmQuitDialog(show: false);
		}

		public void Clicked(ButtonType buttonType)
		{
			if (buttonType == ButtonType.QuitGame)
			{
				if (!WorldSwitching.IsInBuildMode())
				{
					QuitGame();
				}
				else
				{
					ReturnToMothership();
				}
			}
			if (buttonType == ButtonType.Confirm)
			{
				ConfirmQuit();
			}
			if (buttonType == ButtonType.Cancel)
			{
				CancelQuit();
			}
			if (buttonType == ButtonType.ResumeGame)
			{
				guiInputController.CloseCurrentScreen();
			}
		}
	}
}
