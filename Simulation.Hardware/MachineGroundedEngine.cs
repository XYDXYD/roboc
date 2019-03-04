using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware
{
	internal class MachineGroundedEngine : IQueryingEntityViewEngine, IEngine
	{
		private const float GROUND_CLEARANCE = 2f;

		[Inject]
		public MachinePreloader machinePreloader
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)TickTask);
		}

		private IEnumerator TickTask()
		{
			while (true)
			{
				Tick();
				yield return null;
			}
		}

		private void Tick()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			int num = default(int);
			MachineGroundedNode[] array = entityViewsDB.QueryEntityViewsAsArray<MachineGroundedNode>(ref num);
			for (int i = 0; i < num; i++)
			{
				MachineGroundedNode machineGroundedNode = array[i];
				int iD = machineGroundedNode.get_ID();
				PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(iD);
				Rigidbody rb = machineGroundedNode.rigidbodyComponent.rb;
				Vector3 machineSize = preloadedMachine.machineInfo.MachineSize;
				float num2 = 2f + machineSize.y * 0.5f;
				bool flag = Physics.Raycast(rb.get_worldCenterOfMass(), Vector3.get_down(), num2, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
				if (!flag)
				{
					flag = Physics.Raycast(rb.get_worldCenterOfMass(), -rb.get_transform().get_up(), num2, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
				}
				machineGroundedNode.machineGroundedComponent.grounded = flag;
			}
		}
	}
}
