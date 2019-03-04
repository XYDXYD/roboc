using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class LoadItemShopBundleListRequest : WebServicesCachedRequest<ItemShopResponseData>, ILoadItemShopBundleListRequest, IServiceRequest, IAnswerOnComplete<ItemShopResponseData>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 188;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadItemShopBundleListRequest()
			: base("strGenericError", "strLoadItemShopBundleListError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override ItemShopResponseData ProcessResponse(OperationResponse response)
		{
			isDone = true;
			try
			{
				byte[] data = (byte[])response.Parameters[65];
				List<ItemShopBundle> list = Deserialise(data);
				Console.Log("----------------------------------received itemsshopbundles: " + list.Count);
				List<ItemShopBundle> list2 = new List<ItemShopBundle>(list.Distinct());
				list2.Sort();
				int num = 17;
				int num2 = 17;
				for (int i = 0; i < list2.Count; i++)
				{
					ItemShopBundle itemShopBundle = list2[i];
					switch (itemShopBundle.Recurrence)
					{
					case ItemShopRecurrence.Daily:
						num = num * 23 + itemShopBundle.GetHashCode();
						break;
					case ItemShopRecurrence.Weekly:
						num2 = num2 * 23 + itemShopBundle.GetHashCode();
						break;
					default:
						throw new Exception("ItemShopRecurrence not recogised");
					}
				}
				return new ItemShopResponseData(list2, num, num2);
			}
			catch (Exception arg)
			{
				throw new Exception("Failed to load Item Shop product list " + arg);
			}
		}

		private List<ItemShopBundle> Deserialise(byte[] data)
		{
			List<ItemShopBundle> list = new List<ItemShopBundle>();
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						string sku = binaryReader.ReadString();
						string productNameStrKey = binaryReader.ReadString();
						string spriteName = binaryReader.ReadString();
						bool isSpriteFullSize = binaryReader.ReadBoolean();
						string value = binaryReader.ReadString();
						int price = binaryReader.ReadInt32();
						DateTime t = new DateTime(1970, 1, 1).AddSeconds(binaryReader.ReadInt64());
						int discountPrice = binaryReader.ReadInt32();
						int recurrence = binaryReader.ReadInt32();
						bool ownsRequiredCube = binaryReader.ReadBoolean();
						int category = binaryReader.ReadInt32();
						bool limitedEdition = binaryReader.ReadBoolean();
						bool discounted = t > DateTime.UtcNow;
						CurrencyType currencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), value);
						ItemShopBundle item = new ItemShopBundle(sku, productNameStrKey, spriteName, isSpriteFullSize, (ItemShopCategory)category, currencyType, price, discountPrice, (ItemShopRecurrence)recurrence, ownsRequiredCube, discounted, limitedEdition);
						list.Add(item);
					}
					return list;
				}
			}
		}

		void ILoadItemShopBundleListRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
