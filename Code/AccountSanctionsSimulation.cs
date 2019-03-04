using Utility;

internal sealed class AccountSanctionsSimulation : AccountSanctions
{
	public override void HandleSuspensionEvent()
	{
		Console.Log("Received suspension event during battle!");
	}

	public override bool CanShowSanctionDialog()
	{
		return true;
	}
}
