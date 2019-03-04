using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackGraphicsEngine : ITickable, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private RaycastHit _raycastHit;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<TankTrackGraphicsNode> val = entityViewsDB.QueryEntityViews<TankTrackGraphicsNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				TankTrackGraphicsNode tankTrackGraphicsNode = val.get_Item(i);
				if (!tankTrackGraphicsNode.visibilityComponent.offScreen && tankTrackGraphicsNode.disabledComponent.enabled)
				{
					TrackSuspensionItem[] suspensionItems = tankTrackGraphicsNode.suspensionItemsComponent.suspensionItems;
					for (int j = 0; j < suspensionItems.Length; j++)
					{
						UpdateSuspension(tankTrackGraphicsNode, suspensionItems[j]);
					}
					Transform t = tankTrackGraphicsNode.transformComponent.T;
					Vector3 position = t.get_position();
					Vector3 val2 = position - tankTrackGraphicsNode.previousPosComponent.previousPos;
					float num = Mathf.Sign(Vector3.Dot(val2, tankTrackGraphicsNode.machineRootComponent.rb.get_transform().get_forward()));
					tankTrackGraphicsNode.trackSpeedComponent.trackSpeed = num * val2.get_magnitude() / deltaSec;
					tankTrackGraphicsNode.previousPosComponent.previousPos = position;
					UpdateTrackScrolling(tankTrackGraphicsNode);
				}
			}
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<TankTrackGraphicsNode> val = entityViewsDB.QueryEntityViews<TankTrackGraphicsNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				TankTrackGraphicsNode tankTrackGraphicsNode = val.get_Item(i);
				if (tankTrackGraphicsNode.visibilityComponent.offScreen || !tankTrackGraphicsNode.disabledComponent.enabled)
				{
					continue;
				}
				WheelSpinItem[] spinningItems = tankTrackGraphicsNode.spinningItemsComponent.spinningItems;
				for (int j = 0; j < spinningItems.Length; j++)
				{
					UpdateWheelSpinning(tankTrackGraphicsNode, spinningItems[j]);
				}
				TrackSuspensionItem[] suspensionItems = tankTrackGraphicsNode.suspensionItemsComponent.suspensionItems;
				for (int k = 0; k < suspensionItems.Length; k++)
				{
					SuspensionLookAt lookAt = suspensionItems[k].lookAt;
					if (lookAt != null)
					{
						lookAt.UpdateRotation();
					}
				}
			}
		}

		private void UpdateSuspension(TankTrackGraphicsNode node, TrackSuspensionItem suspension)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			float num = suspension.raycastDist;
			if (Physics.Raycast(suspension.raycastingObj.get_position(), -suspension.raycastingObj.get_up(), ref _raycastHit, suspension.raycastDist, node.raycastLayerComponent.raycastLayer))
			{
				num = _raycastHit.get_distance();
			}
			if (suspension.transform != null)
			{
				Vector3 position = suspension.raycastingObj.get_position();
				Vector3 val = -node.transformComponent.T.get_right();
				Vector3 position2 = position + val * num;
				suspension.transform.set_position(position2);
			}
		}

		private void UpdateTrackScrolling(TankTrackGraphicsNode node)
		{
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			TrackScrollItem trackScroll = node.trackScrollItemComponent.trackScroll;
			CalculateRPM(node);
			float rpm = node.rpmComponent.rpm;
			float num = rpm;
			if (trackScroll.numberFramesToAvg > 0)
			{
				trackScroll.pastRPM.Enqueue(rpm);
				while (trackScroll.pastRPM.Count > trackScroll.numberFramesToAvg)
				{
					trackScroll.pastRPM.Dequeue();
				}
				num = 0f;
				Queue<float>.Enumerator enumerator = trackScroll.pastRPM.GetEnumerator();
				while (enumerator.MoveNext())
				{
					num += enumerator.Current;
				}
				num /= (float)trackScroll.pastRPM.Count;
			}
			trackScroll.scrollAmount += num * trackScroll.scrollScale * node.wheelScrollScaleComponent.wheelScrollScale * Time.get_deltaTime() / 60f;
			trackScroll.scrollingMaterial.set_mainTextureOffset(new Vector2(0f, trackScroll.scrollAmount));
		}

		private void UpdateWheelSpinning(TankTrackGraphicsNode node, WheelSpinItem spinItem)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			float rPM = GetRPM(spinItem, node.trackSpeedComponent.trackSpeed);
			rPM *= node.trackScrollItemComponent.trackScroll.scrollScale * node.wheelScrollScaleComponent.wheelScrollScale * 360f * Time.get_deltaTime() / 60f;
			rPM *= spinItem.sizeScale * spinItem.spinScale;
			spinItem.spinningTransform.Rotate(spinItem.axis, rPM, 1);
		}

		private void CalculateRPM(TankTrackGraphicsNode node)
		{
			float num = node.trackSpeedComponent.trackSpeed / (node.trackScrollItemComponent.trackScroll.radius * (float)Math.PI * 2f);
			node.rpmComponent.rpm = 60f * num;
		}

		private float GetRPM(WheelSpinItem spinItem, float trackSpeed)
		{
			float num = trackSpeed / (spinItem.wheelRadius * (float)Math.PI * 2f);
			return num * 60f;
		}
	}
}
