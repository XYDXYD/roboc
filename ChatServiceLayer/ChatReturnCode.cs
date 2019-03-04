namespace ChatServiceLayer
{
	public enum ChatReturnCode : short
	{
		None,
		UnexpectedError,
		Flood,
		Muted,
		NotOnline,
		DoesNotExist,
		NoConnection,
		ModeratorsOnly,
		AdminsOnly,
		SanctionAlreadyExists,
		AlreadyWarned,
		NoSanctionExists,
		MaintenanceMode,
		ChannelExists,
		IncorrectPassword,
		ChannelNotExists,
		PasswordRequired,
		ChannelExpired
	}
}
