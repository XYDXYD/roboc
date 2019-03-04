using BehaviorDesigner.Runtime.Tasks;
using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class SelectTarget : Conditional
	{
		public SharedTargetInfoComponent selectedTargetInfo;

		public SharedMachineTargetsEntityView agent;

		private const float COOL_DOWN_TIME = 3f;

		private MachineTargetsEntityView _agent;

		public SelectTarget()
			: this()
		{
		}

		public override void OnStart()
		{
			_agent = agent.get_Value();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			TaskStatus result = 2;
			FasterList<MachineTargetsEntityView> visibleTargets = _agent.machineTargetsComponent.visibleTargets;
			float num = float.MaxValue;
			Vector3 worldCenterOfMass = _agent.rigidBodyComponent.rb.get_worldCenterOfMass();
			MachineTargetsEntityView machineTargetsEntityView = null;
			for (int i = 0; i < visibleTargets.get_Count(); i++)
			{
				MachineTargetsEntityView machineTargetsEntityView2 = visibleTargets.get_Item(i);
				float num2 = Vector3.SqrMagnitude(worldCenterOfMass - machineTargetsEntityView2.rigidBodyComponent.rb.get_worldCenterOfMass());
				if (num2 < num)
				{
					machineTargetsEntityView = machineTargetsEntityView2;
					num = num2;
				}
			}
			if (machineTargetsEntityView != null)
			{
				selectedTargetInfo.set_Value(machineTargetsEntityView.machineTargetInfoComponent.targetInfo);
			}
			else
			{
				selectedTargetInfo.set_Value((TargetInfo)null);
				result = 1;
			}
			return result;
		}
	}
}
