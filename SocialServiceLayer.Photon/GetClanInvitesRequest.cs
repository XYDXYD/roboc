using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetClanInvitesRequest : SocialRequest<ClanInvite[]>, IGetClanInvitesRequest, IServiceRequest, IAnswerOnComplete<ClanInvite[]>
	{
		public bool ForceRefresh
		{
			get;
			set;
		}

		protected override byte OperationCode => 39;

		public GetClanInvitesRequest()
			: base("strGetClanInvitesErrorTitle", "strGetClanInvitesErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		public override void Execute()
		{
			if (ForceRefresh)
			{
				CacheDTO.ClanInvites = null;
				base.Execute();
			}
			else if (CacheDTO.ClanInvites != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(CacheDTO.ClanInvites.Values.ToArray());
				}
			}
			else
			{
				base.Execute();
			}
		}

		protected override ClanInvite[] ProcessResponse(OperationResponse response)
		{
			Hashtable[] array = (Hashtable[])response.Parameters[42];
			ClanInvite[] array2 = new ClanInvite[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Hashtable val = array[i];
				string inviterName = (string)val.get_Item((object)"userName");
				string inviterDisplayName = (string)val.get_Item((object)"displayName");
				string clanName = (string)val.get_Item((object)"clanName");
				int clanSize = (int)val.get_Item((object)"clanSize");
				bool useCustomAvatar = (bool)val.get_Item((object)"useCustomAvatar");
				int avatarId = (int)val.get_Item((object)"avatarId");
				array2[i] = new ClanInvite(inviterName, inviterDisplayName, clanName, clanSize, new AvatarInfo(useCustomAvatar, avatarId));
			}
			CacheDTO.ClanInvites = new Dictionary<string, ClanInvite>();
			ClanInvite[] array3 = array2;
			foreach (ClanInvite clanInvite in array3)
			{
				CacheDTO.ClanInvites.Add(clanInvite.ClanName, clanInvite);
			}
			return array2;
		}
	}
}
