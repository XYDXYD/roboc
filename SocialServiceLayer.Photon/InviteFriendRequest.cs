using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class InviteFriendRequest : SocialRequest<IList<Friend>>, IInviteFriendRequest, IServiceRequest<string>, IAnswerOnComplete<IList<Friend>>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 0;

		public InviteFriendRequest()
			: base("strRobocloudError", "strInviteFriendError", 0)
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
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Expected O, but got Unknown
			string text = (string)response.Parameters[1];
			string displayName = (string)response.Parameters[75];
			if (CacheDTO.friendList != null)
			{
				int num = -1;
				for (int i = 0; i < CacheDTO.friendList.Count; i++)
				{
					if (CacheDTO.friendList[i].Name == text)
					{
						Console.LogWarning("Invitee already existed - overwriting to match data on server");
						CacheDTO.friendList[i].InviteStatus = FriendInviteStatus.InviteSent;
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					CacheDTO.friendList.Add(new Friend(text, displayName, FriendInviteStatus.InviteSent));
					num = CacheDTO.friendList.Count - 1;
				}
				if (response.Parameters.ContainsKey(31))
				{
					CacheDTO.friendList[num].ClanName = (string)response.Parameters[31];
				}
				Hashtable val = response.Parameters[9];
				CacheDTO.friendList[num].AvatarInfo = new AvatarInfo((bool)val.get_Item((object)"useCustomAvatar"), (int)val.get_Item((object)"avatarId"));
				return CacheDTO.friendList.AsReadOnly();
			}
			Console.LogWarning("Friend list wasn't loaded before invite request was sent - loading now");
			new GetFriendListRequest().SetAnswer(base.answer).Execute();
			return null;
		}

		protected override void OnFailed(Exception exception)
		{
			_serviceBehaviour.reasonID = null;
			_serviceBehaviour.serverReason = StringTableBase<StringTable>.Instance.GetReplaceString(((SocialErrorCode)_serviceBehaviour.errorCode).ToString(), "{PLAYER}", _userName);
			base.OnFailed(exception);
		}
	}
}
