using ExitGames.Client.Photon;
using Svelto.DataStructures;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal static class _GetBrawlCubeList
	{
		internal static ReadOnlyDictionary<CubeTypeID, CubeListData> ProcessResponse(OperationResponse response, byte code)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, Hashtable> cubeListResponseDictionary = (Dictionary<string, Hashtable>)response.Parameters[code];
			Dictionary<CubeTypeID, CubeListData> dictionary = CubeListParser.ProcessResponse(cubeListResponseDictionary);
			return new ReadOnlyDictionary<CubeTypeID, CubeListData>(dictionary);
		}

		internal static OperationRequest BuildOpRequest(byte opCode)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = opCode;
			Dictionary<byte, object> dictionary = val.Parameters = new Dictionary<byte, object>();
			return val;
		}
	}
}
