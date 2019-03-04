using Authentication;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal class CheckAllRobotsSanctionsRequest : WebServicesRequest<RobotSanctionData[]>, ICheckAllRobotsSanctionsRequest, IServiceRequest, IAnswerOnComplete<RobotSanctionData[]>
	{
		protected override byte OperationCode => 176;

		public CheckAllRobotsSanctionsRequest()
			: base("strRobocloudError", "strCheckAllRobotsSanctionsRequestError", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override RobotSanctionData[] ProcessResponse(OperationResponse response)
		{
			try
			{
				string[] array = (string[])response.Parameters[102];
				if (array != null && array.Length > 0)
				{
					RobotSanctionData[] array2 = new RobotSanctionData[array.Length];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = RobotSanctionData.Deserialise(array[i]);
					}
					return array2;
				}
				return null;
			}
			catch (Exception arg)
			{
				RemoteLogger.Error("Error checking robot sanction", "user = " + User.Username, string.Empty);
				Console.LogError("CheckAllRobotsSanctionsRequest: request failed, could not parse resulting data. Cannot proceed. " + arg);
				throw new Exception("Failed to check robot sanction");
			}
		}
	}
}
