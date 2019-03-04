using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using Utility;

internal class ReceiveLoadingProgressClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private LoadingProgressDependency _dependency;

	[Inject]
	internal BattleLoadProgress BattleLoadProgress
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (LoadingProgressDependency)dependency;
		return this;
	}

	public void Execute()
	{
		Console.Log($"Player '{_dependency.UserName}' {_dependency.Progress * 100f}% loaded");
		string userName = _dependency.UserName;
		float progress = _dependency.Progress;
		BattleLoadProgress.SetPlayerProgress(userName, progress);
	}
}
