using Svelto.Command;
using Svelto.IoC;
using Utility;

namespace Mothership.GUI.Inventory
{
	internal sealed class CubeSelectorPresenter
	{
		private CubeSelector _view;

		private readonly string SKIP_BUILD_CONFIRM_POPUP_KEY = "SkipBuildConfirmPopup";

		[Inject]
		internal IGUIInputControllerMothership guiController
		{
			private get;
			set;
		}

		[Inject]
		internal CurrentToolMode currentToolMode
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

		internal void Show()
		{
			_view.Show();
		}

		internal void Hide()
		{
			_view.Hide();
		}

		internal bool IsActive()
		{
			return _view.IsActive();
		}

		public void SetHalfSize()
		{
			_view.HalfSize();
			UIWidget halfSizeAnchorsWidget = _view.halfSizeAnchorsWidget;
			HalfScreenGUIHelper.ExactlyHalfScreenCameraOnRHS();
		}

		public void SetFullSize()
		{
			_view.FullSize();
			HalfScreenGUIHelper.Hide();
		}

		internal void RegisterView(CubeSelector cubeSelector)
		{
			_view = cubeSelector;
		}

		internal void Listen(object message)
		{
			if (message is CubeTypeID)
			{
				NewCubeSelected();
			}
			else if (message is UnlockCubeCategoryButtonData)
			{
				Console.LogError("Should only be able to unlock via Tech Tree");
			}
		}

		public void OnDataChanged()
		{
			_view.BroadcastRefreshMessage();
		}

		public void NewCubeSelected()
		{
			if (WorldSwitching.IsInBuildMode())
			{
				guiController.ShowScreen(GuiScreens.BuildMode);
			}
		}

		private void SwitchToBuildMode()
		{
			SwitchWorldDependency dependency = new SwitchWorldDependency("RC_BuildMode", _fastSwitch: false);
			commandFactory.Build<SwitchToBuildModeCommand>().Inject(dependency).Execute();
			currentToolMode.TryChangeToolMode(CurrentToolMode.ToolMode.Build);
		}
	}
}
