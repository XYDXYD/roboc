using Simulation.Hardware.Movement.Thruster;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal sealed class CameraRelativeTurnDampingEngine : SingleEntityViewEngine<CameraRelativeTurnDampingNode>, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float ROTATION_SLOWING_THRESHOLD_MULTIPLIER = 2f;

		private const float STOPPING_DAMPING_SCALE = 0.8f;

		private const float SLOWING_DAMPING_SCALE = 0.5f;

		private Rigidbody _localPlayeRigidbody;

		private int _localPlayerMachineId;

		private Dictionary<int, CameraRelativeTurnDampingNode> _turningNodes = new Dictionary<int, CameraRelativeTurnDampingNode>();

		[Inject]
		internal PlayerStrafeDirectionManager strafeDirectionManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(CameraRelativeTurnDampingNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				return;
			}
			if (_turningNodes.Count == 0)
			{
				_localPlayeRigidbody = node.rigidBodyComponent.rb;
				_localPlayerMachineId = node.ownerComponent.machineId;
			}
			ThrusterNode thrusterNode = default(ThrusterNode);
			if (entityViewsDB.TryQueryEntityView<ThrusterNode>(node.get_ID(), ref thrusterNode))
			{
				CubeFace legacyDirection = thrusterNode.facingComponent.legacyDirection;
				if (legacyDirection == CubeFace.Left || legacyDirection == CubeFace.Right)
				{
					node.disabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)OnMovementPartDestroyed);
					if (!node.disabledComponent.isPartDisabled.get_value())
					{
						_turningNodes.Add(node.get_ID(), node);
					}
				}
			}
			else
			{
				node.disabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)OnMovementPartDestroyed);
				if (!node.disabledComponent.isPartDisabled.get_value())
				{
					_turningNodes.Add(node.get_ID(), node);
				}
			}
		}

		protected override void Remove(CameraRelativeTurnDampingNode node)
		{
			if (node.ownerComponent.ownedByMe && _turningNodes.ContainsKey(node.get_ID()))
			{
				_turningNodes.Remove(node.get_ID());
				node.disabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnMovementPartDestroyed);
			}
		}

		private void OnMovementPartDestroyed(int id, bool value)
		{
			CameraRelativeTurnDampingNode value2 = default(CameraRelativeTurnDampingNode);
			if (value)
			{
				_turningNodes.Remove(id);
			}
			else if (entityViewsDB.TryQueryEntityView<CameraRelativeTurnDampingNode>(id, ref value2))
			{
				_turningNodes.Add(id, value2);
			}
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			if (_localPlayeRigidbody != null && strafeDirectionManager.strafingEnabled && _turningNodes.Count > 0)
			{
				float num = 0f;
				StrafingCustomAngleToStraightNode strafingCustomAngleToStraightNode = default(StrafingCustomAngleToStraightNode);
				num = ((!entityViewsDB.TryQueryEntityView<StrafingCustomAngleToStraightNode>(_localPlayerMachineId, ref strafingCustomAngleToStraightNode)) ? CalculateDampingScale() : ((!strafingCustomAngleToStraightNode.customAngleToStraightComponent.customAngleUsed) ? CalculateDampingScale() : CalculateDampingScale(strafingCustomAngleToStraightNode.customAngleToStraightComponent.angleToStraight)));
				if (!strafeDirectionManager.IsAngleToPrevCamPosGreaterThanThreshold())
				{
					float num2 = Vector3.Dot(_localPlayeRigidbody.get_angularVelocity(), _localPlayeRigidbody.get_transform().get_up());
					Vector3 val = _localPlayeRigidbody.get_transform().get_up() * num2 * num;
					_localPlayeRigidbody.AddTorque(val, 2);
				}
			}
		}

		private float CalculateDampingScale()
		{
			float result = 0f;
			if (!strafeDirectionManager.IsAngleToStraightGreaterThanThreshold())
			{
				result = -0.8f;
			}
			else if (!strafeDirectionManager.IsAngleToStraightGreaterThanThreshold(2f))
			{
				result = -0.5f;
			}
			return result;
		}

		private float CalculateDampingScale(float angleToStraight)
		{
			float result = 0f;
			if (!strafeDirectionManager.IsAngleGreaterThanThreshold(angleToStraight))
			{
				result = -0.8f;
			}
			else if (!strafeDirectionManager.IsAngleGreaterThanThreshold(angleToStraight, 2f))
			{
				result = -0.5f;
			}
			return result;
		}
	}
}
