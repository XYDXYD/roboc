using LobbyServiceLayer;
using System;

namespace Mothership
{
	internal class BattleFoundObserver
	{
		public event Action<EnterBattleDependency> EnterBattleEvent;

		public event Action<EnterBattleDependency> BattleFoundEvent;

		public void EnterBattle(EnterBattleDependency dependency)
		{
			if (this.EnterBattleEvent != null)
			{
				this.EnterBattleEvent(dependency);
			}
		}

		public void BattleFound(EnterBattleDependency dependency)
		{
			if (this.BattleFoundEvent != null)
			{
				this.BattleFoundEvent(dependency);
			}
		}
	}
}
