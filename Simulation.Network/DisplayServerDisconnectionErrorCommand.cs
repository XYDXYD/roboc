using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Network
{
	internal sealed class DisplayServerDisconnectionErrorCommand : IDispatchableCommand, ICommand
	{
		private GenericErrorData _errorData;

		private bool _triggeredSwitch;

		[Inject]
		public WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public GameStateClient gameStateClient
		{
			private get;
			set;
		}

		public DisplayServerDisconnectionErrorCommand()
		{
			_errorData = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strNetworkError"), StringTableBase<StringTable>.Instance.GetString("strDisconnectedGameServer"), StringTableBase<StringTable>.Instance.GetString("strOK"), ReturnToMothership);
		}

		public void Execute()
		{
			if (!gameStateClient.hasGameEnded)
			{
				RemoteLogger.Error("disconnection in game", "Client disconnected from gameserver", null);
				guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				ErrorWindow.ShowErrorWindow(_errorData);
			}
		}

		private void ReturnToMothership()
		{
			if (!_triggeredSwitch)
			{
				gameStateClient.ChangeStateToGameEnded(GameStateResult.DisconnectByClient);
				SwitchToMothershipCommand switchToMothershipCommand = commandFactory.Build<SwitchToMothershipCommand>();
				switchToMothershipCommand.Execute();
				_triggeredSwitch = true;
			}
		}
	}
}
