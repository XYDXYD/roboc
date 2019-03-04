using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class LocalPlayerDestructionHandler : IInitialize, IWaitForFrameworkDestruction
	{
		private SendMachineDestroyedClientCommand _machineDestroyedCommand;

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
		public ICommandFactory commandFactory
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

		public void OnDependenciesInjected()
		{
			destructionReporter.OnPlayerSelfDestructs += OnPlayerSelfDestructs;
			destructionReporter.OnMachineKilled += OnPlayerKilled;
			_machineDestroyedCommand = commandFactory.Build<SendMachineDestroyedClientCommand>();
		}

		public void OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerSelfDestructs -= OnPlayerSelfDestructs;
			destructionReporter.OnMachineKilled -= OnPlayerKilled;
		}

		private void OnPlayerSelfDestructs(int playerId)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, playerId))
			{
				SendMachineKilled(playerId, playerId);
			}
		}

		private void OnPlayerKilled(int killedId, int shooterId)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, killedId) || localAIs.IsAIHostedLocally(killedId))
			{
				SendMachineKilled(killedId, shooterId);
			}
		}

		private void SendMachineKilled(int killedId, int shooterId)
		{
			_machineDestroyedCommand.Inject(new KillDependency(killedId, shooterId));
			_machineDestroyedCommand.Execute();
		}
	}
}
