using InputMask;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Mothership
{
	internal sealed class GUIShortcutTicker : ITickable, IWaitForFrameworkInitialization, ITickableBase
	{
		private IGUIInputControllerMothership _inputController;

		[Inject]
		internal IInputActionMask inputActionMask
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

		public GUIShortcutTicker(IGUIInputControllerMothership inputController)
		{
			_inputController = inputController;
		}

		public void Tick(float delta)
		{
			ShortCutMode shortCutMode = _inputController.GetShortCutMode();
			switch (shortCutMode)
			{
			case ShortCutMode.NoKeyboardInputAllowed:
				return;
			case ShortCutMode.AnyKeyClear:
				if (Input.get_anyKeyDown())
				{
					_inputController.CloseCurrentScreen();
				}
				break;
			default:
				if (InputRemapper.Instance.GetButtonDown("Quit") && shortCutMode != ShortCutMode.AllExceptEsc && inputActionMask.InputIsAvailable(UserInputCategory.GUIShortcutInputAxis, 0))
				{
					_inputController.HandleQuitPressed();
				}
				break;
			}
			if ((shortCutMode == ShortCutMode.BuildShortCuts || shortCutMode == ShortCutMode.AllShortCuts || shortCutMode == ShortCutMode.OnlyGUINoSwitching || shortCutMode == ShortCutMode.AllExceptEsc) && InputRemapper.Instance.GetButtonDown("Inventory") && inputActionMask.InputIsAvailable(UserInputCategory.GUIShortcutInputAxis, 1))
			{
				_inputController.ToggleScreenViaShortcut(GuiScreens.InventoryScreen);
			}
			if (shortCutMode != ShortCutMode.AllShortCuts && shortCutMode != ShortCutMode.OnlyGUINoSwitching && shortCutMode != ShortCutMode.AllExceptEsc)
			{
				return;
			}
			if (InputRemapper.Instance.GetButtonDown("Drive robot"))
			{
				if (inputActionMask.InputIsAvailable(UserInputCategory.GUIShortcutInputAxis, 3))
				{
					commandFactory.Build<SwitchToMainMenuCommand>().Execute();
				}
			}
			else if (InputRemapper.Instance.GetButtonDown("Select robot"))
			{
				if (inputActionMask.InputIsAvailable(UserInputCategory.GUIShortcutInputAxis, 4))
				{
					_inputController.ToggleScreenViaShortcut(GuiScreens.Garage);
				}
			}
			else if (InputRemapper.Instance.GetButtonDown("Robot Factory") && inputActionMask.InputIsAvailable(UserInputCategory.GUIShortcutInputAxis, 4))
			{
				_inputController.ToggleScreenViaShortcut(GuiScreens.RobotShop);
			}
		}

		private void OnLoadingFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
		}
	}
}
