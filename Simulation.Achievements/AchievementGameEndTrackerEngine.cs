using Achievements;
using Battle;
using Simulation.BattleTracker;
using Simulation.Hardware;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using System;
using System.Collections.Generic;

namespace Simulation.Achievements
{
	internal class AchievementGameEndTrackerEngine : SingleEntityViewEngine<MovementTrackerNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private List<ItemCategory> _localPlayerMovementCategories = new List<ItemCategory>();

		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		[Inject]
		private GameEndedObserver gameEndedObserver
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
		private BattlePlayers battlePlayers
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

		void IInitialize.OnDependenciesInjected()
		{
			gameEndedObserver.OnGameEnded += SendAchievements;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameEndedObserver.OnGameEnded -= SendAchievements;
		}

		public Type[] AcceptedNodes()
		{
			return new Type[1]
			{
				typeof(MovementTrackerNode)
			};
		}

		protected override void Add(MovementTrackerNode obj)
		{
			if (obj == null)
			{
				return;
			}
			IHardwareOwnerComponent ownerComponent = obj.ownerComponent;
			if (obj.ownerComponent.ownedByMe)
			{
				ItemCategory itemCategory = obj.itemDescriptorComponent.itemDescriptor.itemCategory;
				if (itemCategory == ItemCategory.Rudder)
				{
					itemCategory = ItemCategory.Wing;
				}
				if (!_localPlayerMovementCategories.Contains(itemCategory))
				{
					_localPlayerMovementCategories.Add(itemCategory);
				}
			}
		}

		protected override void Remove(MovementTrackerNode obj)
		{
		}

		private void SendAchievements(bool won)
		{
			for (int i = 0; i < _localPlayerMovementCategories.Count; i++)
			{
				achievementManager.CompletedBattle(_localPlayerMovementCategories[i]);
			}
			if (!won)
			{
				return;
			}
			int myPlatoonId = battlePlayers.MyPlatoonId;
			if (myPlatoonId != 255)
			{
				int num = 0;
				ICollection<int> connectedPlayerIds = connectedPlayersContainer.GetConnectedPlayerIds();
				using (IEnumerator<int> enumerator = connectedPlayerIds.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string playerName = playerNamesContainer.GetPlayerName(enumerator.Current);
						int platoonId = battlePlayers.GetPlatoonId(playerName);
						if (myPlatoonId == platoonId)
						{
							num++;
						}
					}
				}
				if (num == 5)
				{
					achievementManager.CompletedBattleWithFullParty();
				}
			}
		}
	}
}
