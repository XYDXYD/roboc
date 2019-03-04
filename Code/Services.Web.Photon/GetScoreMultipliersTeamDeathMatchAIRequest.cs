using ExitGames.Client.Photon;
using GameServerServiceLayer;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetScoreMultipliersTeamDeathMatchAIRequest : WebServicesRequest<ScoreMultipliers>, IGetScoreMultipliersTeamDeathMatchAIRequest, IServiceRequest, IAnswerOnComplete<ScoreMultipliers>
	{
		protected override byte OperationCode => 117;

		public GetScoreMultipliersTeamDeathMatchAIRequest()
			: base("strRobocloudError", "strUnableLoadTeamDeathMatchAIScoreData", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[136] = GameModeType.PraticeMode;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override ScoreMultipliers ProcessResponse(OperationResponse response)
		{
			return (CacheDTO.scoreMultipliersData = new ScoreMultipliersData((byte[])response.get_Item((byte)137))).scoreMultipliers.Copy();
		}

		public override void Execute()
		{
			if (IsDataCached())
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(GetCachedData());
				}
			}
			else
			{
				base.Execute();
			}
		}

		private bool IsDataCached()
		{
			return CacheDTO.scoreMultipliersData != null;
		}

		private ScoreMultipliers GetCachedData()
		{
			return CacheDTO.scoreMultipliersData.scoreMultipliers.Copy();
		}
	}
}
