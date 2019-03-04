using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCRFShopItemDataRequest : WebServicesRequest<LoadCRFShopItemDataRequestResponse>, ILoadCRFShopItemDataRequest, IServiceRequest<int>, IAnswerOnComplete<LoadCRFShopItemDataRequestResponse>, IServiceRequest
	{
		private int _itemIndex;

		private bool _clearTheCache;

		protected override byte OperationCode => 87;

		public LoadCRFShopItemDataRequest()
			: base("strRobotShopError", "strErrorGetShopItemData", 0)
		{
		}

		public void ClearTheCache()
		{
			_clearTheCache = true;
		}

		public void Inject(int itemIndex)
		{
			_itemIndex = itemIndex;
			_clearTheCache = false;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[94] = _itemIndex;
			val.OperationCode = OperationCode;
			return val;
		}

		public override void Execute()
		{
			if (_clearTheCache || CacheDTO.robotShopRobotData == null || !CacheDTO.robotShopRobotData.ContainsKey(_itemIndex) || CacheDTO.robotShopColourData == null || !CacheDTO.robotShopColourData.ContainsKey(_itemIndex))
			{
				base.Execute();
			}
			else
			{
				OnParseSuccess(base.answer);
			}
		}

		private void OnParseSuccess(IServiceAnswer<LoadCRFShopItemDataRequestResponse> answer)
		{
			if (answer != null && answer.succeed != null)
			{
				LoadCRFShopItemDataRequestResponse loadCRFShopItemDataRequestResponse = new LoadCRFShopItemDataRequestResponse();
				loadCRFShopItemDataRequestResponse.robotData = CacheDTO.robotShopRobotData.get_Item(_itemIndex);
				loadCRFShopItemDataRequestResponse.colorData = CacheDTO.robotShopColourData.get_Item(_itemIndex);
				answer.succeed(loadCRFShopItemDataRequestResponse);
			}
		}

		protected override LoadCRFShopItemDataRequestResponse ProcessResponse(OperationResponse response)
		{
			Hashtable val = response.Parameters[95] as Hashtable;
			int num = (int)val.get_Item((object)"itemIndex");
			byte[] array = (byte[])val.get_Item((object)"cubeData");
			byte[] array2 = (byte[])val.get_Item((object)"colourData");
			LoadCRFShopItemDataRequestResponse loadCRFShopItemDataRequestResponse = new LoadCRFShopItemDataRequestResponse();
			loadCRFShopItemDataRequestResponse.robotData = array;
			loadCRFShopItemDataRequestResponse.colorData = array2;
			CacheDTO.robotShopRobotData.Add(num, array);
			CacheDTO.robotShopColourData.Add(num, array2);
			return loadCRFShopItemDataRequestResponse;
		}
	}
}
