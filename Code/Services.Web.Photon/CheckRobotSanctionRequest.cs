using Authentication;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal class CheckRobotSanctionRequest : WebServicesRequest<RobotSanctionData>, ICheckRobotSanctionRequest, IServiceRequest<string>, IAnswerOnComplete<RobotSanctionData>, IServiceRequest
	{
		private string _robotUniqueId;

		protected override byte OperationCode => 174;

		public CheckRobotSanctionRequest()
			: base("strRobocloudError", "strCheckRobotSanctionRequestError", 0)
		{
		}

		public void Inject(string robotUniqueId)
		{
			_robotUniqueId = robotUniqueId;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[54] = _robotUniqueId;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override RobotSanctionData ProcessResponse(OperationResponse response)
		{
			try
			{
				string[] array = (string[])response.Parameters[102];
				if (array != null && array.Length > 0 && !string.IsNullOrEmpty(array[0]))
				{
					return RobotSanctionData.Deserialise(array[0]);
				}
				return null;
			}
			catch (Exception arg)
			{
				RemoteLogger.Error("Error checking robot sanction", "user = " + User.Username, string.Empty);
				Console.LogError("CheckRobotSanctionRequest: request failed, could not parse resulting data. Cannot proceed. " + arg);
				throw new Exception("Failed to check robot sanction");
			}
		}
	}
}
