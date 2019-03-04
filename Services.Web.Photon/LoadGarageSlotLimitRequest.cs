using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadGarageSlotLimitRequest : WebServicesRequest<uint>, ILoadGarageSlotLimitRequest, IServiceRequest, IAnswerOnComplete<uint>
	{
		protected override byte OperationCode => 60;

		public LoadGarageSlotLimitRequest()
			: base("strRobocloudError", "strLoadGarageLimitError", 0)
		{
		}

		public override void Execute()
		{
			uint? garageSlotLimit = CacheDTO.garageSlotLimit;
			if (!garageSlotLimit.HasValue)
			{
				base.Execute();
			}
			else
			{
				base.answer.succeed(CacheDTO.garageSlotLimit.Value);
			}
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

		protected override uint ProcessResponse(OperationResponse response)
		{
			uint num = Convert.ToUInt32(response.Parameters[68]);
			CacheDTO.garageSlotLimit = num;
			return num;
		}
	}
}
