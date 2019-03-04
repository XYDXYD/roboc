using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Pit
{
	internal class HUDPlayerIDDisplayPit : HUDPlayerIDDisplay, IInitialize, IWaitForFrameworkDestruction
	{
		private Dictionary<int, HUDPlayerIDWidgetPit> _playerIDWidgetsPit = new Dictionary<int, HUDPlayerIDWidgetPit>();

		void IInitialize.OnDependenciesInjected()
		{
			base.hudPlayerIDManager.OnLeaderUpdate += LeaderUpdate;
			base.hudPlayerIDManager.OnStreakUpdate += StreakUpdate;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			base.hudPlayerIDManager.OnLeaderUpdate -= LeaderUpdate;
			base.hudPlayerIDManager.OnStreakUpdate -= StreakUpdate;
		}

		protected override void AddExtraElements(int owner, GameObject gameObject)
		{
			HUDPlayerIDWidgetPit component = gameObject.GetComponent<HUDPlayerIDWidgetPit>();
			_playerIDWidgetsPit.Add(owner, component);
		}

		private void LeaderUpdate(int newLeaderId)
		{
			foreach (KeyValuePair<int, HUDPlayerIDWidgetPit> item in _playerIDWidgetsPit)
			{
				item.Value.SetIsLeader(item.Key == newLeaderId);
			}
		}

		private void StreakUpdate(int playerId, uint streak)
		{
			if (_playerIDWidgetsPit.ContainsKey(playerId))
			{
				_playerIDWidgetsPit[playerId].SetKillStreak(streak);
			}
		}
	}
}
