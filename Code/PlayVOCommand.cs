using Simulation;
using Svelto.Command;
using Svelto.IoC;

internal class PlayVOCommand : IInjectableCommand<string>, ICommand
{
	private string _eventName;

	[Inject]
	internal VOManager voManager
	{
		private get;
		set;
	}

	public void Execute()
	{
		voManager.PlayVO(_eventName);
	}

	public ICommand Inject(string eventName)
	{
		_eventName = eventName;
		return this;
	}
}
