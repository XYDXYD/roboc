using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetPowerBarSettingsRequest : WebServicesRequest<PowerBarSettingsData>, IGetPowerBarSettingsRequest, IServiceRequest, IAnswerOnComplete<PowerBarSettingsData>
	{
		protected override byte OperationCode => 51;

		public GetPowerBarSettingsRequest()
			: base("strRobocloudError", "strUnableLoadPowerBarSettings", 3)
		{
		}

		public override void Execute()
		{
			if (CacheDTO.powerBarSettingsData == null)
			{
				base.Execute();
			}
			else if (base.answer != null && base.answer.succeed != null)
			{
				base.answer.succeed(CacheDTO.powerBarSettingsData);
			}
		}

		protected override PowerBarSettingsData ProcessResponse(OperationResponse response)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			Hashtable value = response.Parameters[61];
			return CacheDTO.powerBarSettingsData = new PowerBarSettingsData(value);
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

		public void ClearCache()
		{
			CacheDTO.powerBarSettingsData = null;
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetPowerBarSettingsRequest not impleted");
		}
	}
}
