using BehaviorDesigner.Runtime.Tasks;
using Simulation.Hardware.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class ShootSelectedTarget : Action
	{
		public SharedTargetInfoComponent selectedTargetInfo;

		public SharedAIBehaviorTreeAgentNode agent;

		public SharedAIWeaponRaycast aiWeaponRaycastShared;

		private const float INACCURACY_MAX_DISTANCE = 2f;

		private const float INACCURACY_AIM_VELOCITY = 1f;

		private Vector3 _targetDir = Vector3.get_forward();

		private float _time;

		private Vector3 _accuracyOffsetTarget;

		private Vector3 _accuracyOffsetCurrent;

		private AIAgentDataComponentsNode _agent;

		private TargetInfo _targetInfo;

		private AIWeaponRaycast _aiWeaponRaycast;

		private Byte3? _targetCubePos;

		public ShootSelectedTarget()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)


		public override void OnStart()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			_agent = agent.get_Value();
			_targetInfo = selectedTargetInfo.get_Value();
			_aiWeaponRaycast = aiWeaponRaycastShared.get_Value();
			_time = 0f;
			_accuracyOffsetTarget = Vector3.get_zero();
			_targetDir = GetTargetPosition() - GetAimStartPosition();
			_targetDir.Normalize();
			this.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			_targetInfo = selectedTargetInfo.get_Value();
			_time += Time.get_fixedDeltaTime();
			Vector3 val = _accuracyOffsetCurrent - _accuracyOffsetTarget;
			if (val.get_sqrMagnitude() < float.Epsilon)
			{
				_accuracyOffsetTarget = GenerateLinearInaccuracy(2f);
			}
			_accuracyOffsetCurrent = Vector3.MoveTowards(_accuracyOffsetCurrent, _accuracyOffsetTarget, 1f * Time.get_fixedDeltaTime());
			Vector3 aimStartPosition = GetAimStartPosition();
			Vector3 targetPosition = GetTargetPosition();
			Vector3 val2 = targetPosition + _accuracyOffsetCurrent;
			Vector3 targetVector = val2 - aimStartPosition;
			_targetDir = targetVector.get_normalized();
			if (IsInRange(targetVector))
			{
				StartShootingIfEnoughPower();
			}
			else
			{
				StopShooting();
			}
			_aiWeaponRaycast.aimPoint = val2;
			_aiWeaponRaycast.targetPoint = val2;
			return 3;
		}

		private bool IsInRange(Vector3 targetVector)
		{
			float maxFiringRange = AIMovementUtils.GetMaxFiringRange(_agent.aiEquippedWeaponComponent.itemCategory);
			return targetVector.get_sqrMagnitude() < maxFiringRange * maxFiringRange;
		}

		private Vector3 GetTargetPosition()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			Byte3? targetCubePos = _targetCubePos;
			if (targetCubePos.HasValue)
			{
				MachineCell cellAt = _targetInfo.machineMap.GetCellAt(_targetCubePos.Value);
				if (cellAt == null || cellAt.info.isDestroyed)
				{
					_targetCubePos = null;
				}
			}
			Byte3? targetCubePos2 = _targetCubePos;
			if (!targetCubePos2.HasValue)
			{
				HashSet<InstantiatedCube> remainingCubes = _targetInfo.machineMap.GetRemainingCubes();
				InstantiatedCube instantiatedCube = ChooseTargetCube(remainingCubes);
				_targetCubePos = instantiatedCube.gridPos;
			}
			return GridScaleUtility.GetCubeWorldPosition(_targetCubePos.Value, _targetInfo.rigidBody, TargetType.Player);
		}

		private static Vector3 GenerateLinearInaccuracy(float max)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(Random.Range(0f - max, max), Random.Range(0f - max, max), Random.Range(0f - max, max));
		}

		private InstantiatedCube ChooseTargetCube(HashSet<InstantiatedCube> remainingCubes)
		{
			int num = Random.Range(0, remainingCubes.Count - 1);
			int num2 = 0;
			using (HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (num2 == num)
					{
						return enumerator.Current;
					}
					num2++;
				}
			}
			throw new Exception("Failed to find a cube to target");
		}

		public override void OnEnd()
		{
			StopShooting();
			this.OnEnd();
		}

		private void StopShooting()
		{
			_agent.aiInputWrapper.fire1 = 0f;
		}

		private void StartShootingIfEnoughPower()
		{
			if (_agent.aiPowerConsumptionComponent.power < _agent.aiPowerConsumptionComponent.currentWeaponPowerConsumption)
			{
				_agent.aiInputWrapper.fire1 = 0f;
			}
			else
			{
				_agent.aiInputWrapper.fire1 = 1f;
			}
		}

		private Vector3 GetAimStartPosition()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = (_agent.aiMovementData.minmax[0] + _agent.aiMovementData.minmax[1]) * 0.5f;
			return _agent.aiMovementData.rigidBody.get_transform().TransformPoint(val);
		}
	}
}
