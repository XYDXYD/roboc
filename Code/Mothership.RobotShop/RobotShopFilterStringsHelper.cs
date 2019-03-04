using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mothership.RobotShop
{
	internal class RobotShopFilterStringsHelper
	{
		public FasterList<string> GeneratePartCategoryFilter(List<ItemCategory> categories)
		{
			FasterList<string> val = new FasterList<string>();
			val.Add("strAnyCategories");
			for (int i = 0; i < categories.Count; i++)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("str");
				stringBuilder.Append(categories[i].ToString());
				stringBuilder.Append("Name");
				val.Add(stringBuilder.ToString());
			}
			return val;
		}

		public FasterList<string> GenerateSortByFilter()
		{
			FasterList<string> val = new FasterList<string>();
			ItemSortMode[] array = Enum.GetValues(typeof(ItemSortMode)) as ItemSortMode[];
			for (int i = 0; i < array.Length; i++)
			{
				val.Add(array[i].ToString());
			}
			return val;
		}

		public FasterList<string> GenerateSearchByFilter()
		{
			FasterList<string> val = new FasterList<string>();
			TextSearchField[] array = Enum.GetValues(typeof(TextSearchField)) as TextSearchField[];
			for (int i = 0; i < array.Length; i++)
			{
				val.Add(array[i].ToString());
			}
			return val;
		}

		public FasterList<string> GenerateRobotTierFilter(TiersData tiersData)
		{
			int tiersCount = RRAndTiers.GetTiersCount(tiersData);
			FasterList<string> val = new FasterList<string>();
			val.Add(StringTableBase<StringTable>.Instance.GetString("strCRFFilterAnyTiers"));
			string empty = string.Empty;
			for (int i = 0; i < tiersCount; i++)
			{
				empty = ((!RRAndTiers.IsMegabotTier((uint)i, tiersData)) ? StringTableBase<StringTable>.Instance.GetReplaceString("strCRFFilerTier", "{TIER_VALUE}", (i + 1).ToString()) : StringTableBase<StringTable>.Instance.GetString("strCRFFilterMegabotTier"));
				val.Add(empty);
			}
			return val;
		}
	}
}
