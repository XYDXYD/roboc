internal class LoadRobotShopSubmissionInfosResult
{
	public uint playerSubmissionCount
	{
		get;
		private set;
	}

	public uint maxSubmissionCount
	{
		get;
		private set;
	}

	public LoadRobotShopSubmissionInfosResult(uint playerSubmissionCount, uint maxSubmissionCount)
	{
		this.playerSubmissionCount = playerSubmissionCount;
		this.maxSubmissionCount = maxSubmissionCount;
	}
}
