using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class GetMyClanInfoRequest : SocialRequest<ClanInfo>, IGetMyClanInfoRequest, IServiceRequest, IAnswerOnComplete<ClanInfo>
	{
		protected override byte OperationCode => 43;

		public GetMyClanInfoRequest()
			: base("strGetClanInfoErrorTitle", "strGetClanInfoErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}

		protected override ClanInfo ProcessResponse(OperationResponse response)
		{
			if (!response.Parameters.ContainsKey(31))
			{
				return null;
			}
			string clanName = (string)response.Parameters[31];
			string clanDescription = (string)response.Parameters[32];
			ClanType clanType = (ClanType)response.Parameters[34];
			CacheDTO.MyClanInfo = new ClanInfo(clanName, clanDescription, clanType);
			int clanAvatarNumber = (int)response.Parameters[33];
			CacheDTO.MyClanInfo.ClanAvatarNumber = clanAvatarNumber;
			return CacheDTO.MyClanInfo;
		}

		public override void Execute()
		{
			if (CacheDTO.MyClanInfo != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(CacheDTO.MyClanInfo);
				}
			}
			else
			{
				base.Execute();
			}
		}
	}
}
