using Authentication;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class LoadPlayerRoboPassSeasonRequest : WebServicesCachedRequest<PlayerRoboPassSeasonData>, ILoadPlayerRoboPassSeasonRequest, IServiceRequest, IAnswerOnComplete<PlayerRoboPassSeasonData>
	{
		protected override byte OperationCode => 178;

		public LoadPlayerRoboPassSeasonRequest()
			: base("strGenericError", "strLoadPlayerRoboPassSeasonRequestError", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override PlayerRoboPassSeasonData ProcessResponse(OperationResponse response)
		{
			try
			{
				object obj = response.Parameters[237];
				if (obj == null)
				{
					return null;
				}
				Dictionary<string, object> data = (Dictionary<string, object>)obj;
				return new PlayerRoboPassSeasonData(data);
			}
			catch (Exception ex)
			{
				RemoteLogger.Error("Error loading player RoboPass season", "user = " + User.Username, string.Empty);
				Console.LogError("LoadPlayerRoboPassSeasonRequest: request failed, could not parse resulting data. Cannot proceed. " + ex.Message + " " + ex.StackTrace);
				throw new Exception("Failed to load player RoboPass season ");
			}
		}

		void ILoadPlayerRoboPassSeasonRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
