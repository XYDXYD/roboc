using ExitGames.Client.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCubeListCustomGameRequest : WebServicesCachedRequest<ReadOnlyDictionary<CubeTypeID, CubeListData>>, ILoadCubeListRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<CubeTypeID, CubeListData>>
	{
		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 2;

		public LoadCubeListCustomGameRequest()
			: base("strRobocloudError", "strUnableLoadCubeList", 3)
		{
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for LoadCubeListCustomGameRequest not impleted");
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>
			{
				{
					191,
					true
				}
			};
			return val;
		}

		protected override ReadOnlyDictionary<CubeTypeID, CubeListData> ProcessResponse(OperationResponse response)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			ReadOnlyDictionary<CubeTypeID, CubeListData> result = _GetBrawlCubeList.ProcessResponse(response, 1);
			isDone = true;
			return result;
		}

		void ILoadCubeListRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
