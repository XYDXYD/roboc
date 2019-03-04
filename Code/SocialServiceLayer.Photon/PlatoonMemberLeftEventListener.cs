using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonMemberLeftEventListener : SocialEventListener<string, PlatoonMember.MemberStatus>, IPlatoonMemberLeftEventListener, IServiceEventListener<string, PlatoonMember.MemberStatus>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonMemberLeft;

		protected override void ParseData(EventData eventData)
		{
			string text = (string)eventData.Parameters[1];
			string data = (string)eventData.Parameters[75];
			if (!CacheDTO.platoon.HasPlayer(text))
			{
				return;
			}
			PlatoonMember.MemberStatus data2 = PlatoonMember.MemberStatus.Invited;
			for (int i = 0; i < CacheDTO.platoon.members.Length; i++)
			{
				PlatoonMember platoonMember = CacheDTO.platoon.members[i];
				if (platoonMember.Name == text)
				{
					data2 = platoonMember.Status;
					break;
				}
			}
			if (CacheDTO.platoon.Size > 2)
			{
				CacheDTO.platoon.RemoveMember(text);
			}
			else
			{
				CacheDTO.platoon = new Platoon();
			}
			Invoke(data, data2);
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}
	}
}
