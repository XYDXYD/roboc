using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct MegabotRefundInfo
{
	public int refundAmount
	{
		get;
		private set;
	}

	public MegabotRefundInfo(int refundAmount)
	{
		this = default(MegabotRefundInfo);
		this.refundAmount = refundAmount;
	}
}
