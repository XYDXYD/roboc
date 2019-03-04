internal struct UpdateTutorialStatusData
{
	public readonly bool inProgress;

	public readonly bool skipped;

	public readonly bool completed;

	public UpdateTutorialStatusData(bool inProgress_, bool skipped_, bool completed_)
	{
		inProgress = inProgress_;
		skipped = skipped_;
		completed = completed_;
	}
}
