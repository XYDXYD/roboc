using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonMemberStatusChangedEventListener : SocialEventListener<string, PlatoonStatusChangedData>, IPlatoonMemberStatusChangedEventListener, IServiceEventListener<string, PlatoonStatusChangedData>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonMemberStatusChanged;

		protected override void ParseData(EventData eventData)
		{
			if (CacheDTO.platoon.isInPlatoon)
			{
				string text = (string)eventData.Parameters[1];
				string data = (string)eventData.Parameters[75];
				PlatoonMember.MemberStatus memberStatus = (PlatoonMember.MemberStatus)eventData.Parameters[3];
				PlatoonMember.MemberStatus status = CacheDTO.platoon.GetMemberData(text).Status;
				CacheDTO.platoon.SetMemberStatus(text, memberStatus);
				PlatoonStatusChangedData data2 = default(PlatoonStatusChangedData);
				data2.oldStatus = status;
				data2.newStatus = memberStatus;
				Invoke(data, data2);
				PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			}
		}
	}
}
