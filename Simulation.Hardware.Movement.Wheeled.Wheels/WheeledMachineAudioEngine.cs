using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal class WheeledMachineAudioEngine : SingleEntityViewEngine<WheeledMachineAudioNode>, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<int, FasterList<WheelAudioNode>> _wheelsPerMachine = new Dictionary<int, FasterList<WheelAudioNode>>(20);

		private const float CLAMP_MIN = 0.0001f;

		private const float CLAMP_MAX = 0.9999f;

		private const float WHEEL_RPM_SCALER = 1f;

		private const float WHEEL_LOAD_SCALER = 1f;

		private ITaskRoutine _task;

		private int _nodesCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public WheeledMachineAudioEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(WheeledMachineAudioNode node)
		{
			node.motorAudioStateComponent.avgLevel = -1f;
			if (_nodesCount++ == 0)
			{
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(WheeledMachineAudioNode node)
		{
			UpdateWheelSoundState(node, 0);
			UpdateMotorSoundState(node, 0);
			if (--_nodesCount == 0)
			{
				_task.Pause();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<WheeledMachineAudioNode> audioNodes = entityViewsDB.QueryEntityViews<WheeledMachineAudioNode>();
				for (int i = 0; i < audioNodes.get_Count(); i++)
				{
					WheeledMachineAudioNode audioNode = audioNodes.get_Item(i);
					int count = default(int);
					WheelAudioNode[] wheels = entityViewsDB.QueryGroupedEntityViewsAsArray<WheelAudioNode>(audioNode.get_ID(), ref count);
					if (count > 0)
					{
						float num = 0f;
						float num2 = 0f;
						float num3 = 0f;
						int num4 = 0;
						float num5 = 0f;
						float num6 = 0f;
						if (audioNode.machineRectifierComponent.functionalsEnabled)
						{
							for (int j = 0; j < count; j++)
							{
								WheelAudioNode wheelAudioNode = wheels[j];
								if (!wheelAudioNode.hardwareDisabledComponent.disabled && wheelAudioNode.visibilityComponent.inRange)
								{
									float num7 = wheelAudioNode.wheelSpeedComponent.wheelSpeed / wheelAudioNode.maxSpeedComponent.maxSpeed;
									num7 = Mathf.Abs(num7) * 1f;
									num += num7;
									float num8 = wheelAudioNode.wheelLoadComponent.wheelLoad * 1f;
									if (!wheelAudioNode.groundedComponent.grounded)
									{
										num8 = 0f;
									}
									num2 += num8;
									int level = wheelAudioNode.levelComponent.level;
									num3 += (float)level;
									num4++;
									num5 += Mathf.Abs(wheelAudioNode.slipComponent.forwardSlip);
									num6 += Mathf.Abs(wheelAudioNode.slipComponent.sidewaysSlip);
								}
							}
						}
						float distance = 300f;
						if (num4 > 0)
						{
							num2 = Mathf.Clamp(num2 / (float)num4, 0.0001f, 0.9999f);
							num = Mathf.Clamp(num / (float)num4, 0.0001f, 0.9999f);
							num3 /= (float)num4;
							Camera main = Camera.get_main();
							if (main != null)
							{
								distance = Vector3.Distance(audioNode.rigidbodyComponent.rb.get_worldCenterOfMass(), main.get_transform().get_position());
							}
						}
						num5 /= (float)count;
						num6 /= (float)count;
						UpdateWheelSoundState(audioNode, num4);
						UpdateWheelParams(audioNode, num5, num6);
						UpdateMotorSoundState(audioNode, num4);
						UpdateMotorParams(audioNode, num, num2, num3, distance);
					}
					yield return null;
				}
			}
		}

		private void UpdateMotorParams(WheeledMachineAudioNode audioNode, float totalRPMScale, float totalLoad, float totalLevel, float distance)
		{
			if (audioNode.motorAudioStateComponent.motorSoundPlaying)
			{
				if (audioNode.motorAudioStateComponent.avgLevel != totalLevel)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioNameComponent.motorSoundEvent, "LEVEL", totalLevel, audioNode.audioGoComponent.audioGO);
					audioNode.motorAudioStateComponent.avgLevel = totalLevel;
				}
				if (Mathf.Abs(audioNode.motorAudioStateComponent.totalRPMScale - totalRPMScale) > 0.01f)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioNameComponent.motorSoundEvent, "RPM", totalRPMScale, audioNode.audioGoComponent.audioGO);
					audioNode.motorAudioStateComponent.totalRPMScale = totalRPMScale;
				}
				if (Mathf.Abs(audioNode.motorAudioStateComponent.totalLoadScale - totalLoad) > 0.01f)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioNameComponent.motorSoundEvent, "LOAD", totalLoad, audioNode.audioGoComponent.audioGO);
					audioNode.motorAudioStateComponent.totalLoadScale = totalLoad;
				}
				if (Mathf.Abs(audioNode.motorAudioStateComponent.cameraDistance - distance) > 10f)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioNameComponent.motorSoundEvent, "Distance", distance, audioNode.audioGoComponent.audioGO);
					audioNode.motorAudioStateComponent.cameraDistance = distance;
				}
			}
		}

		private void UpdateMotorSoundState(WheeledMachineAudioNode audioNode, int activeWheels)
		{
			if (!audioNode.motorAudioStateComponent.motorSoundPlaying && activeWheels > 0)
			{
				EventManager.get_Instance().PostEvent(audioNode.audioNameComponent.motorSoundEvent, 0, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.motorAudioStateComponent.motorSoundPlaying = true;
			}
			else if (audioNode.motorAudioStateComponent.motorSoundPlaying && activeWheels == 0)
			{
				EventManager.get_Instance().PostEvent(audioNode.audioNameComponent.motorSoundEvent, 1, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.motorAudioStateComponent.motorSoundPlaying = false;
			}
		}

		private void UpdateWheelParams(WheeledMachineAudioNode audioNode, float wheelspinScale, float sidewaysSkidScale)
		{
			if (audioNode.wheelAudioStateComponent.wheelSoundPlaying)
			{
				if (audioNode.wheelAudioStateComponent.wheelspinScale != wheelspinScale)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioNameComponent.wheelSoundEvent, "FORWARDSKID", wheelspinScale, audioNode.audioGoComponent.audioGO);
					audioNode.wheelAudioStateComponent.wheelspinScale = wheelspinScale;
				}
				if (audioNode.wheelAudioStateComponent.sidewaysSkidScale != sidewaysSkidScale)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioNameComponent.wheelSoundEvent, "WHEELSPIN", sidewaysSkidScale, audioNode.audioGoComponent.audioGO);
					audioNode.wheelAudioStateComponent.sidewaysSkidScale = sidewaysSkidScale;
				}
			}
		}

		private void UpdateWheelSoundState(WheeledMachineAudioNode audioNode, int activeWheels)
		{
			if (!audioNode.wheelAudioStateComponent.wheelSoundPlaying && activeWheels > 0)
			{
				EventManager.get_Instance().PostEvent(audioNode.audioNameComponent.wheelSoundEvent, 0, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.wheelAudioStateComponent.wheelSoundPlaying = true;
			}
			else if (audioNode.wheelAudioStateComponent.wheelSoundPlaying && activeWheels == 0)
			{
				EventManager.get_Instance().PostEvent(audioNode.audioNameComponent.wheelSoundEvent, 1, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.wheelAudioStateComponent.wheelSoundPlaying = false;
			}
		}
	}
}
