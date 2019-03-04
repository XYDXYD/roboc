using System.Collections;
using System.Collections.Generic;

internal class RoboPassSeasonRewardData
{
	private const string STR_KEY_CURRENCY_AMOUNT = "{Amount}";

	public bool isDeluxe
	{
		get;
		private set;
	}

	public string spriteName
	{
		get;
		private set;
	}

	public bool spriteFullSize
	{
		get;
		private set;
	}

	public string rewardName
	{
		get;
		private set;
	}

	public string rewardNameKey
	{
		get;
		private set;
	}

	public string categoryName
	{
		get;
		private set;
	}

	public ItemData[] items
	{
		get;
		private set;
	}

	public RoboPassSeasonRewardData(IDictionary data)
	{
		isDeluxe = Decode.Get<bool>(data, "isDeluxe");
		spriteName = Decode.Get<string>(data, "spriteName");
		spriteFullSize = Decode.Get<bool>(data, "spriteFullSize");
		object[] array = Decode.Get<object[]>(data, "items");
		items = new ItemData[array.Length];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new ItemData((Dictionary<string, object>)array[i]);
		}
		categoryName = StringTableBase<StringTable>.Instance.GetString(GetCategoryNameKey(items));
		rewardNameKey = Decode.Get<string>(data, "rewardStrKey");
		rewardName = GetRewardName(rewardNameKey);
	}

	private string GetRewardName(string rewardNameKey)
	{
		string text = StringTableBase<StringTable>.Instance.GetString(rewardNameKey);
		ItemData itemData = items[0];
		if (items.Length == 1 && (itemData.category == RoboPassSeasonRewardCategory.CosmeticCredits || itemData.category == RoboPassSeasonRewardCategory.Robits))
		{
			text = text.Replace("{Amount}", itemData.count.ToString());
		}
		return text;
	}

	private string GetCategoryNameKey(ItemData[] items)
	{
		string result = string.Empty;
		if (items.Length > 1)
		{
			result = "strBundleName";
		}
		else
		{
			switch (items[0].category)
			{
			case RoboPassSeasonRewardCategory.Cube:
				result = "strCubeName";
				break;
			case RoboPassSeasonRewardCategory.GarageBaySkin:
				result = "strGarageBaySkinName";
				break;
			case RoboPassSeasonRewardCategory.Robits:
				result = "strRobits";
				break;
			case RoboPassSeasonRewardCategory.CosmeticCredits:
				result = "strCosmeticCredits";
				break;
			case RoboPassSeasonRewardCategory.SpawnEffect:
				result = "strSpawnEffect";
				break;
			case RoboPassSeasonRewardCategory.DeathEffect:
				result = "strDeathEffect";
				break;
			}
		}
		return result;
	}
}
