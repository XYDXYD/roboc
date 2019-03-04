using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class KillTracker : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public INetworkEventManagerClient eventManagerClient
		{
			private get;
			set;
		}

		[Inject]
		public LocalAIsContainer localAIs
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineKilled += HandleOnMachineKilled;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= HandleOnMachineKilled;
		}

		private void HandleOnMachineKilled(int ownerId, int shooterId)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, ownerId) || localAIs.IsAIHostedLocally(ownerId))
			{
				eventManagerClient.SendEventToServer(NetworkEvent.KillBonusRequest, new KillDependency(ownerId, shooterId));
			}
		}
	}
}
