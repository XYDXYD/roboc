using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SpawnStunMachineEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private NetworkStunnedMachineEffectDependency _dependency;

		private NetworkStunnedMachineData _data = new NetworkStunnedMachineData(0, isStunned_: false);

		[Inject]
		internal NetworkStunMachineObservable empEffectObservable
		{
			private get;
			set;
		}

		public void Execute()
		{
			_data.SetValues(_dependency.machineId, _dependency.isStunned);
			empEffectObservable.Dispatch(ref _data);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as NetworkStunnedMachineEffectDependency);
			return this;
		}
	}
}
