using Svelto.Command;
using Svelto.IoC;

internal class LeaveClanChatChannelCommand : ICommand
{
	[Inject]
	internal ChatPresenter ChatPresenter
	{
		private get;
		set;
	}

	public void Execute()
	{
		ChatPresenter.ClanChanged(null);
	}
}
