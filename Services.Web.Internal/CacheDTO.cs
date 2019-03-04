using GameServerServiceLayer;
using Services.Mothership;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;

namespace Services.Web.Internal
{
	[Obsolete("<color=white>CacheDTO is deprecated for WebRequest</color>")]
	internal static class CacheDTO
	{
		internal static uint[] currentGarageSlot = null;

		internal static FasterList<int> garageSlotOrder = new FasterList<int>();

		internal static Dictionary<uint, GarageSlotData> garageSlots = new Dictionary<uint, GarageSlotData>();

		internal static CircularBufferIndexer<int, byte[]> robotShopRobotData = new CircularBufferIndexer<int, byte[]>(20);

		internal static CircularBufferIndexer<int, byte[]> robotShopColourData = new CircularBufferIndexer<int, byte[]>(20);

		internal static LoadRobotShopConfigResponse robotShopConfig;

		internal static byte[] hintProgresses = null;

		internal static Dictionary<CubeTypeID, CubeListData> cubeList = null;

		internal static Dictionary<uint, SpecialItemListData> specialItemList = null;

		internal static uint? garageSlotLimit = null;

		internal static float? depotSellScale = null;

		internal static Dictionary<uint, uint> inventory = null;

		internal static PremiumData premiumData = null;

		internal static ChatSettingsData? chatSettings = null;

		internal static IncomeScalesResponse? incomeScales = null;

		internal static uint? levelRequiredForLeagueGame = null;

		internal static uint? cpuRequiredForLeagueGame = null;

		internal static HashSet<string> chatIgnores = null;

		internal static GameModeSettingsDependency gameModeSettings = null;

		internal static ColorPaletteData defaultColorPalette = null;

		internal static AutoRegenHealthSettingsData autoRegenHealthSettings = null;

		internal static Dictionary<int, WeaponStatsData> weaponStats = null;

		internal static PowerBarSettingsData powerBarSettingsData = null;

		internal static AvatarInfo localPlayerAvatarInfo = null;

		internal static ScoreMultipliersData scoreMultipliersData = null;

		internal static List<int> defaultWeaponOrderSubcategories = null;

		internal static string[] PublicChatChannelNames = null;

		internal static int? tutorialstage = null;

		internal static TutorialEnemyMachineData tutorialEnemyMachineData = null;
	}
}
