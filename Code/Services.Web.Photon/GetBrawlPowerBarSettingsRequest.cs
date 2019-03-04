using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetBrawlPowerBarSettingsRequest : WebServicesCachedRequest<PowerBarSettingsData>, IGetPowerBarSettingsRequest, IServiceRequest, IAnswerOnComplete<PowerBarSettingsData>
	{
		protected override byte OperationCode => 138;

		public bool isDone
		{
			get;
			private set;
		}

		public GetBrawlPowerBarSettingsRequest()
			: base("strRobocloudError", "strUnableLoadBrawlPowerBarSettings", 3)
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

		protected override PowerBarSettingsData ProcessResponse(OperationResponse response)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable value = response.Parameters[159];
			PowerBarSettingsData result = new PowerBarSettingsData(value);
			isDone = true;
			return result;
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetPowerBarSettingsRequest not impleted");
		}

		void IGetPowerBarSettingsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
