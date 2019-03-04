using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class AerofoilMachineAFXEngine : SingleEntityViewEngine<MachineAerofoilAudioNode>, IQueryingEntityViewEngine, IEngine
	{
		public const string AudioParamLevel = "LEVEL";

		public const string AudioParamPower = "POWER";

		public const string AudioParamLift = "LIFT";

		private const float VELOCITY_SCALAR = 0.1f;

		private ITaskRoutine _task;

		private int _nodesCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public AerofoilMachineAFXEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(MachineAerofoilAudioNode entityView)
		{
			if (_nodesCount++ == 0)
			{
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(MachineAerofoilAudioNode entityView)
		{
			UpdatePlayState(entityView, 0);
			if (--_nodesCount == 0)
			{
				_task.Pause();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<MachineAerofoilAudioNode> audioNodes = entityViewsDB.QueryEntityViews<MachineAerofoilAudioNode>();
				for (int i = 0; i < audioNodes.get_Count(); i++)
				{
					MachineAerofoilAudioNode machineAerofoilAudioNode = audioNodes.get_Item(i);
					int num = default(int);
					AerofoilAudioNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<AerofoilAudioNode>(machineAerofoilAudioNode.get_ID(), ref num);
					float num2 = 0f;
					int num3 = 0;
					for (int j = 0; j < num; j++)
					{
						AerofoilAudioNode aerofoilAudioNode = array[j];
						if (!aerofoilAudioNode.disabledComponent.disabled)
						{
							num2 += aerofoilAudioNode.aerofoilAfxComponent.audioLevel;
							num3++;
						}
					}
					float distance = 300f;
					if (num3 > 0)
					{
						num2 /= (float)num3;
						Camera main = Camera.get_main();
						if (main != null)
						{
							distance = Vector3.Distance(machineAerofoilAudioNode.rigidbodyComponent.rb.get_worldCenterOfMass(), main.get_transform().get_position());
						}
					}
					UpdatePlayState(machineAerofoilAudioNode, num3);
					UpdateParameters(machineAerofoilAudioNode, num2, distance);
				}
				yield return null;
			}
		}

		private void UpdatePlayState(MachineAerofoilAudioNode node, int activeNodeCount)
		{
			IMachineAerofoilAudioComponent audioComponent = node.audioComponent;
			if (activeNodeCount > 0 && !audioComponent.isAudioPlaying)
			{
				audioComponent.isAudioPlaying = true;
				EventManager.get_Instance().PostEvent(audioComponent.audioEvent, 0, (object)null, node.audioGOComponent.audioGO);
			}
			else if (activeNodeCount == 0 && audioComponent.isAudioPlaying)
			{
				audioComponent.isAudioPlaying = false;
				EventManager.get_Instance().PostEvent(audioComponent.audioEvent, 1, (object)null, node.audioGOComponent.audioGO);
			}
		}

		public void UpdateParameters(MachineAerofoilAudioNode node, float avgLevel, float distance)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			IMachineAerofoilAudioComponent audioComponent = node.audioComponent;
			if (node.audioComponent.isAudioPlaying)
			{
				float num = 0f;
				float num2 = 0f;
				if (node.ownerComponent.ownedByMe)
				{
					Vector3 aerofoilInput = node.aerofoilComponent.aerofoilInput;
					Vector3 val = default(Vector3);
					val._002Ector(0f, aerofoilInput.y, aerofoilInput.z);
					num = val.get_magnitude();
					num2 = Mathf.Clamp01(Mathf.Abs(aerofoilInput.x) + Mathf.Clamp01(-1f * aerofoilInput.y));
				}
				else
				{
					Vector3 worldCenterOfMass = node.rigidbodyComponent.rb.get_worldCenterOfMass();
					float num3 = Vector3.Distance(node.rigidbodyComponent.rb.get_worldCenterOfMass(), node.audioComponent.lastPos) / Time.get_deltaTime();
					node.audioComponent.lastPos = worldCenterOfMass;
					num = (num2 = 1f - Mathf.Clamp01(num3 * 0.1f));
				}
				if (audioComponent.audioParamLevel != avgLevel)
				{
					EventManager.get_Instance().SetParameter(audioComponent.audioEvent, "LEVEL", avgLevel, node.audioGOComponent.audioGO);
					audioComponent.audioParamLevel = avgLevel;
				}
				if (Mathf.Abs(audioComponent.audioParamDistance - distance) > 10f)
				{
					EventManager.get_Instance().SetParameter(audioComponent.audioEvent, "Distance", distance, node.audioGOComponent.audioGO);
					audioComponent.audioParamDistance = distance;
				}
				if (Mathf.Abs(audioComponent.audioParamPower - num) > 0.1f)
				{
					EventManager.get_Instance().SetParameter(audioComponent.audioEvent, "POWER", num, node.audioGOComponent.audioGO);
					audioComponent.audioParamPower = num;
				}
				if (Mathf.Abs(audioComponent.audioParamLift - num2) > 0.1f)
				{
					EventManager.get_Instance().SetParameter(audioComponent.audioEvent, "LIFT", num2, node.audioGOComponent.audioGO);
					audioComponent.audioParamLift = num2;
				}
			}
		}
	}
}
