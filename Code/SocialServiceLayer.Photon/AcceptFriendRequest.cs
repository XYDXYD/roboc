using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class AcceptFriendRequest : SocialRequest<IList<Friend>>, IAcceptFriendRequest, IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 1;

		public AcceptFriendRequest()
			: base("strRobocloudError", "strAcceptInviteError", 0)
		{
		}

		public void Inject(string userName)
		{
			_userName = userName;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[1] = _userName;
			val.Parameters = dictionary;
			return val;
		}

		protected override IList<Friend> ProcessResponse(OperationResponse response)
		{
			bool isOnline = (bool)response.Parameters[2];
			Friend friend = CacheDTO.friendList.Find((Friend f) => f.Name == _userName);
			if (friend != null)
			{
				friend.IsOnline = isOnline;
				friend.InviteStatus = FriendInviteStatus.Accepted;
			}
			return CacheDTO.friendList.AsReadOnly();
		}
	}
}
