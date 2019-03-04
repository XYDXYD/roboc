using Battle;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.BattleArena.EnemyFusionShield
{
	internal class EnemyShieldDegenerateHealthEngine : IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private const float SEND_RATE = 0.5f;

		private float _globalTimer;

		private int _damagePerSecond;

		private Dictionary<InstantiatedCube, int> _proposedDamagedCubes = new Dictionary<InstantiatedCube, int>(30);

		private List<HitCubeInfo> _damagedCubes = new List<HitCubeInfo>(20);

		private DamagedByEnemyShieldDependency _damagedByEnemyShieldDependency = new DamagedByEnemyShieldDependency();

		private PlayerDamagedByEnemyShieldCommand _damagedCommand;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal CubeDamagePropagator cubeDamagePropagator
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager networkMachineManager
		{
			private get;
			set;
		}

		[Inject]
		internal BattleTimer battleTimer
		{
			get;
			private set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_damagedCommand = commandFactory.Build<PlayerDamagedByEnemyShieldCommand>();
			gameStartDispatcher.Register(OnGameStart);
			IGetGameClientSettingsRequest getGameClientSettingsRequest = serviceFactory.Create<IGetGameClientSettingsRequest>();
			getGameClientSettingsRequest.SetAnswer(new ServiceAnswer<GameClientSettingsDependency>(delegate(GameClientSettingsDependency data)
			{
				_damagePerSecond = data.fusionShieldDPS;
			}));
			getGameClientSettingsRequest.Execute();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
		}

		private void OnGameStart()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Update);
		}

		private IEnumerator Update()
		{
			while (true)
			{
				_globalTimer += Time.get_deltaTime();
				FasterReadOnlyList<DegenerateHealthMachineEntityView> playerMachines = entityViewsDB.QueryEntityViews<DegenerateHealthMachineEntityView>();
				for (int i = 0; i < playerMachines.get_Count(); i++)
				{
					ProcessMachine(playerMachines.get_Item(i));
				}
				if (_globalTimer > 0.5f)
				{
					ComputeAndSendData();
					_globalTimer = 0f;
				}
				yield return null;
			}
		}

		private void ProcessMachine(DegenerateHealthMachineEntityView playerMachine)
		{
			if (!playerMachine.aliveStateComponent.isAlive.get_value())
			{
				return;
			}
			if (ShouldTakeDamage(playerMachine))
			{
				if (!WasTakingDamageInsideShield(playerMachine))
				{
					playerMachine.fusionShieldHealthChangeComponent.isTakingDamage = true;
				}
				playerMachine.fusionShieldHealthChangeComponent.timeHealthChanging += Time.get_deltaTime();
			}
			else if (WasTakingDamageInsideShield(playerMachine))
			{
				playerMachine.fusionShieldHealthChangeComponent.isTakingDamage = false;
			}
		}

		private void ComputeAndSendData()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<DegenerateHealthMachineEntityView> val = entityViewsDB.QueryEntityViews<DegenerateHealthMachineEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				DegenerateHealthMachineEntityView degenerateHealthMachineEntityView = val.get_Item(i);
				if (degenerateHealthMachineEntityView.fusionShieldHealthChangeComponent.isTakingDamage)
				{
					ResetData();
					ComputeDamagedCubes(degenerateHealthMachineEntityView);
					SendDataToServer(degenerateHealthMachineEntityView.machineOwnerComponent.ownerMachineId);
					degenerateHealthMachineEntityView.fusionShieldHealthChangeComponent.timeHealthChanging = 0f;
				}
			}
		}

		private bool ShouldTakeDamage(DegenerateHealthMachineEntityView playerMachine)
		{
			if (IsInsideEnemyShield(playerMachine))
			{
				FusionShieldEntityView fusionShieldEntityView = entityViewsDB.QueryEntityView<FusionShieldEntityView>(playerMachine.insideFusionShieldComponent.teamId);
				return fusionShieldEntityView.activableComponent.powerState;
			}
			return false;
		}

		private bool IsInsideEnemyShield(DegenerateHealthMachineEntityView playerMachine)
		{
			return playerMachine.insideFusionShieldComponent.isInsideShield && playerMachine.insideFusionShieldComponent.teamId != playerMachine.ownerTeamComponent.ownerTeamId;
		}

		private bool WasTakingDamageInsideShield(DegenerateHealthMachineEntityView playerMachine)
		{
			return playerMachine.fusionShieldHealthChangeComponent.isTakingDamage && playerMachine.insideFusionShieldComponent.teamId != playerMachine.ownerTeamComponent.ownerTeamId;
		}

		private void ResetData()
		{
			_proposedDamagedCubes.Clear();
			_damagedCubes.Clear();
		}

		private void ComputeDamagedCubes(DegenerateHealthMachineEntityView playerMachine)
		{
			IMachineMap machineMap = networkMachineManager.GetMachineMap(TargetType.Player, playerMachine.machineOwnerComponent.ownerMachineId);
			HashSet<InstantiatedCube> remainingCubes = machineMap.GetRemainingCubes();
			if (remainingCubes.Count > 0)
			{
				InstantiatedCube target = PickRandomCubeToDamage(remainingCubes);
				int damage = Mathf.CeilToInt((float)_damagePerSecond * playerMachine.fusionShieldHealthChangeComponent.timeHealthChanging);
				cubeDamagePropagator.ComputeProposedDamage(target, damage, 1f, ref _proposedDamagedCubes);
				if (_proposedDamagedCubes.Count > 0)
				{
					cubeDamagePropagator.GenerateDestructionGroupHitInfo(_proposedDamagedCubes, _damagedCubes);
				}
			}
		}

		private void SendDataToServer(int machineId)
		{
			if (_damagedCubes.Count > 0)
			{
				_damagedByEnemyShieldDependency.SetValues(machineId, battleTimer.SecondsSinceGameInitialised, _damagedCubes);
				_damagedCommand.Inject(_damagedByEnemyShieldDependency);
				_damagedCommand.Execute();
			}
		}

		private InstantiatedCube PickRandomCubeToDamage(HashSet<InstantiatedCube> remainingCubes)
		{
			HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator();
			int num = Random.Range(0, remainingCubes.Count);
			enumerator.MoveNext();
			for (int i = 0; i < num; i++)
			{
				enumerator.MoveNext();
			}
			return enumerator.Current;
		}

		public void Ready()
		{
		}
	}
}
