using Simulation.Hardware.Weapons;
using System.Text;
using UnityEngine;

namespace Simulation.Pit
{
	internal class BattleEventStreamDisplayPit : BattleEventStreamDisplay
	{
		protected override void RegisterListeners()
		{
			base.RegisterListeners();
			IBattleEventStreamManagerPit battleEventStreamManagerPit = base.battleEventStreamManager as IBattleEventStreamManagerPit;
			battleEventStreamManagerPit.OnNewLeader += NewLeader;
			battleEventStreamManagerPit.OnStreakUpdate += StreakUpdate;
			battleEventStreamManagerPit.OnStreakLost += StreakLost;
		}

		protected override void DeregisterListeners()
		{
			base.DeregisterListeners();
			IBattleEventStreamManagerPit battleEventStreamManagerPit = base.battleEventStreamManager as IBattleEventStreamManagerPit;
			battleEventStreamManagerPit.OnNewLeader -= NewLeader;
			battleEventStreamManagerPit.OnStreakUpdate -= StreakUpdate;
			battleEventStreamManagerPit.OnStreakLost -= StreakLost;
		}

		private void NewLeader(int playerId)
		{
			StringBuilder stringBuilder = new StringBuilder(StringTableBase<StringTable>.Instance.GetString("strPitIsNewLeader"));
			stringBuilder.Replace("{playerColour}", GetColourTag(playerId)).Replace("{playerName}", base.playerNamesContainer.GetDisplayName(playerId));
			BattleEventData battleEventData = new BattleEventData(stringBuilder.ToString(), Time.get_time());
			battleEvents.Add(battleEventData);
			UpdateBattleStreamDisplay();
		}

		private void StreakUpdate(int playerId, uint streak)
		{
			if (streak >= 2 && streak <= 5)
			{
				StringBuilder stringBuilder = new StringBuilder(StringTableBase<StringTable>.Instance.GetString("strPitIs"));
				stringBuilder.Replace("{playerColour}", GetColourTag(playerId)).Replace("{playerName}", base.playerNamesContainer.GetDisplayName(playerId)).Replace("{pitTitle}", PitUtils.TitleForStreak(streak));
				BattleEventData battleEventData = new BattleEventData(stringBuilder.ToString(), Time.get_time());
				battleEvents.Add(battleEventData);
				UpdateBattleStreamDisplay();
			}
		}

		private void StreakLost(int playerId)
		{
			StringBuilder stringBuilder = new StringBuilder(StringTableBase<StringTable>.Instance.GetString("strPitLostStreak"));
			stringBuilder.Replace("{playerColour}", GetColourTag(playerId)).Replace("{playerName}", base.playerNamesContainer.GetDisplayName(playerId));
			BattleEventData battleEventData = new BattleEventData(stringBuilder.ToString(), Time.get_time());
			battleEvents.Add(battleEventData);
			UpdateBattleStreamDisplay();
		}

		private string GetColourTag(int playerId)
		{
			return (!base.playerTeamsContainer.IsMe(TargetType.Player, playerId)) ? "[FF3C3CFF]" : "[28DC82FF]";
		}
	}
}
