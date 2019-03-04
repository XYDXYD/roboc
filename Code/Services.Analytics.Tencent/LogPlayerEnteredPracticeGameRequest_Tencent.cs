using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerEnteredPracticeGameRequest_Tencent : WebServicesRequest, ILogPlayerEnteredPracticeGameRequest, IServiceRequest, IAnswerOnComplete
	{
		protected override byte OperationCode => 209;

		public LogPlayerEnteredPracticeGameRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerEnterPracticeGameError", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["GameModeType"] = GameModeType.PraticeMode;
			dictionary["IsRanked"] = false;
			dictionary["IsBrawl"] = false;
			dictionary["IsCustomGame"] = false;
			dictionary["ReconnectGameGUID"] = string.Empty;
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[225] = dictionary;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
