using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal static class CubeListParser
	{
		private const uint PLACEMENT_FACES_ALL = 63u;

		public const string CPU_RATING = "cpuRating";

		public const string HEALTH = "health";

		public const string ROBOT_RANKING = "robotRanking";

		public const string IS_COSMETIC = "isCosmetic";

		public static Dictionary<CubeTypeID, CubeListData> ProcessResponse(Dictionary<string, Hashtable> cubeListResponseDictionary)
		{
			Dictionary<CubeTypeID, CubeListData> dictionary = new Dictionary<CubeTypeID, CubeListData>();
			foreach (KeyValuePair<string, Hashtable> item in cubeListResponseDictionary)
			{
				CubeTypeID key = ParseCubeID(item.Key);
				Hashtable value = item.Value;
				CubeListData value2 = ParseNode(value);
				dictionary.Add(key, value2);
			}
			return dictionary;
		}

		public static CubeTypeID ParseCubeID(string input)
		{
			return Convert.ToUInt32(input, 16);
		}

		private static CubeListData ParseNode(Hashtable value)
		{
			uint cpu = 0u;
			if (((Dictionary<object, object>)value).ContainsKey((object)"cpuRating"))
			{
				cpu = Convert.ToUInt32(value.get_Item((object)"cpuRating"));
			}
			int h = 0;
			if (((Dictionary<object, object>)value).ContainsKey((object)"health"))
			{
				h = Convert.ToInt32(value.get_Item((object)"health"));
			}
			float healthBoost_ = 0f;
			if (((Dictionary<object, object>)value).ContainsKey((object)"healthBoost"))
			{
				healthBoost_ = (float)Convert.ToDouble(value.get_Item((object)"healthBoost"));
			}
			bool greyOutInTutorial_ = false;
			if (((Dictionary<object, object>)value).ContainsKey((object)"GreyOutInTutorial"))
			{
				greyOutInTutorial_ = Convert.ToBoolean(value.get_Item((object)"GreyOutInTutorial"));
			}
			BuildVisibility buildVisibility_ = BuildVisibility.Mothership;
			if (((Dictionary<object, object>)value).ContainsKey((object)"buildVisibility"))
			{
				string text = value.get_Item((object)"buildVisibility").ToString();
				if (text.CompareTo("Mothership") == 0)
				{
					buildVisibility_ = BuildVisibility.Mothership;
				}
				if (text.CompareTo("Tutorial") == 0)
				{
					buildVisibility_ = BuildVisibility.Tutorial;
				}
				if (text.CompareTo("All") == 0)
				{
					buildVisibility_ = BuildVisibility.All;
				}
				if (text.CompareTo("None") == 0)
				{
					buildVisibility_ = BuildVisibility.None;
				}
			}
			bool isIndestructible_ = false;
			if (((Dictionary<object, object>)value).ContainsKey((object)"isIndestructible"))
			{
				isIndestructible_ = Convert.ToBoolean(value.get_Item((object)"isIndestructible"));
			}
			uint num = 0u;
			if (((Dictionary<object, object>)value).ContainsKey((object)"ItemCategory"))
			{
				num = Convert.ToUInt32(value.get_Item((object)"ItemCategory"));
			}
			ItemCategory cat = (ItemCategory)num;
			uint placements = 63u;
			if (((Dictionary<object, object>)value).ContainsKey((object)"PlacementFaces"))
			{
				placements = Convert.ToUInt32(value.get_Item((object)"PlacementFaces"));
			}
			bool protonium = false;
			if (((Dictionary<object, object>)value).ContainsKey((object)"protoniumCrystal"))
			{
				protonium = Convert.ToBoolean(value.get_Item((object)"protoniumCrystal"));
			}
			SpecialCubesKind specialCubeKind_ = SpecialCubesKind.None;
			if (((Dictionary<object, object>)value).ContainsKey((object)"UnlockedByLeague"))
			{
				specialCubeKind_ = (Convert.ToBoolean(value.get_Item((object)"UnlockedByLeague")) ? SpecialCubesKind.LeagueBadge : SpecialCubesKind.None);
			}
			int leagueUnlockIndex_ = 0;
			if (((Dictionary<object, object>)value).ContainsKey((object)"LeagueUnlockIndex"))
			{
				leagueUnlockIndex_ = Convert.ToInt32(value.get_Item((object)"LeagueUnlockIndex"));
			}
			Dictionary<string, object> displayStats_ = null;
			if (((Dictionary<object, object>)value).ContainsKey((object)"DisplayStats"))
			{
				displayStats_ = (value.get_Item((object)"DisplayStats") as Dictionary<string, object>);
			}
			string descriptionStrKey_ = string.Empty;
			if (((Dictionary<object, object>)value).ContainsKey((object)"Description"))
			{
				descriptionStrKey_ = (value.get_Item((object)"Description") as string);
			}
			uint num2 = 0u;
			if (((Dictionary<object, object>)value).ContainsKey((object)"ItemSize"))
			{
				num2 = Convert.ToUInt32(value.get_Item((object)"ItemSize"));
			}
			ItemSize itemSize_ = (ItemSize)num2;
			ItemType itemType_ = ItemType.NotAFunctionalItem;
			if (((Dictionary<object, object>)value).ContainsKey((object)"ItemType"))
			{
				itemType_ = (ItemType)Enum.Parse(typeof(ItemType), (string)value.get_Item((object)"ItemType"));
			}
			int robotRanking_ = -1;
			if (((Dictionary<object, object>)value).ContainsKey((object)"robotRanking"))
			{
				robotRanking_ = Convert.ToInt32(value.get_Item((object)"robotRanking"));
			}
			bool isCosmetic_ = false;
			if (((Dictionary<object, object>)value).ContainsKey((object)"isCosmetic"))
			{
				isCosmetic_ = Convert.ToBoolean(value.get_Item((object)"IsCosmetic"));
			}
			CubeTypeID variantOf_ = 0u;
			if (((Dictionary<object, object>)value).ContainsKey((object)"variantOf"))
			{
				variantOf_ = ParseCubeID((string)value.get_Item((object)"variantOf"));
			}
			else if (((Dictionary<object, object>)value).ContainsKey((object)"requiredCubeId"))
			{
				variantOf_ = ParseCubeID((string)value.get_Item((object)"requiredCubeId"));
			}
			return new CubeListData(cpu, h, healthBoost_, cat, placements, protonium, displayStats_, descriptionStrKey_, specialCubeKind_, itemSize_, leagueUnlockIndex_, isIndestructible_, buildVisibility_, greyOutInTutorial_, itemType_, robotRanking_, isCosmetic_, variantOf_);
		}
	}
}
