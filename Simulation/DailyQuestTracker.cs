using Authentication;
using Battle;
using Simulation.BattleTracker;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class DailyQuestTracker : IInitialize, IWaitForFrameworkDestruction
	{
		private LocalPlayerDailyQuestProgress _localPlayerProgress = new LocalPlayerDailyQuestProgress();

		private LocalPlayerMadeKillObserver _playerMadeKillObserver;

		private LocalPlayerHealedOtherToFullHealthObserver _playerHealedOtherObserver;

		[Inject]
		private GameEndedObserver gameEndedObserver
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private WorldSwitching worldSwitching
		{
			get;
			set;
		}

		[Inject]
		private BattlePlayers battlePlayers
		{
			get;
			set;
		}

		[Inject]
		private PlayerNamesContainer playerNamesContainer
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		private ConnectedPlayersContainer connectedPlayersContainer
		{
			get;
			set;
		}

		public unsafe DailyQuestTracker(LocalPlayerMadeKillObserver killObserver, LocalPlayerHealedOtherToFullHealthObserver healObserver)
		{
			_playerMadeKillObserver = killObserver;
			_playerHealedOtherObserver = healObserver;
			_playerMadeKillObserver.AddAction(new ObserverAction<PlayerKillData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_playerHealedOtherObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void OnDependenciesInjected()
		{
			gameEndedObserver.OnGameEnded += GameEnded;
			worldSwitching.OnWorldIsSwitching.Add(OnWorldSwitching());
		}

		public unsafe void OnFrameworkDestroyed()
		{
			gameEndedObserver.OnGameEnded -= GameEnded;
			_playerMadeKillObserver.RemoveAction(new ObserverAction<PlayerKillData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void TrackPlayerMadeKill(ref PlayerKillData data)
		{
			ItemCategory activeWeapon = data.activeWeapon;
			if (!_localPlayerProgress.killCountWithWeapon.ContainsKey(activeWeapon))
			{
				_localPlayerProgress.killCountWithWeapon.Add(activeWeapon, 0);
			}
			Dictionary<ItemCategory, int> killCountWithWeapon;
			ItemCategory key;
			(killCountWithWeapon = _localPlayerProgress.killCountWithWeapon)[key = activeWeapon] = killCountWithWeapon[key] + 1;
		}

		private void TrackPlayerHealedOthers(ref int startingHealth)
		{
			if (!_localPlayerProgress.playerHealStartHealthPercent.ContainsKey(startingHealth))
			{
				_localPlayerProgress.playerHealStartHealthPercent.Add(startingHealth, 0);
			}
			Dictionary<int, int> playerHealStartHealthPercent;
			int key;
			(playerHealStartHealthPercent = _localPlayerProgress.playerHealStartHealthPercent)[key = startingHealth] = playerHealStartHealthPercent[key] + 1;
		}

		private IEnumerator OnWorldSwitching()
		{
			if (!_localPlayerProgress.gameEnded)
			{
				UpdateAndSendDailyQuestProgress();
			}
			yield break;
		}

		private void GameEnded(bool won)
		{
			_localPlayerProgress.gameEnded = true;
			_localPlayerProgress.gameWon = won;
			UpdateAndSendDailyQuestProgress();
		}

		private void UpdateAndSendDailyQuestProgress()
		{
			Dictionary<string, PlayerDataDependency> expectedPlayersDict = battlePlayers.GetExpectedPlayersDict();
			PlayerDataDependency playerDataDependency = expectedPlayersDict[User.Username];
			_localPlayerProgress.gameMode = WorldSwitching.GetGameModeType();
			_localPlayerProgress.isRanked = WorldSwitching.IsRanked();
			_localPlayerProgress.isBrawl = WorldSwitching.IsBrawl();
			_localPlayerProgress.isCustomGame = WorldSwitching.IsCustomGame();
			_localPlayerProgress.playerRobotUniqueId = playerDataDependency.RobotUniqueId;
			int platoonId = playerDataDependency.PlatoonId;
			Dictionary<string, bool> currentBattlesCompletedPlayerNames = connectedPlayersContainer.GetCurrentBattlesCompletedPlayerNames();
			foreach (KeyValuePair<string, bool> item in currentBattlesCompletedPlayerNames)
			{
				string key = item.Key;
				int platoonId2 = battlePlayers.GetPlatoonId(key);
				if (platoonId != 255 && platoonId == platoonId2)
				{
					_localPlayerProgress.partyPlayerNames.Add(key);
				}
			}
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SendDailyQuestProgress);
		}

		private IEnumerator SendDailyQuestProgress()
		{
			loadingIconPresenter.NotifyLoading("UpdatingDailyQuestsProgress");
			IUpdatePlayerDailyQuestProgressRequest request = serviceFactory.Create<IUpdatePlayerDailyQuestProgressRequest, LocalPlayerDailyQuestProgress>(_localPlayerProgress);
			TaskService task = new TaskService(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("UpdatingDailyQuestsProgress");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("UpdatingDailyQuestsProgress");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("UpdatingDailyQuestsProgress");
		}
	}
}
