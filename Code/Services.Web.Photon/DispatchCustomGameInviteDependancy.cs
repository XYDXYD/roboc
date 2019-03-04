namespace Services.Web.Photon
{
	public class DispatchCustomGameInviteDependancy
	{
		public readonly string Invitee;

		public readonly bool IsTeamA;

		public DispatchCustomGameInviteDependancy(string invitee_, bool isTeamA_)
		{
			Invitee = invitee_;
			IsTeamA = isTeamA_;
		}
	}
}
