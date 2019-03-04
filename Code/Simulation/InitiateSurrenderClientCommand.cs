using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class InitiateSurrenderClientCommand : IInjectableCommand<int>, ICommand
	{
		private int _playerId;

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		internal SurrenderControllerClient surrenderControllerClient
		{
			private get;
			set;
		}

		[Inject]
		internal MultiplayerGameTimerClient multiplayerGameTimerClient
		{
			private get;
			set;
		}

		public ICommand Inject(int dependency)
		{
			_playerId = dependency;
			return this;
		}

		public void Execute()
		{
			InitiateSurrenderDependency dependency = new InitiateSurrenderDependency(_playerId, multiplayerGameTimerClient.GetElapsedTime());
			eventManagerClient.SendEventToServer(NetworkEvent.SurrenderRequest, dependency);
			surrenderControllerClient.SurrenderInitiated();
		}
	}
}
