using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadTiersBandingRequest : WebServicesCachedRequest<TiersData>, ILoadTiersBandingRequest, IServiceRequest, IAnswerOnComplete<TiersData>
	{
		private const int DEFAULT_TIER_NUMBER = 5;

		private const string KEY = "tiersbands";

		private const string KEY_MAX_RR = "maximumRobotRankingARobotCanObtain";

		protected override byte OperationCode => 7;

		public LoadTiersBandingRequest()
			: base("strGenericError", "strLoadTiersBandingConfigError", 3)
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

		protected override TiersData ProcessResponse(OperationResponse response)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)response.Parameters[1];
			int[] array = (int[])dictionary["tiersbands"];
			uint[] array2 = new uint[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (uint)array[i];
			}
			uint maxRobotRankingARobotCanObtain = Convert.ToUInt32(dictionary["maximumRobotRankingARobotCanObtain"]);
			return new TiersData(array2, maxRobotRankingARobotCanObtain);
		}
	}
}
