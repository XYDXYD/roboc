using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Destruction
{
	internal class VisualDestructionEffects : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IInitialize
	{
		private const uint B1 = 715094163u;

		private Func<ParticleSystem> _onDeadCubeAllocation;

		private Func<ParticleSystem> _onDeadProtoniumAllocation;

		private Func<ParticleSystem> _onDeadProtoniumEAllocation;

		private Func<ParticleSystem> _onDeadProtoniumNAllocation;

		private Func<ParticleSystem> _onDebrisAllocation;

		private GameObject _deadCubePrefab;

		private GameObject _deadProtoniumPrefab;

		private GameObject _deadProtoniumPrefab_E;

		private GameObject _deadProtoniumPrefab_N;

		private GameObject _debrisPrefab;

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public ParticleSystemObjectPool pool
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

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
			destructionReporter.OnProtoniumDamageApplied += HandleOnProtoniumDamageApplied;
		}

		private void HandleOnProtoniumDamageApplied(DestructionData data)
		{
			if (data.destroyedCubes.get_Count() > 0)
			{
				DestroyCubes(data);
			}
		}

		private void HandleOnPlayerDamageApplied(DestructionData data)
		{
			if (data.destroyedCubes.get_Count() > 0 && !data.isDestroyed)
			{
				DestroyCubes(data);
			}
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
			destructionReporter.OnProtoniumDamageApplied -= HandleOnProtoniumDamageApplied;
		}

		private ParticleSystem AddRecycleOnDisableForParticles(GameObject prefab)
		{
			return pool.AddRecycleOnDisableForParticles<ParticleSystem, AutoRecycleParticleSystemBehaviour>(prefab, isPrefab: true, startDisabled: true);
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			_onDeadCubeAllocation = (() => AddRecycleOnDisableForParticles(_deadCubePrefab));
			_onDeadProtoniumAllocation = (() => AddRecycleOnDisableForParticles(_deadProtoniumPrefab));
			_onDeadProtoniumEAllocation = (() => AddRecycleOnDisableForParticles(_deadProtoniumPrefab_E));
			_onDeadProtoniumNAllocation = (() => AddRecycleOnDisableForParticles(_deadProtoniumPrefab_N));
			_onDebrisAllocation = (() => AddRecycleOnDisableForParticles(_debrisPrefab));
			pool.Preallocate(_deadCubePrefab.get_name(), 50, _onDeadCubeAllocation);
			pool.Preallocate(_deadProtoniumPrefab.get_name(), 20, _onDeadProtoniumAllocation);
			pool.Preallocate(_deadProtoniumPrefab_E.get_name(), 20, _onDeadProtoniumEAllocation);
			pool.Preallocate(_deadProtoniumPrefab_N.get_name(), 20, _onDeadProtoniumNAllocation);
			pool.Preallocate(_debrisPrefab.get_name(), 50, _onDebrisAllocation);
		}

		public void SetEffectData(DeadCubeEffectData effectData)
		{
			_deadCubePrefab = effectData.deadCubePrefabPlayer;
			_deadProtoniumPrefab = effectData.protoniumDestroy;
			_deadProtoniumPrefab_E = effectData.protoniumDestroy_E;
			_deadProtoniumPrefab_N = effectData.protoniumDestroy_N;
			_debrisPrefab = effectData.debrisPrefabPlayer;
		}

		private void PlayDeadCubeParticleEffect(FasterList<InstantiatedCube> deadCubes, Rigidbody rb, TargetType targetType, int targetPlayerId)
		{
			int playerTeam = playerTeamsContainer.GetPlayerTeam(targetType, targetPlayerId);
			int playerTeam2 = playerTeamsContainer.GetPlayerTeam(TargetType.Player, playerTeamsContainer.localPlayerId);
			int count = deadCubes.get_Count();
			if (count == 0)
			{
				return;
			}
			bool flag = false;
			int num;
			if (count >= 50)
			{
				num = ((count < 200) ? 20 : ((count >= 1000) ? (count / 10) : 100));
			}
			else
			{
				num = 10;
				flag = true;
			}
			int i = 0;
			if (flag)
			{
				InstantiatedCube instantiatedCube = null;
				for (int j = 0; j < deadCubes.get_Count(); j++)
				{
					InstantiatedCube instantiatedCube2 = deadCubes.get_Item(j);
					if (targetType != TargetType.TeamBase && instantiatedCube2.persistentCubeData.category != CubeCategory.Chassis)
					{
						ActuallyPlayDebrisParticleEffect(instantiatedCube2, rb, targetType);
						continue;
					}
					if (instantiatedCube == null)
					{
						instantiatedCube = instantiatedCube2;
					}
					i++;
					if (i == num)
					{
						if (targetType == TargetType.TeamBase)
						{
							PlayDeadProtoniumParticleEffect(instantiatedCube2, rb, targetType, playerTeam2, playerTeam);
						}
						else
						{
							ActuallyPlayDeadCubeParticleEffect(instantiatedCube, num, rb, targetType, playerTeam2, playerTeam);
						}
						instantiatedCube = null;
						i = 0;
					}
				}
				if (instantiatedCube != null)
				{
					if (targetType == TargetType.TeamBase)
					{
						PlayDeadProtoniumParticleEffect(instantiatedCube, rb, targetType, playerTeam2, playerTeam);
					}
					else
					{
						ActuallyPlayDeadCubeParticleEffect(instantiatedCube, i, rb, targetType, playerTeam2, playerTeam);
					}
				}
				return;
			}
			for (; i < count; i += num)
			{
				int numCubesDestroyed = Mathf.Min(num, count - i);
				if (targetType == TargetType.TeamBase)
				{
					PlayDeadProtoniumParticleEffect(deadCubes.get_Item(i), rb, targetType, playerTeam2, playerTeam);
				}
				else
				{
					ActuallyPlayDeadCubeParticleEffect(deadCubes.get_Item(i), numCubesDestroyed, rb, targetType, playerTeam2, playerTeam);
				}
			}
		}

		private void DestroyCubes(DestructionData data)
		{
			PlayDeadCubeParticleEffect(data.destroyedCubes, data.hitRigidbody, data.targetType, data.hitPlayerId);
		}

		private void ActuallyPlayDeadCubeParticleEffect(InstantiatedCube deadCube, int numCubesDestroyed, Rigidbody rb, TargetType targetType, int myTeam, int targetTeam)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			ParticleSystem deadCubePrefab = GetDeadCubePrefab(targetType, myTeam, targetTeam);
			deadCubePrefab.get_gameObject().SetActive(true);
			Transform transform = deadCubePrefab.get_transform();
			transform.set_position(GridScaleUtility.GetCubeWorldPosition(rb.get_transform(), deadCube.gridPos, targetType));
			transform.set_localScale(Vector3.get_one() * (FastCubeRoot(numCubesDestroyed) * 0.6666f));
			MainModule main = deadCubePrefab.get_main();
			main.set_startColor(MinMaxGradient.op_Implicit(deadCube.colour));
			deadCubePrefab.Emit(numCubesDestroyed);
		}

		private void PlayDeadProtoniumParticleEffect(InstantiatedCube deadCube, Rigidbody rb, TargetType targetType, int myTeam, int targetTeam)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = rb.get_transform();
			ParticleSystem deadCubePrefab = GetDeadCubePrefab(targetType, myTeam, targetTeam);
			GameObject gameObject = deadCubePrefab.get_gameObject();
			gameObject.SetActive(true);
			Transform transform2 = gameObject.get_transform();
			transform2.set_position(GridScaleUtility.GetCubeWorldPosition(transform, deadCube.gridPos, targetType));
		}

		private ParticleSystem GetDeadCubePrefab(TargetType targetType, int myTeam, int targetTeam)
		{
			switch (targetType)
			{
			case TargetType.Player:
				return pool.Use(_deadCubePrefab.get_name(), _onDeadCubeAllocation);
			case TargetType.TeamBase:
				if (targetTeam == -1)
				{
					return pool.Use(_deadProtoniumPrefab_N.get_name(), _onDeadProtoniumNAllocation);
				}
				if (targetTeam != myTeam)
				{
					return pool.Use(_deadProtoniumPrefab_E.get_name(), _onDeadProtoniumEAllocation);
				}
				return pool.Use(_deadProtoniumPrefab.get_name(), _onDeadProtoniumAllocation);
			default:
				return null;
			}
		}

		private void ActuallyPlayDebrisParticleEffect(InstantiatedCube deadCube, Rigidbody rb, TargetType targetType)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = rb.get_transform();
			ParticleSystem val = pool.Use(_debrisPrefab.get_name(), _onDebrisAllocation);
			val.get_gameObject().SetActive(true);
			Transform transform2 = val.get_transform();
			transform2.set_position(transform.get_position() + transform.get_rotation() * GridScaleUtility.GridToWorld(deadCube.gridPos, targetType));
		}

		private static float FastCubeRoot(float param)
		{
			return param / 715094144f;
		}
	}
}
