namespace SinglePlayerServiceLayer.Photon
{
	public enum SinglePlayerErrorCode : short
	{
		None,
		DatabaseError,
		UnexpectedError,
		WrongNumberOfAuthParams,
		MaintenanceMode,
		DuplicateLogin
	}
}
