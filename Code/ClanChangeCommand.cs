using Svelto.Command;
using Svelto.IoC;

internal class ClanChangeCommand : ICommand
{
	private string _clanName;

	[Inject]
	internal ChatPresenter ChatPresenter
	{
		private get;
		set;
	}

	public ICommand Inject(string clanName)
	{
		_clanName = clanName;
		return this;
	}

	public void Execute()
	{
		ChatPresenter.ClanChanged(_clanName);
	}
}
