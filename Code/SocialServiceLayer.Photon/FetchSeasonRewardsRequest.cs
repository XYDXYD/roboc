using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class FetchSeasonRewardsRequest : SocialRequest<FetchSeasonRewardsResponse>, IFetchSeasonRewardsRequest, IServiceRequest<FetchSeasonRewardsDependancy>, IAnswerOnComplete<FetchSeasonRewardsResponse>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 50;

		public FetchSeasonRewardsRequest()
			: base("strFetchSeasonRewardsErrorTitle", "strFetchSeasonRewardsErrorBody", 0)
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

		public void Inject(FetchSeasonRewardsDependancy dependency)
		{
			_userName = dependency.username;
		}

		protected override FetchSeasonRewardsResponse ProcessResponse(OperationResponse response)
		{
			int seasonMonth_ = (int)response.Parameters[49];
			int seasonYear_ = (int)response.Parameters[63];
			int robitsReward_ = (int)response.Parameters[47];
			bool hasClaimedAllRewards_ = (bool)response.Parameters[46];
			int averageSeasonExperienceForEveryoneInClan_ = (int)response.Parameters[54];
			int clansTotalExperience_ = (int)response.Parameters[55];
			string clanname_ = (string)response.Parameters[31];
			int totalSeasonXPForThisPlayer_ = (int)response.Parameters[57];
			return new FetchSeasonRewardsResponse(robitsReward_, hasClaimedAllRewards_, seasonMonth_, seasonYear_, averageSeasonExperienceForEveryoneInClan_, clansTotalExperience_, clanname_, totalSeasonXPForThisPlayer_);
		}
	}
}
