using Mothership;

internal sealed class AccountSanctionsMothership : AccountSanctions
{
	public override void HandleSuspensionEvent()
	{
		base.CommandFactory.Build<LeaveMatchmakingQueueCommand>().Execute();
	}

	public override bool CanShowSanctionDialog()
	{
		return shouldShowSanctionDialog;
	}
}
