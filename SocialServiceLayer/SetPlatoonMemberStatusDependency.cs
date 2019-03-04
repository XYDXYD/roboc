namespace SocialServiceLayer
{
	internal struct SetPlatoonMemberStatusDependency
	{
		public readonly string UserName;

		public readonly PlatoonMember.MemberStatus Status;

		public SetPlatoonMemberStatusDependency(string userName, PlatoonMember.MemberStatus status)
		{
			UserName = userName;
			Status = status;
		}
	}
}
