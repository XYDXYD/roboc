using ExitGames.Client.Photon;
using LobbyServiceLayer;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class SaveCampaignGameAwardsRequest : WebServicesRequest, ISaveCampaignGameAwardsRequest, IServiceRequest<SaveGameAwardsRequestDependency>, IAnswerOnComplete, IServiceRequest
	{
		private SaveGameAwardsRequestDependency _dependency;

		protected override byte OperationCode => 78;

		public SaveCampaignGameAwardsRequest()
			: base("strRobocloudError", "strSinglePlayerStartError", 0)
		{
		}

		public void Inject(SaveGameAwardsRequestDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			OperationRequest val2 = val;
			val2.Parameters[82] = _dependency.gameResult.Serialise();
			val2.Parameters[22] = _dependency.campaignId;
			val2.Parameters[23] = _dependency.campaignDifficulty;
			val2.Parameters[84] = _dependency.longPlayMultiplier;
			val2.Parameters[85] = CacheDTO.ReconnectGameGUID;
			val2.Parameters[136] = GameModeType.Campaign.ToString();
			val2.Parameters[78] = CacheDTO.MapName;
			val2.OperationCode = OperationCode;
			return val2;
		}
	}
}
