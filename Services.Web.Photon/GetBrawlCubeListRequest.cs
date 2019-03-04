using ExitGames.Client.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;

namespace Services.Web.Photon
{
	internal sealed class GetBrawlCubeListRequest : WebServicesCachedRequest<ReadOnlyDictionary<CubeTypeID, CubeListData>>, ILoadCubeListRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<CubeTypeID, CubeListData>>
	{
		protected override byte OperationCode => 136;

		public bool isDone
		{
			get;
			private set;
		}

		public GetBrawlCubeListRequest()
			: base("strRobocloudError", "strUnableLoadBrawlCubeList", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		protected override OperationRequest BuildOpRequest()
		{
			return _GetBrawlCubeList.BuildOpRequest(OperationCode);
		}

		protected override ReadOnlyDictionary<CubeTypeID, CubeListData> ProcessResponse(OperationResponse response)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			ReadOnlyDictionary<CubeTypeID, CubeListData> result = _GetBrawlCubeList.ProcessResponse(response, 157);
			isDone = true;
			return result;
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetBrawlCubeListRequest not impleted");
		}

		void ILoadCubeListRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
