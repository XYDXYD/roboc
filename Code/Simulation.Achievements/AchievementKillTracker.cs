using Achievements;
using Simulation.BattleTracker;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation.Achievements
{
	internal class AchievementKillTracker : IWaitForFrameworkDestruction
	{
		private LocalPlayerMadeKillObserver _observer;

		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		public unsafe AchievementKillTracker(LocalPlayerMadeKillObserver observer)
		{
			_observer = observer;
			_observer.AddAction(new ObserverAction<PlayerKillData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_observer.RemoveAction(new ObserverAction<PlayerKillData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void CheckKillAchievement(ref PlayerKillData data)
		{
			achievementManager.MadeAKill(data.activeWeapon);
			FasterList<ItemCategory> victimMovements = data.victimMovements;
			for (int i = 0; i < victimMovements.get_Count(); i++)
			{
				ItemCategory itemCategory = victimMovements.get_Item(i);
				achievementManager.MadeAKill(itemCategory);
			}
		}
	}
}
