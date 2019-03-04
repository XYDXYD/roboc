internal class SaveGameAwardsRequestDependency
{
	public readonly GameResult gameResult;

	public readonly float longPlayMultiplier;

	public readonly string campaignId;

	public readonly int campaignDifficulty;

	public SaveGameAwardsRequestDependency(GameResult gameResult_, float longPlayMultiplier_, string campaignId_, int campaignDifficulty_)
	{
		gameResult = gameResult_;
		longPlayMultiplier = longPlayMultiplier_;
		campaignId = campaignId_;
		campaignDifficulty = campaignDifficulty_;
	}
}
