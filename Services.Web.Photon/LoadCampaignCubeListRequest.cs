using ExitGames.Client.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCampaignCubeListRequest : WebServicesCachedRequest<ReadOnlyDictionary<CubeTypeID, CubeListData>>, ILoadCubeListRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<CubeTypeID, CubeListData>>
	{
		private ParameterOverride _parameterOverride;

		protected override byte OperationCode => 2;

		public LoadCampaignCubeListRequest()
			: base("strRobocloudError", "strUnableLoadCampaignCubeList", 3)
		{
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			_parameterOverride = parameterOverride;
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
					(byte)_parameterOverride.ParameterCode,
					_parameterOverride.ParameterValue
				}
			};
			return val;
		}

		protected override ReadOnlyDictionary<CubeTypeID, CubeListData> ProcessResponse(OperationResponse response)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return _GetBrawlCubeList.ProcessResponse(response, 1);
		}

		void ILoadCubeListRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
