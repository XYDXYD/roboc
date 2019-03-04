using Authentication;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class RemoveFriendRequest : SocialRequest<IList<Friend>>, IRemoveFriendRequest, IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 3;

		public RemoveFriendRequest()
			: base("strRobocloudError", "strRemoveFriendError", 0)
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
			Friend friend = CacheDTO.friendList.Find((Friend f) => f.Name == _userName);
			if (friend != null)
			{
				CacheDTO.friendList.Remove(friend);
			}
			else
			{
				Console.LogError("Removed friend is not in user's friend list");
			}
			PhotonSocialUtility.Instance.RaiseInternalEvent<IFriendRemovedEventListener, FriendListUpdate>(new FriendListUpdate(User.Username, User.DisplayName, CacheDTO.friendList.AsReadOnly()));
			return CacheDTO.friendList.AsReadOnly();
		}
	}
}
