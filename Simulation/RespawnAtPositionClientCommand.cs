using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class RespawnAtPositionClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SpawnPointDependency _dependency;

		[Inject]
		public MachineRespawner machineRespawner
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

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (SpawnPointDependency)dependency;
			return this;
		}

		public void Execute()
		{
			int primaryMachineId = PlayerMachinesContainer.GetPrimaryMachineId(_dependency.owner);
			machineRespawner.StartRespawn(_dependency);
		}
	}
}
