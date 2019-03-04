using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Hardware.Cosmetic
{
	internal class SpawnRemoteTauntCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private TauntDependency _dependency;

		[Inject]
		private RemoteTauntObservable _remoteTauntObservable
		{
			get;
			set;
		}

		public void Execute()
		{
			_remoteTauntObservable.Dispatch(ref _dependency);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (TauntDependency)dependency;
			return this;
		}
	}
}
