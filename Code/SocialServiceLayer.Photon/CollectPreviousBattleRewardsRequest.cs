using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class CollectPreviousBattleRewardsRequest : SocialRequest<bool>, ICollectPreviousBattleRewardsRequest, IServiceRequest<string>, IAnswerOnComplete<bool>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 55;

		public CollectPreviousBattleRewardsRequest()
			: base("strCollectPreviousBattleRewardsRequestErrorTitle", "strCollectPreviousBattleRewardsRequestErrorBody", 0)
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

		protected override bool ProcessResponse(OperationResponse response)
		{
			return (bool)response.Parameters[60];
		}
	}
}
