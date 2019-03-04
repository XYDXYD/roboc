using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetNewPreviousBattleRewardsRequest : SocialRequest<GetNewPreviousBattleRequestData>, IGetNewPreviousBattleRewardsRequest, IServiceRequest<string>, IAnswerOnComplete<GetNewPreviousBattleRequestData>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 53;

		public GetNewPreviousBattleRewardsRequest()
			: base("strGetNewPreviousBattleRewardsRequestErrorTitle", "strGetNewPreviousBattleRewardsRequestErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(1, _userName);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		public void Inject(string dependency)
		{
			_userName = dependency;
		}

		protected override GetNewPreviousBattleRequestData ProcessResponse(OperationResponse response)
		{
			int newSeasonXP_ = (int)response.Parameters[57];
			int xpAwardBase_ = (int)response.Parameters[58];
			int xpAwardPremiumPart_ = (int)response.Parameters[59];
			int xpAwardPartyPart_ = (int)response.Parameters[61];
			int xpAwardTierPart_ = (int)response.Parameters[52];
			int robitsTotal_ = (int)response.Parameters[50];
			int averageXpForEveryone_ = (int)response.Parameters[54];
			int clanTotalXP_ = (int)response.Parameters[55];
			float xpMultiplier_ = (float)response.Parameters[64];
			int robits_ = (int)response.Parameters[68];
			int premiumRobits_ = (int)response.Parameters[69];
			float longPlayMultiplier_ = 0f;
			if (response.Parameters.ContainsKey(72))
			{
				longPlayMultiplier_ = (float)response.Parameters[72];
			}
			return new GetNewPreviousBattleRequestData(newSeasonXP_, xpAwardBase_, xpAwardPremiumPart_, xpAwardPartyPart_, xpAwardTierPart_, robitsTotal_, averageXpForEveryone_, clanTotalXP_, xpMultiplier_, robits_, premiumRobits_, longPlayMultiplier_);
		}
	}
}
