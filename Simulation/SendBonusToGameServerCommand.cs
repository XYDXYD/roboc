using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SendBonusToGameServerCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal ForceFlushBonusObserver forceFlushBonusObserver
		{
			private get;
			set;
		}

		public void Execute()
		{
			forceFlushBonusObserver.Flush();
		}
	}
}
