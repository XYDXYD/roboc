using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ApplyPromoCodeResponse
{
	public bool Success
	{
		get;
		set;
	}

	public PromotionResultCode ResultCode
	{
		get;
		set;
	}

	public bool IsSerialKey
	{
		get;
		set;
	}

	public float Value
	{
		get;
		set;
	}

	public string PromoId
	{
		get;
		set;
	}

	public string CubesAwarded
	{
		get;
		set;
	}

	public string MessageStrKey
	{
		get;
		set;
	}

	public string BundleId
	{
		get;
		set;
	}

	public bool RoboPassAwarded
	{
		get;
		set;
	}

	public long CosmeticCreditsAwarded
	{
		get;
		set;
	}
}
