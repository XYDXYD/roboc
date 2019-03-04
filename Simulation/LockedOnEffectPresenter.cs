using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class LockedOnEffectPresenter : IInitialize, ITickable, ITickableBase
	{
		private readonly Func<GameObject> _onLockOnArrowsAllocation;

		private Dictionary<int, GameObject> _rocketOwnerToArrows;

		private Dictionary<int, Rigidbody> _rocketOwnerToRigidBody;

		private FasterList<int> _arrowsForRemoval;

		private GameObject _lockedOnArrows;

		private int _maximumLockWarnings;

		private float _upArrowLimit = 0.9f;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachines
		{
			get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyContainer
		{
			get;
			set;
		}

		public LockedOnEffectPresenter()
		{
			_onLockOnArrowsAllocation = OnLockOnArrowsAllocation;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_rocketOwnerToArrows = new Dictionary<int, GameObject>();
			_rocketOwnerToRigidBody = new Dictionary<int, Rigidbody>();
		}

		internal void RegisterData(GameObject lockArrowPrefab, int maxLockWarnings)
		{
			_lockedOnArrows = lockArrowPrefab;
			_maximumLockWarnings = maxLockWarnings;
			_arrowsForRemoval = new FasterList<int>(_maximumLockWarnings);
			PreallocateEffectPools();
		}

		public void ApplyLockOnEffectFromPlayer(Dictionary<int, int> opponentsWithLocksOnPlayer)
		{
			RemoveInactiveLockOnArrows(opponentsWithLocksOnPlayer);
			if (_rocketOwnerToArrows.Count == _maximumLockWarnings)
			{
				return;
			}
			Dictionary<int, int>.KeyCollection.Enumerator enumerator = opponentsWithLocksOnPlayer.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				if (!_rocketOwnerToArrows.ContainsKey(current))
				{
					_rocketOwnerToArrows[current] = PlayEffect(current);
				}
			}
		}

		private GameObject OnLockOnArrowsAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForGameObject(_lockedOnArrows);
		}

		public void Tick(float deltaSec)
		{
			if (_rocketOwnerToArrows.Count > 0)
			{
				UpdateEffects();
			}
		}

		private void RemoveInactiveLockOnArrows(Dictionary<int, int> opponentsWithLocksOnPlayer)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			_arrowsForRemoval.FastClear();
			Dictionary<int, GameObject>.KeyCollection.Enumerator enumerator = _rocketOwnerToArrows.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				if (!opponentsWithLocksOnPlayer.ContainsKey(current))
				{
					_arrowsForRemoval.Add(current);
				}
			}
			FasterListEnumerator<int> enumerator2 = _arrowsForRemoval.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				int current2 = enumerator2.get_Current();
				GameObject val = _rocketOwnerToArrows[current2];
				val.SetActive(false);
				_rocketOwnerToArrows.Remove(current2);
				_rocketOwnerToRigidBody.Remove(current2);
			}
		}

		private void PreallocateEffectPools()
		{
			gameObjectPool.Preallocate(_lockedOnArrows.get_name(), _maximumLockWarnings, _onLockOnArrowsAllocation);
		}

		private GameObject PlayEffect(int lockingPlayer)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectPool.Use(_lockedOnArrows.get_name(), _onLockOnArrowsAllocation);
			int activeMachine = playerMachines.GetActiveMachine(TargetType.Player, playerTeamsContainer.localPlayerId);
			Rigidbody rigidBodyData = rigidbodyContainer.GetRigidBodyData(TargetType.Player, activeMachine);
			val.get_transform().set_parent(rigidBodyData.get_transform());
			val.get_transform().set_localPosition(rigidBodyData.get_centerOfMass());
			int activeMachine2 = playerMachines.GetActiveMachine(TargetType.Player, lockingPlayer);
			_rocketOwnerToRigidBody[lockingPlayer] = rigidbodyContainer.GetRigidBodyData(TargetType.Player, activeMachine2);
			Vector3 worldCenterOfMass = _rocketOwnerToRigidBody[lockingPlayer].get_worldCenterOfMass();
			val.get_transform().LookAt(worldCenterOfMass, Vector3.get_up());
			val.SetActive(true);
			return val;
		}

		private void UpdateEffects()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<int, GameObject>.Enumerator enumerator = _rocketOwnerToArrows.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GameObject val = _rocketOwnerToArrows[enumerator.Current.Key];
				Vector3 worldCenterOfMass = _rocketOwnerToRigidBody[enumerator.Current.Key].get_worldCenterOfMass();
				val.get_transform().LookAt(worldCenterOfMass, Camera.get_main().get_transform().get_right());
				float num = Vector3.Dot(val.get_transform().get_forward(), Camera.get_main().get_transform().get_right());
				float num2 = Mathf.Abs(num);
				if (num2 > _upArrowLimit)
				{
					val.get_transform().LookAt(worldCenterOfMass, Camera.get_main().get_transform().get_up());
				}
			}
		}
	}
}
