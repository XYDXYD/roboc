using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetCubesInventoryRequest : WebServicesRequest<Dictionary<uint, uint>>, ILoadUserCubeInventoryRequest, IServiceRequest, IAnswerOnComplete<Dictionary<uint, uint>>
	{
		protected override byte OperationCode => 16;

		public GetCubesInventoryRequest()
			: base("strRobocloudError", "strUnableLoadCubeInventory", 3)
		{
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

		protected override Dictionary<uint, uint> ProcessResponse(OperationResponse response)
		{
			Dictionary<uint, uint> dictionary = new Dictionary<uint, uint>();
			Dictionary<int, int> dictionary2 = (Dictionary<int, int>)response.Parameters[16];
			foreach (KeyValuePair<int, int> item in dictionary2)
			{
				dictionary.Add((uint)item.Key, (uint)item.Value);
			}
			CacheDTO.inventory = new Dictionary<uint, uint>(dictionary);
			return CacheDTO.inventory;
		}
	}
}
