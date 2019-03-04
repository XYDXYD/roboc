using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SwitchToMothershipCommand : IInjectableCommand<bool>, ICommand
	{
		private bool _fastSwitch;

		[Inject]
		internal WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		internal GameStateClient gameStateClient
		{
			private get;
			set;
		}

		public SwitchToMothershipCommand()
		{
			_fastSwitch = false;
		}

		public ICommand Inject(bool fastSwitch)
		{
			_fastSwitch = fastSwitch;
			return this;
		}

		public void Execute()
		{
			gameStateClient.ChangeStateToGameEnded(gameEnded: true);
			worldSwitching.SwitchToLastMothershipGameMode(_fastSwitch);
		}
	}
}
