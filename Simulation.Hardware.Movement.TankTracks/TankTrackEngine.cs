using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackEngine : IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private WheelHit _hit = default(WheelHit);

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void PhysicsTick(float deltaTime)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<TankTrackMachineNode> val = entityViewsDB.QueryEntityViews<TankTrackMachineNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				int count = default(int);
				TankTrackNode[] tracks = entityViewsDB.QueryGroupedEntityViewsAsArray<TankTrackNode>(val.get_Item(i).get_ID(), ref count);
				Update(tracks, count);
			}
		}

		private void Update(TankTrackNode[] tracks, int count)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < count; i++)
			{
				TankTrackNode tankTrackNode = tracks[i];
				if (!tankTrackNode.hardwareDisabledComponent.disabled)
				{
					CalcuateForcePointAndDistanceToCoM(tankTrackNode);
					UpdateGrounded(tankTrackNode);
					CalculateSlopeScalar(tankTrackNode);
					HandleTrackIsStopped(tankTrackNode);
					ApplyForces(tankTrackNode);
				}
				else
				{
					ITrackGroundedComponent trackGroundedComponent = tankTrackNode.trackGroundedComponent;
					trackGroundedComponent.grounded = false;
					trackGroundedComponent.groundedWheelCount = 0;
				}
				tankTrackNode.pendingForceComponent.pendingForce = Vector3.get_zero();
				tankTrackNode.pendingForceComponent.pendingVelocityChangeForce = Vector3.get_zero();
			}
		}

		private void HandleTrackIsStopped(TankTrackNode node)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			float num = node.frictionStiffnessComponent.stoppedFrictionStiffness;
			float motorTorque = 0f;
			if (!node.trackStoppedComponent.stopped && !node.turningToDriveDirection.turning)
			{
				Vector3 velocity = node.rigidbodyComponent.rb.get_velocity();
				float num2 = velocity.get_magnitude() / (node.maxSpeedComponent.maxSpeed * 0.35f);
				num2 = Mathf.Clamp01(num2);
				float num3 = node.frictionStiffnessComponent.movingFrictionStiffness - node.frictionStiffnessComponent.stoppedFrictionStiffness;
				num3 *= num2;
				num += num3;
				motorTorque = 1E-06f;
			}
			FasterList<WheelColliderData> wheelColliders = node.wheelColliderDataComponent.wheelColliders;
			for (int i = 0; i < wheelColliders.get_Count(); i++)
			{
				WheelCollider wheelCollider = wheelColliders.get_Item(i).wheelCollider;
				if (wheelCollider != null)
				{
					WheelFrictionCurve sidewaysFriction = wheelCollider.get_sidewaysFriction();
					sidewaysFriction.set_stiffness(num);
					wheelCollider.set_sidewaysFriction(sidewaysFriction);
					wheelCollider.set_motorTorque(motorTorque);
				}
			}
		}

		private void CalcuateForcePointAndDistanceToCoM(TankTrackNode node)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			Vector3 worldCenterOfMass = node.rigidbodyComponent.rb.get_worldCenterOfMass();
			Vector3 position = node.forcePointComponent.forcePointTransform.get_position();
			Vector3 val = worldCenterOfMass - position;
			CalculateForcePoint(node, val);
			float num = val.get_sqrMagnitude() - node.distanceToCOMComponent.distance * node.distanceToCOMComponent.distance;
			if (Mathf.Abs(num) > 0.05f)
			{
				CalculateDistanceToCOM(node, val);
			}
		}

		private void UpdateGrounded(TankTrackNode node)
		{
			FasterList<WheelColliderData> wheelColliders = node.wheelColliderDataComponent.wheelColliders;
			bool grounded = false;
			int num = 0;
			for (int i = 0; i < wheelColliders.get_Count(); i++)
			{
				WheelCollider wheelCollider = wheelColliders.get_Item(i).wheelCollider;
				if (wheelCollider != null && wheelCollider.get_isGrounded())
				{
					grounded = true;
					num++;
				}
			}
			ITrackGroundedComponent trackGroundedComponent = node.trackGroundedComponent;
			trackGroundedComponent.grounded = grounded;
			trackGroundedComponent.groundedWheelCount = num;
		}

		private void CalculateSlopeScalar(TankTrackNode node)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			FasterList<WheelColliderData> wheelColliders = node.wheelColliderDataComponent.wheelColliders;
			Vector3 val = Vector3.get_zero();
			int groundedWheelCount = node.trackGroundedComponent.groundedWheelCount;
			for (int i = 0; i < wheelColliders.get_Count(); i++)
			{
				WheelCollider wheelCollider = wheelColliders.get_Item(i).wheelCollider;
				if (wheelCollider != null && wheelCollider.GetGroundHit(ref _hit) && _hit.get_collider() != null)
				{
					val += _hit.get_normal();
				}
			}
			val /= (float)groundedWheelCount;
			float currentSlopeScalar = 1f;
			IFrictionAngleComponent frictionAngleComponent = node.frictionAngleComponent;
			float num = Vector3.Angle(val, Vector3.get_up());
			if (num >= node.frictionAngleComponent.angleWithMinFriction)
			{
				currentSlopeScalar = frictionAngleComponent.minFrictionScalar;
			}
			else if (num > node.frictionAngleComponent.angleWithMaxFriction)
			{
				float angleWithMaxFriction = frictionAngleComponent.angleWithMaxFriction;
				float num2 = (num - angleWithMaxFriction) / (frictionAngleComponent.angleWithMinFriction - angleWithMaxFriction);
				currentSlopeScalar = num2 * frictionAngleComponent.minFrictionScalar + (1f - num2) * 1f;
			}
			node.currentSlopeScalarComponent.currentSlopeScalar = currentSlopeScalar;
		}

		private void CalculateForcePoint(TankTrackNode node, Vector3 vectorToCoM)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forcePointOffset = default(Vector3);
			forcePointOffset._002Ector(0f, vectorToCoM.y, 0f);
			node.forcePointComponent.forcePointOffset = forcePointOffset;
			node.forcePointComponent.forcePoint = node.forcePointComponent.forcePointTransform.get_position() + node.forcePointComponent.forcePointOffset;
		}

		private void CalculateDistanceToCOM(TankTrackNode node, Vector3 vectorToCom)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(vectorToCom, node.rigidbodyComponent.rb.get_transform().get_right());
			float magnitude = vectorToCom.get_magnitude();
			if ((double)magnitude > 0.2)
			{
				if (num > 0.01f)
				{
					node.machineSideComponent.machineSide = MachineSide.left;
				}
				else if (num < -0.01f)
				{
					node.machineSideComponent.machineSide = MachineSide.right;
				}
				else
				{
					node.machineSideComponent.machineSide = MachineSide.center;
				}
			}
			else
			{
				node.machineSideComponent.machineSide = MachineSide.center;
			}
			node.distanceToCOMComponent.distance = magnitude;
		}

		private void ApplyForces(TankTrackNode node)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = node.pendingForceComponent.pendingForce * node.currentSlopeScalarComponent.currentSlopeScalar;
			Vector3 val2 = node.pendingForceComponent.pendingVelocityChangeForce * node.currentSlopeScalarComponent.currentSlopeScalar;
			Rigidbody rb = node.rigidbodyComponent.rb;
			if (node.trackGroundedComponent.groundedWheelCount <= 0)
			{
				return;
			}
			if (val.get_sqrMagnitude() > 0.01f)
			{
				Vector3 velocity = rb.get_velocity();
				if (velocity.get_sqrMagnitude() < 0.01f)
				{
					FasterList<WheelColliderData> wheelColliders = node.wheelColliderDataComponent.wheelColliders;
					for (int i = 0; i < wheelColliders.get_Count(); i++)
					{
						WheelCollider wheelCollider = wheelColliders.get_Item(i).wheelCollider;
						if (wheelCollider != null)
						{
							wheelCollider.set_brakeTorque(0f);
							wheelCollider.set_motorTorque(1E-06f);
						}
					}
				}
				if (node.turningToDriveDirection.turning)
				{
					val *= 1.2f;
				}
				node.rigidbodyComponent.rb.AddForceAtPosition(val, node.forcePointComponent.forcePoint);
			}
			node.rigidbodyComponent.rb.AddForce(val2, 2);
		}
	}
}
