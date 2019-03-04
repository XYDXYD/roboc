using Fabric;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Simulation.Pit
{
	internal sealed class PitModeHudPresenter : IWaitForFrameworkDestruction, IInitialize
	{
		private PitModeHudView _pitModeHudView;

		private Dictionary<int, PitStatData> _playerIdToPlayerStats;

		private int _previousLeader = int.MinValue;

		private bool _gameEnded;

		[Inject]
		internal ConnectedPlayersContainer connectedPlayers
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal VOManager voManager
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			ConnectedPlayersContainer connectedPlayers = this.connectedPlayers;
			connectedPlayers.OnPlayerDisconnected = (Action<int>)Delegate.Combine(connectedPlayers.OnPlayerDisconnected, new Action<int>(HandleOnPlayerDisconnected));
			gameEndedObserver.OnGameEnded += GameEndedDisableAnim;
		}

		public void OnFrameworkDestroyed()
		{
			ConnectedPlayersContainer connectedPlayers = this.connectedPlayers;
			connectedPlayers.OnPlayerDisconnected = (Action<int>)Delegate.Remove(connectedPlayers.OnPlayerDisconnected, new Action<int>(HandleOnPlayerDisconnected));
			gameEndedObserver.OnGameEnded -= GameEndedDisableAnim;
		}

		public void RegisterView(PitModeHudView pitModeHudView)
		{
			_pitModeHudView = pitModeHudView;
		}

		public void UnregisterView(PitModeHudView pitModeHudView)
		{
			_pitModeHudView = null;
		}

		public void UpdateDisplay(int shootingPlayerId, uint shootersLatestScore, uint streak, int destroyedPlayerId, int leaderId)
		{
			int localPlayerId = playerTeamsContainer.localPlayerId;
			if (!_gameEnded)
			{
				CheckForAndPlayCorrectLeaderSound(localPlayerId, shootingPlayerId, destroyedPlayerId, leaderId);
				if (localPlayerId == shootingPlayerId)
				{
					DisplayStreakAnim(streak);
				}
				else if (streak == 5)
				{
					_pitModeHudView.ShowStreakTextAnim(streak, isOwnPlayer: false, playerNamesContainer.GetDisplayName(shootingPlayerId));
				}
			}
			UpdateEventStream(shootingPlayerId, destroyedPlayerId, leaderId, streak);
			UpdateStats(localPlayerId, shootingPlayerId, destroyedPlayerId, shootersLatestScore, streak);
			UpdateHUD(leaderId, localPlayerId);
			if (_previousLeader != leaderId && _previousLeader < 0)
			{
				_pitModeHudView.ShowHud();
			}
			_previousLeader = leaderId;
		}

		internal void InitialiseStats(Dictionary<int, uint> scores, Dictionary<int, uint> streaks, int leader)
		{
			if (_playerIdToPlayerStats == null)
			{
				SetupScoreBoard();
			}
			if (leader >= 0)
			{
				int localPlayerId = playerTeamsContainer.localPlayerId;
				CheckPlayerExists(leader);
				foreach (KeyValuePair<int, uint> score in scores)
				{
					CheckPlayerExists(score.Key);
					PitStatData value = _playerIdToPlayerStats[score.Key];
					value.Score = score.Value;
					value.Streak = streaks[score.Key];
					_playerIdToPlayerStats[score.Key] = value;
				}
				UpdateHUD(leader, localPlayerId);
			}
		}

		private void HandleOnPlayerDisconnected(int playerId)
		{
			_playerIdToPlayerStats.Remove(playerId);
			if (_previousLeader < 0)
			{
				return;
			}
			if (playerId == _previousLeader)
			{
				_previousLeader = int.MinValue;
				uint num = 0u;
				using (Dictionary<int, PitStatData>.Enumerator enumerator = _playerIdToPlayerStats.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PitStatData value = enumerator.Current.Value;
						if (value.Score > num)
						{
							PitStatData value2 = enumerator.Current.Value;
							num = value2.Score;
							_previousLeader = enumerator.Current.Key;
						}
					}
				}
				if (_previousLeader != int.MinValue)
				{
					PlayNewLeaderEffect(playerTeamsContainer.localPlayerId, _previousLeader, _previousLeader);
				}
			}
			_pitModeHudView.ClearStats();
			if (_previousLeader > int.MinValue)
			{
				UpdateHUD(_previousLeader, playerTeamsContainer.localPlayerId);
			}
		}

		private void UpdateHUD(int leaderId, int ownPlayerId)
		{
			List<int> leaderboard = GetLeaderboard(leaderId);
			int num = leaderboard.IndexOf(ownPlayerId);
			_pitModeHudView.DisplayLeader(_playerIdToPlayerStats[leaderboard[0]], num == 0);
			if (leaderboard.Count > 1)
			{
				PitStatData pitStatData = _playerIdToPlayerStats[leaderboard[1]];
				if (pitStatData.Score != 0)
				{
					_pitModeHudView.DisplaySideLeaderboardSlot1(_playerIdToPlayerStats[leaderboard[1]], num == 1);
					if (leaderboard.Count > 2)
					{
						PitStatData pitStatData2 = _playerIdToPlayerStats[leaderboard[2]];
						if (pitStatData2.Score != 0)
						{
							_pitModeHudView.DisplaySideLeaderboardSlot2(_playerIdToPlayerStats[leaderboard[2]], num == 2);
							goto IL_00eb;
						}
					}
					_pitModeHudView.ResetSideLeaderboardSlot2();
					goto IL_00eb;
				}
			}
			_pitModeHudView.ResetSideLeaderboardSlot1();
			goto IL_00eb;
			IL_00eb:
			_pitModeHudView.DisplayOwnStats(num == 0, num + 1, _playerIdToPlayerStats[ownPlayerId]);
		}

		private void UpdateEventStream(int shootingPlayerId, int destroyedPlayerId, int leaderId, uint streak)
		{
			CheckPlayerExists(shootingPlayerId);
			CheckPlayerExists(destroyedPlayerId);
			CheckPlayerExists(leaderId);
			if (leaderId != _previousLeader)
			{
				commandFactory.Build<UpdateLeaderCommand>().Inject(leaderId).Execute();
			}
			PitStatData pitStatData = _playerIdToPlayerStats[destroyedPlayerId];
			if (pitStatData.Streak > 1)
			{
				commandFactory.Build<KillStreakLostCommand>().Inject(destroyedPlayerId).Execute();
			}
			commandFactory.Build<UpdateKillStreakCommand>().Inject(new UpdateKillStreamDependency(shootingPlayerId, streak)).Execute();
		}

		private void CreateStatsForPlayer(int playerId)
		{
			PitStatData value = default(PitStatData);
			value.Name = playerNamesContainer.GetDisplayName(playerId);
			value.Score = (value.Streak = 0u);
			_playerIdToPlayerStats[playerId] = value;
		}

		private void SetupScoreBoard()
		{
			_playerIdToPlayerStats = new Dictionary<int, PitStatData>();
			ICollection<int> connectedPlayerIds = connectedPlayers.GetConnectedPlayerIds();
			foreach (int item in connectedPlayerIds)
			{
				CreateStatsForPlayer(item);
			}
			if (_previousLeader >= 0)
			{
				_pitModeHudView.ShowHud();
			}
		}

		private void CheckPlayerExists(int playerId)
		{
			if (!_playerIdToPlayerStats.ContainsKey(playerId))
			{
				CreateStatsForPlayer(playerId);
			}
		}

		private List<int> GetLeaderboard(int leaderId)
		{
			List<int> list = new List<int>();
			list.Add(leaderId);
			foreach (int key2 in _playerIdToPlayerStats.Keys)
			{
				if (leaderId != key2)
				{
					bool flag = false;
					for (int i = 0; i < list.Count; i++)
					{
						int key = list[i];
						PitStatData pitStatData = _playerIdToPlayerStats[key];
						uint score = pitStatData.Score;
						PitStatData pitStatData2 = _playerIdToPlayerStats[key2];
						if (score < pitStatData2.Score)
						{
							flag = true;
							list.Insert(i, key2);
							break;
						}
					}
					if (!flag)
					{
						list.Add(key2);
					}
				}
			}
			return list;
		}

		private void CheckForAndPlayCorrectLeaderSound(int ownPlayerId, int shootingPlayerId, int destroyedPlayerId, int currentLeaderId)
		{
			if (_previousLeader != int.MinValue)
			{
				if (shootingPlayerId == ownPlayerId && destroyedPlayerId == _previousLeader)
				{
					EventManager.get_Instance().PostEvent("Pit_YouKilledLeader", 0);
					_pitModeHudView.ShowNotification(StringTableBase<StringTable>.Instance.GetString("strYouKilledLeader"), 0u, isOwnPlayer: false);
					voManager.PlayVO("VO_Pit_YouKilledLeader");
				}
				else if (destroyedPlayerId == ownPlayerId && shootingPlayerId == currentLeaderId)
				{
					EventManager.get_Instance().PostEvent("Pit_LeaderKilledYou", 0);
					voManager.PlayVO("VO_Pit_LeaderKilledYou");
				}
				else if (_previousLeader != currentLeaderId)
				{
					PlayNewLeaderEffect(ownPlayerId, shootingPlayerId, currentLeaderId);
				}
			}
		}

		private void PlayNewLeaderEffect(int ownPlayerId, int shootingPlayerId, int currentLeaderId)
		{
			EventManager.get_Instance().PostEvent((ownPlayerId != currentLeaderId) ? "Pit_NewLeader" : "Pit_YouLeader", 0);
			_pitModeHudView.ShowNotification((ownPlayerId != currentLeaderId) ? StringTableBase<StringTable>.Instance.GetReplaceString("strPlayerHasTakenLead", "{playerName}", playerNamesContainer.GetDisplayName(shootingPlayerId)) : StringTableBase<StringTable>.Instance.GetString("strYouAreLeader"), 0u, ownPlayerId == currentLeaderId);
			voManager.PlayVO((ownPlayerId != currentLeaderId) ? "VO_Pit_NewLeader" : "VO_Pit_YouLeader");
		}

		private void DisplayStreakAnim(uint streak)
		{
			switch (streak)
			{
			case 0u:
			case 1u:
				break;
			case 2u:
				_pitModeHudView.ShowStreakAnim(streak);
				EventManager.get_Instance().PostEvent("Pit_Rampant", 0);
				voManager.PlayVO("VO_Pit_Rampant");
				break;
			case 3u:
				_pitModeHudView.ShowStreakAnim(streak);
				EventManager.get_Instance().PostEvent("Pit_Dominating", 0);
				voManager.PlayVO("VO_Pit_Dominating");
				break;
			case 4u:
				_pitModeHudView.ShowStreakAnim(streak);
				EventManager.get_Instance().PostEvent("Pit_Unstoppable", 0);
				voManager.PlayVO("VO_Pit_Unstoppable");
				break;
			case 5u:
				_pitModeHudView.ShowStreakAnim(streak);
				EventManager.get_Instance().PostEvent("Pit_Legendary", 0);
				voManager.PlayVO("VO_Pit_Legendary");
				break;
			}
		}

		private void UpdateStats(int ownPlayerId, int shootingPlayerId, int destroyedPlayerId, uint shootersLatestScore, uint streak)
		{
			CheckPlayerExists(shootingPlayerId);
			CheckPlayerExists(destroyedPlayerId);
			PitStatData value = _playerIdToPlayerStats[shootingPlayerId];
			PitStatData value2 = _playerIdToPlayerStats[destroyedPlayerId];
			value.Score = shootersLatestScore;
			value.Streak = streak;
			if (ownPlayerId == destroyedPlayerId && value2.Streak != 0)
			{
				_pitModeHudView.StreakLost();
			}
			value2.Streak = 0u;
			_playerIdToPlayerStats[shootingPlayerId] = value;
			_playerIdToPlayerStats[destroyedPlayerId] = value2;
		}

		private void GameEndedDisableAnim(bool won)
		{
			_gameEnded = true;
		}
	}
}
