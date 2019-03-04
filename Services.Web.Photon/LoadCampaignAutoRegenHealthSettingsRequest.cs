using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCampaignAutoRegenHealthSettingsRequest : WebServicesCachedRequest<AutoRegenHealthSettingsData>, IGetAutoRegenHealthSettings, IServiceRequest, IAnswerOnComplete<AutoRegenHealthSettingsData>
	{
		private ParameterOverride[] _parameterOverrides;

		protected override byte OperationCode => 35;

		public LoadCampaignAutoRegenHealthSettingsRequest()
			: base("strRobocloudError", "strUnableLoadCampaignAutoRegenHealthSettings", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public void SetParameterOverrides(ParameterOverride[] parameterOverrides)
		{
			_parameterOverrides = parameterOverrides;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			for (int i = 0; i < _parameterOverrides.Length; i++)
			{
				ParameterOverride parameterOverride = _parameterOverrides[i];
				val.Parameters[(byte)parameterOverride.ParameterCode] = parameterOverride.ParameterValue;
			}
			return val;
		}

		protected override AutoRegenHealthSettingsData ProcessResponse(OperationResponse response)
		{
			return new AutoRegenHealthSettingsData((byte[])response.Parameters[37]);
		}

		void IGetAutoRegenHealthSettings.ClearCache()
		{
			ClearCache();
		}
	}
}
