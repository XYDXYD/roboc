using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class OnSelfDestructClientCommand : ICommand
	{
		[Inject]
		public PlayerTeamsContainer playerTeams
		{
			private get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		public void Execute()
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerTeams.localPlayerId);
			destructionReporter.PlayerSelfDestructed(playerTeams.localPlayerId, activeMachine, isMe: true);
		}
	}
}
