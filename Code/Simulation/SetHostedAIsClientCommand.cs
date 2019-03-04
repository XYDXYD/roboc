using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal class SetHostedAIsClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private PlayerIDsDependency _dependency;

		[Inject]
		internal LocalAIsContainer _multiplayerAIsContainer
		{
			get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (PlayerIDsDependency)dependency;
			return this;
		}

		public void Execute()
		{
			_multiplayerAIsContainer.OnReceiveHostedAIs(_dependency);
		}
	}
}
