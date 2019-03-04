using Svelto.Command;
using Svelto.IoC;

internal class ReportSocialEventCommand : ICommand
{
	private string _message;

	[Inject]
	internal ChatPresenter ChatPresenter
	{
		private get;
		set;
	}

	public ICommand Inject(string message)
	{
		_message = message;
		return this;
	}

	public void Execute()
	{
		ChatPresenter.SocialMessage(_message);
	}
}
