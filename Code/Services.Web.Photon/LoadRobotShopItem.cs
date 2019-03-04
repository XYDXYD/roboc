using ExitGames.Client.Photon;
using Mothership;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;

namespace Services.Web.Photon
{
	internal class LoadRobotShopItem : WebServicesRequest<CRFItem>, ILoadRobotShopItem, IServiceRequest<int>, IAnswerOnComplete<CRFItem>, IServiceRequest
	{
		private int _itemId;

		protected override byte OperationCode => 173;

		public LoadRobotShopItem()
			: base("strRobocloudError", "strLoadRobotShopItemError", 0)
		{
		}

		public void Inject(int itemId)
		{
			_itemId = itemId;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[94] = _itemId;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override CRFItem ProcessResponse(OperationResponse response)
		{
			byte[] buffer = (byte[])response.Parameters[90];
			RobotShopItem robotShopItem = null;
			using (MemoryStream input = new MemoryStream(buffer))
			{
				using (BinaryReader br = new BinaryReader(input))
				{
					robotShopItem = new RobotShopItem(br);
				}
			}
			return new CRFItem(robotShopItem);
		}
	}
}
