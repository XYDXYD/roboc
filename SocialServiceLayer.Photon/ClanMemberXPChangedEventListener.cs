using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class ClanMemberXPChangedEventListener : SocialEventListener<ClanMemberXPChangedEventContent>, IClanMemberXPChangedEventListener, IServiceEventListener<ClanMemberXPChangedEventContent>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanMemberXPChanged;

		protected override void ParseData(EventData eventData)
		{
			string memberName_ = (string)eventData.Parameters[1];
			int newXP_ = (int)eventData.Parameters[48];
			Invoke(new ClanMemberXPChangedEventContent(memberName_, newXP_));
		}
	}
}
