using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetAutoRegenHealthSettings : WebServicesRequest<AutoRegenHealthSettingsData>, IGetAutoRegenHealthSettings, IServiceRequest, IAnswerOnComplete<AutoRegenHealthSettingsData>
	{
		protected override byte OperationCode => 35;

		public GetAutoRegenHealthSettings()
			: base("strRobocloudError", "strUnableLoadAutoRegenHealthSettings", 3)
		{
		}

		public override void Execute()
		{
			if (IsDataCached())
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(GetCachedData());
				}
			}
			else
			{
				base.Execute();
			}
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
					false
				}
			};
			val.OperationCode = OperationCode;
			return val;
		}

		protected override AutoRegenHealthSettingsData ProcessResponse(OperationResponse response)
		{
			return CacheDTO.autoRegenHealthSettings = new AutoRegenHealthSettingsData((byte[])response.Parameters[37]);
		}

		private bool IsDataCached()
		{
			return CacheDTO.autoRegenHealthSettings != null;
		}

		private AutoRegenHealthSettingsData GetCachedData()
		{
			return new AutoRegenHealthSettingsData(CacheDTO.autoRegenHealthSettings);
		}

		public void ClearCache()
		{
			CacheDTO.autoRegenHealthSettings = null;
		}

		public void SetParameterOverrides(ParameterOverride[] parameterOverrides)
		{
			throw new Exception("SetParameterOverride for GetAutoRegenHealthSettings not impleted");
		}
	}
}
