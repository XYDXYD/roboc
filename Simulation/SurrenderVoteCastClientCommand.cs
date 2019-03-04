using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SurrenderVoteCastClientCommand : IInjectableCommand<bool>, ICommand
	{
		private bool _surrender;

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		public ICommand Inject(bool dependency)
		{
			_surrender = dependency;
			return this;
		}

		public void Execute()
		{
			SurrenderVoteDependency dependency = new SurrenderVoteDependency(_surrender);
			eventManagerClient.SendEventToServer(NetworkEvent.SurrenderVoteCast, dependency);
		}
	}
}
