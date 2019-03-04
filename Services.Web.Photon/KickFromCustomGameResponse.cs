namespace Services.Web.Photon
{
	internal enum KickFromCustomGameResponse
	{
		SessionNoLongerExists,
		KickTargetIsNotInsession,
		UserIsNotSessionLeader,
		UserRemovedFromSession,
		ErrorKickingFromSession
	}
}
