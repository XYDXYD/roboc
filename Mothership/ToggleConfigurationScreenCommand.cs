using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class ToggleConfigurationScreenCommand : ICommand
	{
		[Inject]
		private IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		public void Execute()
		{
			guiInputController.ToggleScreen(GuiScreens.RobotConfigurationScreen);
		}
	}
}
