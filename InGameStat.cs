internal struct InGameStat
{
	private InGameStatId _id;

	private uint _amount;

	private uint _score;

	public InGameStatId ID => _id;

	public uint Amount => _amount;

	public uint Score => _score;

	public InGameStat(InGameStatId id, uint amount, uint score)
	{
		_id = id;
		_amount = amount;
		_score = score;
	}
}
