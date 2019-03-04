using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon.Tencent
{
	internal class QueryOrderRequest_Tencent : WebServicesRequest<bool>, IQueryOrderRequest_Tencent, IServiceRequest<QueryOrderDependency>, IAnswerOnComplete<bool>, IServiceRequest
	{
		private QueryOrderDependency _dependency;

		protected override byte OperationCode => 205;

		public QueryOrderRequest_Tencent()
			: base("strRobocloudError", "strTencentQueryOrderError", 0)
		{
		}

		public void Inject(QueryOrderDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[217] = _dependency.railID;
			val.Parameters[222] = _dependency.orderID;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			return Convert.ToBoolean(response.Parameters[223]);
		}
	}
}
