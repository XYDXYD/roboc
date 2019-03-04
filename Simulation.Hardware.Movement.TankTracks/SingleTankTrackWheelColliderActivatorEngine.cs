using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal class SingleTankTrackWheelColliderActivatorEngine : SingleEntityViewEngine<SingleTrackActivatorNode>, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<int, FasterList<SingleTrackActivatorNode>> _tracksPerMachine = new Dictionary<int, FasterList<SingleTrackActivatorNode>>();

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(SingleTrackActivatorNode node)
		{
			int machineId = node.ownerComponent.machineId;
			if (!_tracksPerMachine.TryGetValue(machineId, out FasterList<SingleTrackActivatorNode> value))
			{
				value = (_tracksPerMachine[machineId] = new FasterList<SingleTrackActivatorNode>());
			}
			value.Add(node);
			CheckForSingleTrackActivation(machineId);
			node.hardwareDisabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)HandleMovementPartDestroyed);
		}

		protected override void Remove(SingleTrackActivatorNode node)
		{
			int machineId = node.ownerComponent.machineId;
			if (_tracksPerMachine.TryGetValue(node.ownerComponent.machineId, out FasterList<SingleTrackActivatorNode> value))
			{
				value.Remove(node);
				if (value.get_Count() == 0)
				{
					_tracksPerMachine.Remove(machineId);
				}
			}
			node.hardwareDisabledComponent.isPartDisabled.StopNotify((Action<int, bool>)HandleMovementPartDestroyed);
		}

		private void HandleMovementPartDestroyed(int partId, bool disabled)
		{
			SingleTrackActivatorNode singleTrackActivatorNode = default(SingleTrackActivatorNode);
			if (entityViewsDB.TryQueryEntityView<SingleTrackActivatorNode>(partId, ref singleTrackActivatorNode))
			{
				int machineId = singleTrackActivatorNode.ownerComponent.machineId;
				if (singleTrackActivatorNode.hardwareDisabledComponent.disabled && singleTrackActivatorNode.supportCollidersParentComponent.supportParent.get_activeSelf())
				{
					singleTrackActivatorNode.supportCollidersParentComponent.supportParent.SetActive(false);
					singleTrackActivatorNode.regularCollidersParentComponent.regularParent.SetActive(true);
				}
				CheckForSingleTrackActivation(machineId);
			}
		}

		private void CheckForSingleTrackActivation(int machineId)
		{
			int num = 0;
			SingleTrackActivatorNode singleTrackActivatorNode = null;
			if (!_tracksPerMachine.TryGetValue(machineId, out FasterList<SingleTrackActivatorNode> value))
			{
				return;
			}
			for (int i = 0; i < value.get_Count(); i++)
			{
				SingleTrackActivatorNode singleTrackActivatorNode2 = value.get_Item(i);
				if (singleTrackActivatorNode2.transformComponent.T.get_gameObject().get_activeInHierarchy())
				{
					if (num == 0)
					{
						singleTrackActivatorNode = singleTrackActivatorNode2;
					}
					num++;
				}
			}
			bool flag = num == 1;
			if (flag && singleTrackActivatorNode != null)
			{
				singleTrackActivatorNode.supportCollidersParentComponent.supportParent.SetActive(true);
				singleTrackActivatorNode.regularCollidersParentComponent.regularParent.SetActive(false);
				return;
			}
			for (int j = 0; j < value.get_Count(); j++)
			{
				SingleTrackActivatorNode singleTrackActivatorNode3 = value.get_Item(j);
				GameObject supportParent = singleTrackActivatorNode3.supportCollidersParentComponent.supportParent;
				GameObject regularParent = singleTrackActivatorNode3.regularCollidersParentComponent.regularParent;
				if (supportParent.get_activeSelf() != flag)
				{
					supportParent.SetActive(flag);
					regularParent.SetActive(!flag);
				}
			}
		}
	}
}
