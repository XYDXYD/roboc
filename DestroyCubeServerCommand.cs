using Simulation;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

internal sealed class DestroyCubeServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	[Inject]
	public DestructionManager destructionManager
	{
		private get;
		set;
	}

	[Inject]
	public NetworkMachineManager machineManager
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

	public IDispatchableCommand Inject(object dependency)
	{
		return this;
	}

	public void Execute()
	{
	}
}
