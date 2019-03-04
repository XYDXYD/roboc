using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetCubeListRequest : WebServicesRequest<ReadOnlyDictionary<CubeTypeID, CubeListData>>, ILoadCubeListRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<CubeTypeID, CubeListData>>
	{
		protected override byte OperationCode => 2;

		public GetCubeListRequest()
			: base("strRobocloudError", "strUnableLoadCubeList", 3)
		{
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetCubeList not impleted");
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

		public override void Execute()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (CacheDTO.cubeList != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(new ReadOnlyDictionary<CubeTypeID, CubeListData>(CacheDTO.cubeList));
				}
			}
			else
			{
				base.Execute();
			}
		}

		public void ClearCache()
		{
			CacheDTO.cubeList = null;
		}

		protected override ReadOnlyDictionary<CubeTypeID, CubeListData> ProcessResponse(OperationResponse response)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, Hashtable> cubeListResponseDictionary = (Dictionary<string, Hashtable>)response.Parameters[1];
			CacheDTO.cubeList = CubeListParser.ProcessResponse(cubeListResponseDictionary);
			return new ReadOnlyDictionary<CubeTypeID, CubeListData>(CacheDTO.cubeList);
		}
	}
}
