using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCampaignPowerBarSettingsRequest : WebServicesCachedRequest<PowerBarSettingsData>, IGetPowerBarSettingsRequest, IServiceRequest, IAnswerOnComplete<PowerBarSettingsData>
	{
		private ParameterOverride _parameterOverride;

		protected override byte OperationCode => 51;

		public LoadCampaignPowerBarSettingsRequest()
			: base("strRobocloudError", "strUnableLoadCampaignPowerBarSettings", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>
			{
				{
					(byte)_parameterOverride.ParameterCode,
					_parameterOverride.ParameterValue
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
			_parameterOverride = parameterOverride;
		}

		void IGetPowerBarSettingsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
