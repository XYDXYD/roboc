using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class ClanRenamedEventListener : SocialEventListener<ClanRenameDependency>, IClanRenamedEventListener, IServiceEventListener<ClanRenameDependency>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.ClanRenamed;

		protected override void ParseData(EventData eventData)
		{
			string oldClanName = (string)eventData.Parameters[31];
			string newClanName = (string)eventData.Parameters[44];
			string adminName = (string)eventData.Parameters[1];
			Invoke(new ClanRenameDependency(oldClanName, newClanName, adminName));
		}
	}
}
