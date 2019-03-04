using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class NotifyTestResultClientCommand : IInjectableCommand<bool>, ICommand
	{
		private bool _result;

		[Inject]
		internal LobbyPresenter lobbyPresenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			lobbyPresenter.SendConnectionTestResult(_result);
		}

		public ICommand Inject(bool dependency)
		{
			_result = dependency;
			return this;
		}
	}
}
