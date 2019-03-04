using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadCpuSettingsRequest : WebServicesCachedRequest<CPUSettingsDependency>, ILoadCpuSettingsRequest, ITask, IServiceRequest, IAnswerOnComplete<CPUSettingsDependency>, IAbstractTask
	{
		protected override byte OperationCode => 75;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadCpuSettingsRequest()
			: base("strRobocloudError", "strLoadCpuSettingsError", 3)
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

		protected override CPUSettingsDependency ProcessResponse(OperationResponse response)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Hashtable val = response.Parameters[194];
			uint maxRegularCPU_ = (uint)Convert.ToInt32(val.get_Item((object)"MaxRegularCPU"));
			uint maxMegabotCPU_ = (uint)Convert.ToInt32(val.get_Item((object)"MaxMegabotCPU"));
			uint premiumForLifeCosmeticCPU_ = (uint)Convert.ToInt32(val.get_Item((object)"PremiumForLifeCosmeticCPU"));
			uint premiumCosmeticCPU_ = (uint)Convert.ToInt32(val.get_Item((object)"PremiumCosmeticCPU"));
			uint noPremiumCosmeticCPU_ = (uint)Convert.ToInt32(val.get_Item((object)"NoPremiumCosmeticCPU"));
			uint maxRegularHealth_ = (uint)Convert.ToInt32(val.get_Item((object)"MaxRegularHealth"));
			uint maxMegabotHealth_ = (uint)Convert.ToInt32(val.get_Item((object)"MaxMegabotHealth"));
			isDone = true;
			return new CPUSettingsDependency(maxRegularCPU_, maxMegabotCPU_, premiumForLifeCosmeticCPU_, premiumCosmeticCPU_, noPremiumCosmeticCPU_, maxRegularHealth_, maxMegabotHealth_);
		}
	}
}
