using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetFriendListRequest : SocialRequest<GetFriendListResponse>, IGetFriendListRequest, IServiceRequest, IAnswerOnComplete<GetFriendListResponse>
	{
		protected override byte OperationCode => 4;

		public bool ForceRefresh
		{
			get;
			set;
		}

		public GetFriendListRequest()
			: base("strRobocloudError", "strGetFriendListError", 0)
		{
		}

		public override void Execute()
		{
			if (ForceRefresh)
			{
				CacheDTO.friendList = null;
				base.Execute();
			}
			else if (CacheDTO.friendList != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(new GetFriendListResponse(CacheDTO.friendList.AsReadOnly()));
				}
			}
			else
			{
				base.Execute();
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override GetFriendListResponse ProcessResponse(OperationResponse response)
		{
			Friend[] array = (Friend[])response.Parameters[5];
			CacheDTO.friendList = new List<Friend>(array);
			Hashtable[] array2 = (Hashtable[])response.Parameters[76];
			foreach (Friend friend in array)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					if (friend.Name == (string)array2[j].get_Item((object)"name"))
					{
						friend.AvatarInfo = new AvatarInfo((bool)array2[j].get_Item((object)"useCustomAvatar"), (int)array2[j].get_Item((object)"avatarId"));
						break;
					}
				}
			}
			return new GetFriendListResponse(CacheDTO.friendList.AsReadOnly());
		}
	}
}
