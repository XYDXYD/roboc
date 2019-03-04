using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetClanInfoAndMembersRequest : SocialRequest<ClanInfoAndMembers>, IGetClanInfoAndMembersRequest, IServiceRequest<string>, IAnswerOnComplete<ClanInfoAndMembers>, ITask, IServiceRequest, IAbstractTask
	{
		private string _clanName;

		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 33;

		public GetClanInfoAndMembersRequest()
			: base("strGetClanInfoErrorTitle", "strGetClanInfoErrorBody", 0)
		{
		}

		public void Inject(string clanName)
		{
			_clanName = clanName;
		}

		public override void Execute()
		{
			if (CacheDTO.OtherClanInfo != null && CacheDTO.OtherClanMembers != null)
			{
				float? xPConversionFactor = CacheDTO.XPConversionFactor;
				if (xPConversionFactor.HasValue && CacheDTO.OtherClanInfo.ClanName.Equals(_clanName, StringComparison.OrdinalIgnoreCase))
				{
					base.answer.succeed(new ClanInfoAndMembers(CacheDTO.OtherClanInfo, CacheDTO.OtherClanMembers, CacheDTO.XPConversionFactor.Value));
					isDone = true;
					return;
				}
			}
			base.Execute();
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

		protected override void OnFailed(Exception exception)
		{
			base.OnFailed(exception);
			isDone = true;
		}

		protected override ClanInfoAndMembers ProcessResponse(OperationResponse response)
		{
			if (!response.Parameters.ContainsKey(31))
			{
				return null;
			}
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
			CacheDTO.XPConversionFactor = num;
			CacheDTO.OtherClanInfo = clanInfo;
			CacheDTO.OtherClanMembers = array2;
			isDone = true;
			return new ClanInfoAndMembers(clanInfo, array2, num);
		}
	}
}
