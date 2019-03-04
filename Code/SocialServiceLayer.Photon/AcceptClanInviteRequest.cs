using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class AcceptClanInviteRequest : SocialRequest<ClanInfoAndMembers>, IAcceptClanInviteRequest, IServiceRequest<string>, IAnswerOnComplete<ClanInfoAndMembers>, IServiceRequest
	{
		private string _clanName;

		protected override byte OperationCode => 36;

		public AcceptClanInviteRequest()
			: base("strAcceptClanInviteErrorTitle", "strAcceptClanInviteErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(31, _clanName);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		protected override ClanInfoAndMembers ProcessResponse(OperationResponse response)
		{
			try
			{
				Hashtable[] array = (Hashtable[])response.Parameters[36];
				ClanMember[] array2 = new ClanMember[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					Hashtable clanMemberRaw = array[i];
					array2[i] = ClanMember.FromHashtable(clanMemberRaw);
				}
				float num = (float)response.Parameters[51];
				string clanName = (string)response.Parameters[31];
				string clanDescription = (string)response.Parameters[32];
				ClanType clanType = (ClanType)response.Parameters[34];
				ClanInfo clanInfo = new ClanInfo(clanName, clanDescription, clanType, array2.Length);
				int num3 = clanInfo.ClanAvatarNumber = (int)response.Parameters[33];
				CacheDTO.MyClanMembers = new Dictionary<string, ClanMember>();
				foreach (ClanMember clanMember in array2)
				{
					CacheDTO.MyClanMembers.Add(clanMember.Name, clanMember);
				}
				CacheDTO.XPConversionFactor = num;
				CacheDTO.MyClanInfo = clanInfo;
				CacheDTO.ClanInvites = new Dictionary<string, ClanInvite>();
				return new ClanInfoAndMembers(clanInfo, array2, num);
			}
			catch (Exception ex)
			{
				Console.LogError("error processing accept clan invite:" + ex.Message);
				throw new Exception("Caught exceptin accepting a clan invite:", ex);
			}
		}

		public void Inject(string clanName)
		{
			_clanName = clanName;
		}
	}
}
