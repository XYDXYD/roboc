namespace SinglePlayerServiceLayer.Photon
{
	public enum SinglePlayerEventCode : byte
	{
		DuplicateLogin,
		MasterSlaveDeniedCCUMax,
		CCUCheckPassCode,
		SpawnRobot,
		SpawnRobotError,
		UpdateExperience,
		NoRobotFoundError
	}
}
