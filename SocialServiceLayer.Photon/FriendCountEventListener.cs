using ExitGames.Client.Photon;

namespace SocialServiceLayer.Photon
{
	internal class FriendCountEventListener : SocialEventListener<int>
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.FriendCount;

		protected override void ParseData(EventData eventData)
		{
			int data = (int)eventData.Parameters[4];
			Invoke(data);
		}
	}
}
