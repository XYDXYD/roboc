using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.BattleArena.Equalizer
{
	internal class EqualizerActivationEngine : SingleEntityViewEngine<EqualizerActivationNode>, IInitialize
	{
		private EqualizerActivationNode _node;

		private EqualizerNotificationObserver _equalizerNotificationObserver;

		[Inject]
		public IMinimapPresenter minimapPresenter
		{
			private get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		public MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public EqualizerActivationEngine(EqualizerNotificationObserver observer)
		{
			_equalizerNotificationObserver = observer;
		}

		public void OnDependenciesInjected()
		{
			spawnDispatcher.OnEntitySpawnedIn += OnEntitySpawnedIn;
		}

		protected override void Add(EqualizerActivationNode node)
		{
			_node = node;
		}

		protected unsafe override void Remove(EqualizerActivationNode node)
		{
			_node = null;
			_equalizerNotificationObserver.RemoveAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnEntitySpawnedIn(SpawnInParametersEntity obj)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (obj.Type == TargetType.EqualizerCrystal)
			{
				_node.rbComponent.activePosition = _node.rbComponent.rb.get_position();
				_node.rbComponent.inactivePosition = _node.rbComponent.activePosition - new Vector3(0f, 20f, 0f);
				_node.rbComponent.rb.set_position(_node.rbComponent.inactivePosition);
				_equalizerNotificationObserver.AddAction(new ObserverAction<EqualizerNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void OnNotificationReceived(ref EqualizerNotificationDependency parameter)
		{
			switch (parameter.EqualizerNotific)
			{
			case EqualizerNotification.ActivationWarning:
				break;
			case EqualizerNotification.Cancelled:
				break;
			case EqualizerNotification.Activate:
				ActivateCrystal(parameter);
				minimapPresenter.SetEqualizerOwner(_node.visualTeamComponent.visualTeam == VisualTeam.MyTeam);
				break;
			case EqualizerNotification.Deactivated:
			case EqualizerNotification.Defended:
				DeactivateCrystal(parameter);
				minimapPresenter.HideEqualizer();
				break;
			case EqualizerNotification.Destroyed:
				DestroyCrystal(parameter);
				minimapPresenter.HideEqualizer();
				break;
			}
		}

		private void DeactivateCrystal(EqualizerNotificationDependency parameter)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			string trigger = (_node.visualTeamComponent.visualTeam != 0) ? "deactivate_enemy" : "deactivate";
			_node.animatorComponent.animator.SetTrigger(trigger);
			_node.teamComponent.ownerTeamId = -1;
			_node.visualTeamComponent.visualTeam = VisualTeam.None;
			SetAlive(isAlive: false);
			TaskRunner.get_Instance().Run(MoveRigidBody(_node.rbComponent.inactivePosition, 1.33f));
		}

		private void DestroyCrystal(EqualizerNotificationDependency parameter)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			string trigger = (_node.visualTeamComponent.visualTeam != 0) ? "explosion_enemy" : "explosion";
			_node.animatorComponent.animator.SetTrigger(trigger);
			_node.teamComponent.ownerTeamId = -1;
			_node.visualTeamComponent.visualTeam = VisualTeam.None;
			SetAlive(isAlive: false);
			TaskRunner.get_Instance().Run(MoveRigidBody(_node.rbComponent.inactivePosition, 1.33f));
		}

		private void ActivateCrystal(EqualizerNotificationDependency parameter)
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			int teamID = parameter.TeamID;
			_node.teamComponent.ownerTeamId = teamID;
			_node.visualTeamComponent.visualTeam = ((teamID != playerTeamsContainer.GetMyTeam()) ? VisualTeam.EnemyTeam : VisualTeam.MyTeam);
			SetAlive(isAlive: true);
			if (parameter.Health < 0)
			{
				ResetHealth(_node.machineMapComponent.machineMap);
			}
			else
			{
				SetHealth(_node.machineMapComponent.machineMap, parameter.Health);
			}
			string trigger = (_node.visualTeamComponent.visualTeam != 0) ? "spawn_enemy" : "spawn";
			_node.animatorComponent.animator.SetTrigger(trigger);
			TaskRunner.get_Instance().Run(MoveRigidBody(_node.rbComponent.activePosition, 1.33f));
		}

		private void SetHealth(IMachineMap machineMap, int health)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			FasterList<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			FasterListEnumerator<InstantiatedCube> enumerator = allInstantiatedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.get_Current();
				current.health = health;
			}
		}

		private IEnumerator MoveRigidBody(Vector3 toPosition, float duration)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 initialPosition = _node.rbComponent.rb.get_position();
			float timer = 0f;
			while (true)
			{
				float num;
				timer = (num = timer + Time.get_deltaTime());
				if (!(num < duration))
				{
					break;
				}
				Vector3 position = Vector3.Lerp(initialPosition, toPosition, Mathf.Clamp01(timer / duration));
				_node.rbComponent.rb.MovePosition(position);
				yield return null;
			}
			_node.rbComponent.rb.MovePosition(toPosition);
		}

		private void ResetHealth(IMachineMap machineMap)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			FasterList<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			FasterListEnumerator<InstantiatedCube> enumerator = allInstantiatedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.get_Current();
				current.health = current.totalHealth;
			}
		}

		private void SetAlive(bool isAlive)
		{
			if (isAlive)
			{
				livePlayersContainer.MarkAsLive(TargetType.EqualizerCrystal, _node.ownerComponent.playerId);
			}
			else
			{
				livePlayersContainer.MarkAsDead(TargetType.EqualizerCrystal, _node.ownerComponent.playerId);
			}
			playerTeamsContainer.ChangePlayerTeam(TargetType.EqualizerCrystal, _node.ownerComponent.playerId, _node.teamComponent.ownerTeamId);
		}
	}
}
