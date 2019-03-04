using Achievements;
using Simulation.BattleTracker;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation.Achievements
{
	internal sealed class AchievementHealerTracker : IWaitForFrameworkDestruction
	{
		private LocalPlayerHealedOtherToFullHealthObserver _observer;

		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		public unsafe AchievementHealerTracker(LocalPlayerHealedOtherToFullHealthObserver observer)
		{
			_observer = observer;
			_observer.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_observer.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void CheckHealAchievement(ref int startingHealth)
		{
			if (startingHealth <= 20)
			{
				achievementManager.CompletedHealFrom20To100();
			}
		}
	}
}
