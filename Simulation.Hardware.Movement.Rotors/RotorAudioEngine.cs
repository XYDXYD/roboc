using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RotorAudioEngine : SingleEntityViewEngine<MachineRotorAudioNode>, IQueryingEntityViewEngine, IEngine
	{
		private const string AUDIO_PARAM = "LOAD";

		private const string AUDIO_RISE_PARAM = "LIFT_UP";

		private const string AUDIO_LOWER_PARAM = "LIFT_DOWN";

		private ITaskRoutine _task;

		private int _nodesCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RotorAudioEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		protected override void Add(MachineRotorAudioNode node)
		{
			if (_nodesCount++ == 0)
			{
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(MachineRotorAudioNode node)
		{
			UpdatePlayState(node, 0);
			if (--_nodesCount == 0)
			{
				_task.Pause();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<MachineRotorAudioNode> nodes = entityViewsDB.QueryEntityViews<MachineRotorAudioNode>();
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					MachineRotorAudioNode node = nodes.get_Item(i);
					int count = default(int);
					RotorBladeAudioNode[] rotorNodes = entityViewsDB.QueryGroupedEntityViewsAsArray<RotorBladeAudioNode>(node.get_ID(), ref count);
					if (count > 0)
					{
						int num = 0;
						float num2 = 0f;
						for (int j = 0; j < count; j++)
						{
							RotorBladeAudioNode rotorBladeAudioNode = rotorNodes[i];
							if (!rotorBladeAudioNode.disabledComponent.disabled && rotorBladeAudioNode.visibilityComponent.inRange)
							{
								num++;
								num2 += (float)rotorBladeAudioNode.rotorAudioLevelComponent.audioLevel;
							}
						}
						float level = 0f;
						float distance = 300f;
						if (num > 0)
						{
							level = num2 / (float)num;
						}
						UpdatePlayState(node, num);
						UpdateAudioParameters(node, level, distance);
					}
					yield return null;
				}
				yield return null;
			}
		}

		private void UpdateAudioParameters(MachineRotorAudioNode machineNode, float level, float distance)
		{
			IPlayingAudioComponent playingAudioComponent = machineNode.playingAudioComponent;
			GameObject audioGO = machineNode.audioGameObjectComponent.audioGO;
			IAudioLiftingLoweringComponent audioLiftingLoweringComponent = machineNode.audioLiftingLoweringComponent;
			float power = machineNode.powerValueComponent.power;
			float num = 0f;
			float num2 = 0f;
			float num3 = power;
			if (audioLiftingLoweringComponent.lifting)
			{
				num2 = power;
			}
			else if (audioLiftingLoweringComponent.lowering)
			{
				num = power;
			}
			if (Mathf.Abs(playingAudioComponent.prevLowerParam - num) > 0.01f)
			{
				EventManager.get_Instance().SetParameter(playingAudioComponent.playingAudio, "LIFT_DOWN", num, audioGO);
				playingAudioComponent.prevLowerParam = num;
			}
			if (Mathf.Abs(playingAudioComponent.prevRiseParam - num2) > 0.01f)
			{
				EventManager.get_Instance().SetParameter(playingAudioComponent.playingAudio, "LIFT_UP", num2, audioGO);
				playingAudioComponent.prevRiseParam = num2;
			}
			if (Mathf.Abs(playingAudioComponent.prevLoadParam - num3) > 0.01f)
			{
				EventManager.get_Instance().SetParameter(playingAudioComponent.playingAudio, "LOAD", power, audioGO);
				playingAudioComponent.prevLoadParam = num3;
			}
			if (playingAudioComponent.avgLevel != level)
			{
				EventManager.get_Instance().SetParameter(playingAudioComponent.playingAudio, "LEVEL", level, audioGO);
				playingAudioComponent.avgLevel = level;
			}
			if (Mathf.Abs(playingAudioComponent.cameraDistance - distance) > 10f)
			{
				EventManager.get_Instance().SetParameter(playingAudioComponent.playingAudio, "Distance", distance, audioGO);
				playingAudioComponent.cameraDistance = distance;
			}
		}

		private void UpdatePlayState(MachineRotorAudioNode machineNode, int activeRotorsCount)
		{
			IPlayingAudioComponent playingAudioComponent = machineNode.playingAudioComponent;
			GameObject audioGO = machineNode.audioGameObjectComponent.audioGO;
			if (activeRotorsCount == 0 && machineNode.playingAudioComponent.isAudioPlaying)
			{
				EventManager.get_Instance().PostEvent(playingAudioComponent.playingAudio, 1, (object)null, audioGO);
				playingAudioComponent.isAudioPlaying = false;
			}
			else if (activeRotorsCount > 0 && !machineNode.playingAudioComponent.isAudioPlaying)
			{
				EventManager.get_Instance().PostEvent(playingAudioComponent.playingAudio, 0, (object)null, audioGO);
				playingAudioComponent.isAudioPlaying = true;
			}
		}

		public void Ready()
		{
		}
	}
}
