using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.BattleTracker
{
	internal sealed class LocalPlayerHealedOtherToFullHealthTracker : IInitialize, IWaitForFrameworkDestruction
	{
		public const float LAPSED_SECS_INVALIDATE_HEALING = 1.5f;

		private Dictionary<int, PlayerBeingHealed> _playersBeingHealed = new Dictionary<int, PlayerBeingHealed>();

		private LocalPlayerHealedOtherToFullHealthObservable _observable;

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		private MachineCpuDataManager machineCPUDataManager
		{
			get;
			set;
		}

		public LocalPlayerHealedOtherToFullHealthTracker(LocalPlayerHealedOtherToFullHealthObservable observable)
		{
			_observable = observable;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineCPUDataManager.OnMachineCpuChanged += CheckHealerIsLocalPlayer;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineCPUDataManager.OnMachineCpuChanged -= CheckHealerIsLocalPlayer;
		}

		private void CheckHealerIsLocalPlayer(int shooterId, TargetType shooterType, int playerId, float percent)
		{
			if (!(percent > 0f) || shooterId == playerId || !playerTeamsContainer.IsOnMyTeam(TargetType.Player, playerId) || !playerTeamsContainer.IsMe(TargetType.Player, shooterId))
			{
				return;
			}
			int num = Mathf.CeilToInt(percent * 100f);
			if (_playersBeingHealed.ContainsKey(playerId))
			{
				PlayerBeingHealed playerBeingHealed = _playersBeingHealed[playerId];
				if ((DateTime.UtcNow - playerBeingHealed.LastHealTime).TotalSeconds <= 1.5)
				{
					if (num == 100)
					{
						_playersBeingHealed.Remove(playerId);
						_observable.Dispatch(ref playerBeingHealed.StartHealthPercent);
					}
					else
					{
						playerBeingHealed.LastHealTime = DateTime.UtcNow;
					}
				}
				else
				{
					_playersBeingHealed.Remove(playerId);
				}
			}
			else if (num < 100)
			{
				_playersBeingHealed.Add(playerId, new PlayerBeingHealed(DateTime.UtcNow, num));
			}
		}
	}
}
