using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal class GetQuitterPenaltyInfo : LobbyRequest<QuitterInfo>, IGetQuitterPenaltyInfo, IServiceRequest, IAnswerOnComplete<QuitterInfo>
	{
		protected override byte OperationCode => 3;

		public GetQuitterPenaltyInfo()
			: base("strLobbyError", "strLobbyJoinFail", 3)
		{
		}

		protected override QuitterInfo ProcessResponse(OperationResponse response)
		{
			bool quitLastGame = (bool)response.Parameters[19];
			int quitterBlockTime = (int)response.Parameters[15];
			return new QuitterInfo(quitLastGame, quitterBlockTime);
		}
	}
}
