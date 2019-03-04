using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class InviteToClanRequest : SocialRequest<ClanMember[]>, IInviteToClanRequest, IServiceRequest<string>, IAnswerOnComplete<ClanMember[]>, IServiceRequest
	{
		private string _invitee;

		protected override byte OperationCode => 35;

		public InviteToClanRequest()
			: base("strInviteToClanErrorTitle", "strInviteToClanErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(1, _invitee);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		public void Inject(string invitee)
		{
			_invitee = invitee;
		}

		protected override ClanMember[] ProcessResponse(OperationResponse response)
		{
			string name = (string)response.get_Item((byte)1);
			string displayName = (string)response.get_Item((byte)75);
			bool flag = (bool)response.get_Item((byte)13);
			int avatarId = (!flag) ? ((int)response.get_Item((byte)14)) : 0;
			int seasonXP = (int)response.get_Item((byte)48);
			ClanMember clanMember = new ClanMember(name, displayName, ClanMemberState.Invited, new AvatarInfo(flag, avatarId), ClanMemberRank.Member, isOnline: false, seasonXP);
			CacheDTO.MyClanMembers.Add(clanMember.Name, clanMember);
			return CacheDTO.MyClanMembers.Values.ToArray();
		}
	}
}
