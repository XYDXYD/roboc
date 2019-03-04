using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadCosmeticsRenderLimitsRequest : WebServicesCachedRequest<CosmeticsRenderLimitsDependency>, ILoadCosmeticsRenderLimitsRequest, ITask, IServiceRequest, IAnswerOnComplete<CosmeticsRenderLimitsDependency>, IAbstractTask
	{
		protected override byte OperationCode => 72;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadCosmeticsRenderLimitsRequest()
			: base("strRobocloudError", "LOAD COSMETIC RENDER LIMIT ERROR", 3)
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

		protected override CosmeticsRenderLimitsDependency ProcessResponse(OperationResponse response)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			CosmeticsRenderLimitsDependency result = new CosmeticsRenderLimitsDependency(response.Parameters[196]);
			isDone = true;
			return result;
		}
	}
}
