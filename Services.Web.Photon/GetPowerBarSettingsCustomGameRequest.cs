using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetPowerBarSettingsCustomGameRequest : WebServicesCachedRequest<PowerBarSettingsData>, IGetPowerBarSettingsRequest, IServiceRequest, IAnswerOnComplete<PowerBarSettingsData>
	{
		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 51;

		public GetPowerBarSettingsCustomGameRequest()
			: base("strRobocloudError", "strUnableLoadPowerBarSettings", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>
			{
				{
					191,
					true
				}
			};
			return val;
		}

		protected override PowerBarSettingsData ProcessResponse(OperationResponse response)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			Hashtable value = response.Parameters[61];
			return new PowerBarSettingsData(value);
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetPowerBarSettingsCustomGameRequest not impleted");
		}

		void IGetPowerBarSettingsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
