namespace SinglePlayerServiceLayer.Photon
{
	public enum SinglePlayerOperationCode : byte
	{
		LoadTdmAiRobots = 1,
		SaveTdmGameAwards = 2,
		StartEacValidation = 4,
		FinishEacValidation = 5
	}
}
