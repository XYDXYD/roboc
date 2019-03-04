using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetBrawlAutoRegenHealthSettingsRequest : WebServicesCachedRequest<AutoRegenHealthSettingsData>, IGetAutoRegenHealthSettings, IServiceRequest, IAnswerOnComplete<AutoRegenHealthSettingsData>
	{
		protected override byte OperationCode => 139;

		public bool isDone
		{
			get;
			private set;
		}

		public GetBrawlAutoRegenHealthSettingsRequest()
			: base("strRobocloudError", "strUnableLoadBrawlAutoRegenHealthSettings", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
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

		protected override AutoRegenHealthSettingsData ProcessResponse(OperationResponse response)
		{
			AutoRegenHealthSettingsData result = new AutoRegenHealthSettingsData((byte[])response.Parameters[160]);
			isDone = true;
			return result;
		}

		public void SetParameterOverrides(ParameterOverride[] parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetBrawlAutoRegenHealthSettingsRequest not impleted");
		}

		void IGetAutoRegenHealthSettings.ClearCache()
		{
			ClearCache();
		}
	}
}
