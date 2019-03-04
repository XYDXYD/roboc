using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class BuyItemShopBundleRequest : WebServicesRequest<string[]>, IBuyItemShopBundleRequest, IServiceRequest<ItemShopBundle>, IAnswerOnComplete<string[]>, IServiceRequest
	{
		private ItemShopBundle _selectedBundle;

		protected override byte OperationCode => 189;

		public BuyItemShopBundleRequest()
			: base("strRobocloudError", "strBuyItemShopBundleRequestError", 0)
		{
		}

		public void Inject(ItemShopBundle bundle)
		{
			_selectedBundle = bundle;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[32] = _selectedBundle.SKU;
			val.Parameters[53] = _selectedBundle.CurrencyType.ToString();
			val.Parameters[65] = ((!_selectedBundle.Discounted) ? _selectedBundle.Price : _selectedBundle.DiscountPrice);
			val.OperationCode = OperationCode;
			return val;
		}

		protected override string[] ProcessResponse(OperationResponse response)
		{
			return (string[])response.Parameters[72];
		}
	}
}
