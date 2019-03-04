internal struct Balance
{
	public uint softCurrency;

	public bool hidden;

	public Balance(uint soft)
	{
		softCurrency = soft;
		hidden = false;
	}

	public Balance(uint soft, bool h)
	{
		softCurrency = soft;
		hidden = h;
	}
}
