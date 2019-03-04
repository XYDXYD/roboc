namespace Services.Web
{
	internal enum AdjustCustomGameConfigurationResponse
	{
		ConfigurationUpdated,
		UserIsNotSessionLeader,
		AdjustSessionError,
		SessionDoesntExist,
		InvalidOrUnknownField,
		InvalidFieldChoice
	}
}
