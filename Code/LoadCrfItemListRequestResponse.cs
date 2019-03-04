using Mothership;
using System.Collections.Generic;
using System.IO;

internal class LoadCrfItemListRequestResponse
{
	public List<CRFItem> robotShopItemList;

	public LoadCrfItemListRequestResponse(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = binaryReader.ReadInt32();
				robotShopItemList = new List<CRFItem>(num);
				for (int i = 0; i < num; i++)
				{
					RobotShopItem robotShopItem = new RobotShopItem(binaryReader);
					robotShopItemList.Add(new CRFItem(robotShopItem));
				}
			}
		}
	}
}
