using Simulation;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class HandleNetworkErrorClientCommand : IDispatchableCommand, ICommand
{
	[Inject]
	internal BonusManager bonusFlushManager
	{
		private get;
		set;
	}

	public void Execute()
	{
		bonusFlushManager.ConnectionError();
	}
}
