using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class DamagedByEnemyShieldClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DamagedByEnemyShieldDependency _dependency;

		[Inject]
		internal DestructionManager destructionManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			get;
			private set;
		}

		public void Execute()
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, _dependency.machineId);
			bool targetIsMe = playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId);
			destructionManager.PerformDestruction(_dependency.damagedCubes, playerFromMachineId, _dependency.machineId, playerFromMachineId, TargetType.Player, targetIsMe, playEffects: true, int.MaxValue);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as DamagedByEnemyShieldDependency);
			return this;
		}
	}
}
