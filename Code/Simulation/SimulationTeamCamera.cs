using Simulation.DeathEffects;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class SimulationTeamCamera : SimulationCamera
	{
		public float timeToViewKiller = 1f;

		public float viewSpaceTargetTolerance = 0.1f;

		private int _currentPlayerId;

		private int _currentPlayerIdInLivePlayers;

		private float _viewKillerTime;

		private bool _viewedKiller;

		private int _killerId;

		private bool _leftMouseWasDown;

		private bool _rightMouseWasDown;

		private readonly Dictionary<int, MachineInfo> _machineInfoPerPlayer = new Dictionary<int, MachineInfo>();

		[Inject]
		internal LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher machineSpawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal DeathAnimationFinishedObserver deathAnimationFinishedObserver
		{
			private get;
			set;
		}

		public override void OnDependenciesInjected()
		{
			base.OnDependenciesInjected();
			machineSpawnDispatcher.OnPlayerRegistered += MachineSpawnDispatcher_OnPlayerRegistered;
		}

		private void MachineSpawnDispatcher_OnPlayerRegistered(SpawnInParametersPlayer obj)
		{
			int machineId = obj.machineId;
			_machineInfoPerPlayer[machineId] = obj.preloadedMachine.machineInfo;
		}

		protected unsafe override void Start()
		{
			base.Start();
			SetMachine(_killerId);
			deathAnimationFinishedObserver.AddAction(new ObserverAction<Kill>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Update()
		{
			base.Update();
			if (!_viewedKiller)
			{
				UpdateViewAttacker();
			}
		}

		public override void HandleCharacterInput(InputCharacterData data)
		{
			if (this.get_enabled())
			{
				base.HandleCharacterInput(data);
				if (_viewedKiller)
				{
					HandleCyclePlayer(data);
				}
			}
		}

		internal void SetKiller(int myKiller)
		{
			_killerId = myKiller;
			_viewedKiller = false;
			_viewKillerTime = 0f;
			SetMachine(_killerId);
		}

		private void OnMachineDestroyed(ref Kill kill)
		{
			if (kill.victimId == _currentPlayerId)
			{
				CyclePlayer(1);
			}
		}

		private void HandleCyclePlayer(InputCharacterData data)
		{
			int num = 0;
			if (data.data[6] > 0f)
			{
				_leftMouseWasDown = true;
			}
			else
			{
				if (_leftMouseWasDown)
				{
					num++;
				}
				_leftMouseWasDown = false;
			}
			if (data.data[7] > 0f)
			{
				_rightMouseWasDown = true;
			}
			else
			{
				if (_rightMouseWasDown)
				{
					num--;
				}
				_rightMouseWasDown = false;
			}
			if (num != 0)
			{
				CyclePlayer(num);
			}
		}

		private bool IsViewingAttacker()
		{
			if (!livePlayersContainer.IsPlayerAlive(TargetType.Player, _killerId))
			{
				return true;
			}
			if (_TrackingCube == null)
			{
				SetMachine(_killerId);
			}
			return IsLookingAtTarget();
		}

		private bool IsLookingAtTarget()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Camera.get_main().WorldToViewportPoint(base.cameraPivotPoint);
			val.z = 0f;
			Vector3 val2 = default(Vector3);
			val2._002Ector(0.5f, 0.5f, 0f);
			Vector3 val3 = val2 - val;
			return val3.get_sqrMagnitude() < viewSpaceTargetTolerance * viewSpaceTargetTolerance;
		}

		private void UpdateViewAttacker()
		{
			if (IsViewingAttacker())
			{
				_viewKillerTime += Time.get_deltaTime();
				if (_viewKillerTime >= timeToViewKiller)
				{
					_viewedKiller = true;
					ViewFirstPlayer();
				}
			}
		}

		private void ViewFirstPlayer()
		{
			_currentPlayerId = -1;
			CyclePlayer(1);
		}

		private void CyclePlayer(int offset)
		{
			IList<int> livePlayers = livePlayersContainer.GetLivePlayers(TargetType.Player);
			bool flag = false;
			for (int i = 0; i < livePlayers.Count; i++)
			{
				int num = (_currentPlayerIdInLivePlayers + offset * (i + 1) + livePlayers.Count) % livePlayers.Count;
				int num2 = livePlayers[num];
				if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, num2))
				{
					_currentPlayerIdInLivePlayers = num;
					_currentPlayerId = num2;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				SetMachine(_currentPlayerId);
			}
		}

		private void SetMachine(int playerId)
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
			MachineInfo machineInfo = _machineInfoPerPlayer[activeMachine];
			Transform cameraPivotTransform = machineInfo.cameraPivotTransform;
			if (cameraPivotTransform != null)
			{
				_TrackingCube = cameraPivotTransform;
				ComputeOffsets(machineInfo);
			}
		}
	}
}
