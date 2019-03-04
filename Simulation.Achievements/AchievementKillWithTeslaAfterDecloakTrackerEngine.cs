using Achievements;
using Simulation.BattleTracker;
using Simulation.Hardware;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation.Achievements
{
	internal class AchievementKillWithTeslaAfterDecloakTrackerEngine : MultiEntityViewsEngine<AchievementMachineVisibilityNode, WeaponTrackerNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private int _localPlayerId = -1;

		private bool _localPlayerKilledWithTesla;

		private List<WeaponTrackerNode> _localPlayersTeslas = new List<WeaponTrackerNode>();

		private IEnumerator _decloakTimerTask;

		[Inject]
		private DestructionReporter destructionReporter
		{
			get;
			set;
		}

		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineKilled += CheckShooterIsLocalPlayer;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineKilled -= CheckShooterIsLocalPlayer;
		}

		private void StartDecloakedTimer(IMachineVisibilityComponent comp, int playerID)
		{
			if (_decloakTimerTask == null)
			{
				_decloakTimerTask = (IEnumerator)TaskRunner.get_Instance().Run((Func<IEnumerator>)CheckPlayerMadeAKill);
			}
		}

		private IEnumerator CheckPlayerMadeAKill()
		{
			_localPlayerKilledWithTesla = false;
			yield return (object)new WaitForSecondsEnumerator(2f);
			if (_localPlayerKilledWithTesla)
			{
				achievementManager.CompletedKillWithTeslaAfterDecloaked();
			}
			_decloakTimerTask = null;
		}

		private void CheckShooterIsLocalPlayer(int victimId, int shooterId)
		{
			if (_localPlayerId != shooterId || _localPlayersTeslas.Count <= 0)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < _localPlayersTeslas.Count)
				{
					if (_localPlayersTeslas[num].weaponActiveComponent.active)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_localPlayerKilledWithTesla = true;
		}

		protected override void Add(AchievementMachineVisibilityNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.machineVisibilityComponent.machineBecameVisible.subscribers += StartDecloakedTimer;
			}
		}

		protected override void Remove(AchievementMachineVisibilityNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.machineVisibilityComponent.machineBecameVisible.subscribers -= StartDecloakedTimer;
			}
		}

		protected override void Add(WeaponTrackerNode node)
		{
			IHardwareOwnerComponent ownerComponent = node.ownerComponent;
			if (ownerComponent.ownedByMe && node.itemDescriptorComponent.itemDescriptor.itemCategory == ItemCategory.Tesla)
			{
				_localPlayerId = ownerComponent.ownerId;
				_localPlayersTeslas.Add(node);
			}
		}

		protected override void Remove(WeaponTrackerNode node)
		{
			if (_localPlayersTeslas.Contains(node))
			{
				_localPlayersTeslas.Remove(node);
			}
		}
	}
}
