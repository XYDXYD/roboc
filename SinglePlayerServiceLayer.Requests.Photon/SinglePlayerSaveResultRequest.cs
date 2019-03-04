using ExitGames.Client.Photon;
using LobbyServiceLayer;
using SinglePlayerServiceLayer.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal class SinglePlayerSaveResultRequest : SinglePlayerRequest, ISinglePlayerSaveResultRequest, IServiceRequest<SaveGameAwardsRequestDependency>, IAnswerOnComplete, IServiceRequest
	{
		private SaveGameAwardsRequestDependency _dependency;

		protected override byte OperationCode => 2;

		public override bool isEncrypted => true;

		public SinglePlayerSaveResultRequest()
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
			GameModeKey value = CacheDTO.GameMode.Value;
			val2.Parameters[13] = value.type.ToString();
			val2.Parameters[15] = value.IsRanked;
			val2.Parameters[16] = value.IsBrawl;
			val2.Parameters[17] = value.IsCustomGame;
			val2.Parameters[14] = CacheDTO.MapName;
			val2.Parameters[9] = _dependency.gameResult.Serialise();
			val2.Parameters[12] = _dependency.longPlayMultiplier;
			val2.Parameters[18] = CacheDTO.ReconnectGameGUID;
			val2.OperationCode = OperationCode;
			return val2;
		}
	}
}
