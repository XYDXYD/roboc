public class SwitchSinglePlayerCampaignWorldDependency
{
	public readonly string planetToLoad;

	public readonly string campaignName;

	public readonly string campaignID;

	public readonly int campaignDifficulty;

	public GameModeType gameModeType => GameModeType.Campaign;

	public SwitchSinglePlayerCampaignWorldDependency(string planetToLoad_, string campaignId_, int difficulty_, string campaignName_)
	{
		planetToLoad = planetToLoad_;
		campaignID = campaignId_;
		campaignDifficulty = difficulty_;
		campaignName = campaignName_;
	}
}
