using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class ResetTutorialRobotRequest : WebServicesRequest<uint>, IResetTutorialRobotRequest, IServiceRequest<ResetTutorialRobotDependancy>, IAnswerOnComplete<uint>, IServiceRequest
	{
		private ResetTutorialRobotDependancy _dependency;

		protected override byte OperationCode => 128;

		public ResetTutorialRobotRequest()
			: base("strGenericError", "strTutorialResetRobotRequestFailed", 0)
		{
		}

		public void Inject(ResetTutorialRobotDependancy dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[148] = Convert.ToInt32(_dependency.stage);
			val.OperationCode = OperationCode;
			return val;
		}

		protected override uint ProcessResponse(OperationResponse response)
		{
			bool flag = Convert.ToBoolean(WebServicesParameterCode.ResetTutorialRobotRequestResult);
			uint num = (uint)Convert.ToInt32(response.Parameters[48]);
			CacheDTO.garageSlots[num].machineModel = null;
			CacheDTO.garageSlots[num].colorMap = null;
			CacheDTO.garageSlots[num].weaponOrder = null;
			CacheDTO.garageSlots[num].movementCategories = null;
			return num;
		}
	}
}
