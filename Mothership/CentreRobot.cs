using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class CentreRobot : IHandleEditingInput, IInputComponent, IComponent
	{
		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership inputController
		{
			private get;
			set;
		}

		public void HandleEditingInput(InputEditingData data)
		{
			if (inputController.GetActiveScreen() == GuiScreens.BuildMode && data[EditingInputAxis.CENTRE_ROBOT] != 0f)
			{
				commandFactory.Build<CentreRobotCommand>().Execute();
			}
		}
	}
}
