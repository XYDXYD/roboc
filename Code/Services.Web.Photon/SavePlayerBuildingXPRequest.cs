using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SavePlayerBuildingXPRequest : WebServicesRequest<PlayerLevelAndXPData>, ISavePlayerBuildingXPRequest, IServiceRequest, IAnswerOnComplete<PlayerLevelAndXPData>
	{
		protected override byte OperationCode => 170;

		public SavePlayerBuildingXPRequest()
			: base("strRobocloudError", "strUnableToSavePlayerBuildingXP", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override PlayerLevelAndXPData ProcessResponse(OperationResponse response)
		{
			float playerProgress = Convert.ToSingle(response.Parameters[199]);
			int playerLevel = Convert.ToInt32(response.Parameters[81]);
			int playerGainedXP = Convert.ToInt32(response.Parameters[205]);
			return new PlayerLevelAndXPData(playerProgress, playerLevel, playerGainedXP);
		}
	}
}
