using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct Wallet
{
	public long RobitsBalance
	{
		get;
		set;
	}

	public long CosmeticCreditsBalance
	{
		get;
		set;
	}

	public Wallet(long robitsBalance, long cosmeticCreditsBalance)
	{
		this = default(Wallet);
		RobitsBalance = robitsBalance;
		CosmeticCreditsBalance = cosmeticCreditsBalance;
	}
}
