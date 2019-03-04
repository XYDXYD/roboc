using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class LoadPurchasesRequest : WebServicesRequest<FasterList<PurchaseRequestData>>, ILoadPurchasesRequest, IServiceRequest, IAnswerOnComplete<FasterList<PurchaseRequestData>>
	{
		protected override byte OperationCode => 81;

		public LoadPurchasesRequest()
			: base("strRobocloudError", "strLoadPurchasesError", 3)
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

		protected override FasterList<PurchaseRequestData> ProcessResponse(OperationResponse response)
		{
			FasterList<PurchaseRequestData> val = new FasterList<PurchaseRequestData>();
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[88];
			foreach (KeyValuePair<string, Hashtable> item in dictionary)
			{
				ShopItemType shopItemType = (ShopItemType)Enum.Parse(typeof(ShopItemType), item.Key);
				PurchaseRequestData purchaseRequestData;
				switch (shopItemType)
				{
				case ShopItemType.Premium:
				case ShopItemType.PremiumForLife:
					purchaseRequestData = ParsePremium(item.Value, shopItemType);
					break;
				case ShopItemType.Cube:
					purchaseRequestData = ParseCube(item.Value);
					break;
				case ShopItemType.CosmeticCredits:
					purchaseRequestData = ParseCosmeticCredits(item.Value);
					break;
				case ShopItemType.RoboPass:
					purchaseRequestData = ParseRoboPass(item.Value);
					break;
				default:
					throw new Exception("Unknown item type.");
				}
				val.Add(purchaseRequestData);
			}
			return val;
		}

		private static PurchaseRequestData ParsePremium(Hashtable premiumInfoResponse, ShopItemType shopItemType)
		{
			Console.Log("LoadPurchasesRequest got premium");
			PurchaseRequestData purchaseRequestData = new PurchaseRequestData(shopItemType);
			int days = Convert.ToInt32(premiumInfoResponse.get_Item((object)(byte)8));
			int hours = Convert.ToInt32(premiumInfoResponse.get_Item((object)(byte)13));
			int minutes = Convert.ToInt32(premiumInfoResponse.get_Item((object)(byte)14));
			int seconds = Convert.ToInt32(premiumInfoResponse.get_Item((object)(byte)15));
			bool hasPremiumForLife = Convert.ToBoolean(premiumInfoResponse.get_Item((object)(byte)150));
			int numPremiumDaysAwarded_ = Convert.ToInt32(premiumInfoResponse.get_Item((object)(byte)151));
			CacheDTO.premiumData.hasPremiumForLife = hasPremiumForLife;
			TimeSpan timeSpan = new TimeSpan(days, hours, minutes, seconds);
			CacheDTO.premiumData.premiumTimeLeft = timeSpan;
			if (timeSpan.TotalSeconds > 0.0)
			{
				CacheDTO.premiumData.premiumLoadTime = DateTime.UtcNow;
			}
			else
			{
				CacheDTO.premiumData.premiumLoadTime = DateTime.MinValue;
			}
			purchaseRequestData.premiumPurchaseResponse = new PremiumPurchaseResponse(timeSpan, hasPremiumForLife, numPremiumDaysAwarded_);
			return purchaseRequestData;
		}

		private static PurchaseRequestData ParseCube(Hashtable cubeInfoResponse)
		{
			Console.Log("LoadPurchasesRequest got cube");
			PurchaseRequestData purchaseRequestData = new PurchaseRequestData(ShopItemType.Cube);
			if (CacheDTO.inventory == null)
			{
				CacheDTO.inventory = new Dictionary<uint, uint>();
			}
			Dictionary<uint, uint> dictionary = new Dictionary<uint, uint>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			foreach (DictionaryEntry item in cubeInfoResponse)
			{
				uint key = Convert.ToUInt32((string)item.Key, 16);
				uint num = Convert.ToUInt32(item.Value);
				if (CacheDTO.inventory.ContainsKey(key))
				{
					dictionary2[(string)item.Key] = num - CacheDTO.inventory[key];
				}
				else
				{
					dictionary2[(string)item.Key] = num;
				}
				CacheDTO.inventory[key] = num;
				dictionary[key] = num;
			}
			purchaseRequestData.newCubeTotals = dictionary;
			purchaseRequestData.cubesAwarded = dictionary2;
			return purchaseRequestData;
		}

		private static PurchaseRequestData ParseCosmeticCredits(Hashtable cosmeticCreditsResp)
		{
			Console.Log("LoadPurchasesRequest got cosmetic credits");
			PurchaseRequestData purchaseRequestData = new PurchaseRequestData(ShopItemType.CosmeticCredits);
			foreach (DictionaryEntry item in cosmeticCreditsResp)
			{
				int[] array = (int[])item.Value;
				for (int i = 0; i < array.Length; i++)
				{
					purchaseRequestData.TotalPurchasedCC += array[i];
					purchaseRequestData.PurchasedCCList.Add(array[i]);
				}
			}
			return purchaseRequestData;
		}

		private static PurchaseRequestData ParseRoboPass(Hashtable robopassResponse)
		{
			return new PurchaseRequestData(ShopItemType.RoboPass);
		}
	}
}
