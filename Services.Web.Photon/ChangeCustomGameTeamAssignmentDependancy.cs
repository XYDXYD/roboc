namespace Services.Web.Photon
{
	internal class ChangeCustomGameTeamAssignmentDependancy
	{
		public readonly string sourcePlayer;

		public readonly string TargetPlayer;

		public readonly bool DestinationIsTeamB;

		public ChangeCustomGameTeamAssignmentDependancy(string sourcePlayer_, string targetPlayer_, bool destinationIsTeamB_)
		{
			sourcePlayer = sourcePlayer_;
			TargetPlayer = targetPlayer_;
			DestinationIsTeamB = destinationIsTeamB_;
		}
	}
}
