using Svelto.ECS;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation
{
	internal class HighSpeedColliderEngine : SingleEntityViewEngine<HighSpeedColliderNode>, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float STEP_DISTANCE_THRESHOLD = 1f;

		private HighSpeedColliderNode _localPlayerNode;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		public void PhysicsTick(float deltaSec)
		{
			if (_localPlayerNode != null)
			{
				InternalPhysicsTick(deltaSec);
			}
		}

		private void InternalPhysicsTick(float deltaSec)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = _localPlayerNode.rigidbodyComponent.rb;
			if (!(rb != null))
			{
				return;
			}
			Vector3 velocity = rb.get_velocity();
			float magnitude = velocity.get_magnitude();
			float num = magnitude * deltaSec;
			if (num > 1f)
			{
				if ((int)rb.get_collisionDetectionMode() != 1)
				{
					rb.set_collisionDetectionMode(1);
				}
			}
			else if ((int)rb.get_collisionDetectionMode() != 0)
			{
				rb.set_collisionDetectionMode(0);
			}
		}

		protected override void Add(HighSpeedColliderNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_localPlayerNode = node;
			}
		}

		protected override void Remove(HighSpeedColliderNode node)
		{
			if (_localPlayerNode == node)
			{
				_localPlayerNode = null;
			}
		}
	}
}
