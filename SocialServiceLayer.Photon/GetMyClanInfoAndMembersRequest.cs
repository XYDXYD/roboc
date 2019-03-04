using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetMyClanInfoAndMembersRequest : SocialRequest<ClanInfoAndMembers>, IGetMyClanInfoAndMembersRequest, IServiceRequest, IAnswerOnComplete<ClanInfoAndMembers>, ITask, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public bool ForceRefresh
		{
			get;
			set;
		}

		protected override byte OperationCode => 33;

		public GetMyClanInfoAndMembersRequest()
			: base("strGetClanInfoErrorTitle", "strGetClanInfoErrorBody", 0)
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

		protected override ClanInfoAndMembers ProcessResponse(OperationResponse response)
		{
			isDone = true;
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
			int clanAvatarNumber = (int)response.Parameters[33];
			ClanInfo clanInfo = new ClanInfo(clanName, clanDescription, clanType, array2.Length);
			clanInfo.ClanAvatarNumber = clanAvatarNumber;
			CacheDTO.XPConversionFactor = num;
			CacheDTO.MyClanInfo = clanInfo;
			CacheDTO.MyClanMembers = new Dictionary<string, ClanMember>(StringComparer.OrdinalIgnoreCase);
			foreach (ClanMember clanMember in array2)
			{
				CacheDTO.MyClanMembers.Add(clanMember.Name, clanMember);
			}
			return new ClanInfoAndMembers(clanInfo, array2, num);
		}

		protected override void OnFailed(Exception exception)
		{
			isDone = true;
			base.OnFailed(exception);
		}

		public override void Execute()
		{
			if (ForceRefresh)
			{
				CacheDTO.MyClanInfo = null;
				CacheDTO.MyClanMembers = null;
				base.Execute();
				return;
			}
			if (CacheDTO.MyClanInfo != null && CacheDTO.MyClanMembers != null)
			{
				float? xPConversionFactor = CacheDTO.XPConversionFactor;
				if (xPConversionFactor.HasValue)
				{
					isDone = true;
					if (base.answer != null && base.answer.succeed != null)
					{
						base.answer.succeed(new ClanInfoAndMembers(CacheDTO.MyClanInfo, CacheDTO.MyClanMembers.Values.ToArray(), CacheDTO.XPConversionFactor.Value));
					}
					return;
				}
			}
			base.Execute();
		}
	}
}
