using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class NewRobotOptionsPresenter
	{
		private NewRobotOptionsView _view;

		[Inject]
		private ICommandFactory commandFactory
		{
			get;
			set;
		}

		[Inject]
		private IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		public void SetView(NewRobotOptionsView view)
		{
			_view = view;
		}

		public void Show(bool enable)
		{
			_view.Show(enable);
		}

		public bool IsActive()
		{
			return _view.get_gameObject().get_activeSelf();
		}

		public void ButtonClicked(ButtonType buttonType)
		{
			switch (buttonType)
			{
			case ButtonType.AddNewGarageSlot:
				commandFactory.Build<AddNewGarageSlotCommand>().Execute();
				break;
			case ButtonType.FromGarageToShop:
				guiInputController.ToggleScreenViaShortcut(GuiScreens.RobotShop);
				break;
			case ButtonType.PrebuiltRobotScreen:
				guiInputController.ShowScreen(GuiScreens.PrebuiltRobotScreen);
				break;
			case ButtonType.Close:
				guiInputController.ToggleCurrentScreen();
				break;
			}
		}
	}
}
