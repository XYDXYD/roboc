using Svelto.DataStructures;
using System.Collections.Generic;

namespace Simulation
{
	public sealed class LocalPlayerDailyQuestProgress
	{
		public GameModeType gameMode;

		public bool isRanked;

		public bool isBrawl;

		public bool isCustomGame;

		public bool gameEnded;

		public bool gameWon;

		public string playerRobotUniqueId = string.Empty;

		public FasterList<string> partyPlayerNames = new FasterList<string>();

		public Dictionary<ItemCategory, int> killCountWithWeapon = new Dictionary<ItemCategory, int>();

		public Dictionary<int, int> playerHealStartHealthPercent = new Dictionary<int, int>();
	}
}
