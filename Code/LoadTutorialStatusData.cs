internal struct LoadTutorialStatusData
{
	public readonly bool inProgress;

	public readonly bool skipped;

	public readonly bool completed;

	public LoadTutorialStatusData(bool inProgress_, bool skipped_, bool completed_)
	{
		inProgress = inProgress_;
		skipped = skipped_;
		completed = completed_;
	}
}
