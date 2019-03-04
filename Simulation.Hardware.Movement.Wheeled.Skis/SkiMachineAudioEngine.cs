using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal class SkiMachineAudioEngine : SingleEntityViewEngine<SkiMachineAudioNode>, IQueryingEntityViewEngine, IEngine
	{
		private const string loopAudioName = "Ski_Loop";

		private const string turnAudioName = "Ski_Turn";

		private const float CLAMP_MIN = 0.0001f;

		private const float CLAMP_MAX = 0.9999f;

		private ITaskRoutine _task;

		private int _skisNodeCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public SkiMachineAudioEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(SkiMachineAudioNode node)
		{
			if (_skisNodeCount++ == 0)
			{
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(SkiMachineAudioNode node)
		{
			if (--_skisNodeCount == 0)
			{
				_task.Stop();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<SkiMachineAudioNode> audioNodes = entityViewsDB.QueryEntityViews<SkiMachineAudioNode>();
				for (int i = 0; i < audioNodes.get_Count(); i++)
				{
					SkiMachineAudioNode skiMachineAudioNode = audioNodes.get_Item(i);
					int num = default(int);
					SkiAudioNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<SkiAudioNode>(skiMachineAudioNode.get_ID(), ref num);
					float num2 = 0f;
					int num3 = 0;
					int num4 = 0;
					if (skiMachineAudioNode.machineRectifierComponent.functionalsEnabled)
					{
						for (int j = 0; j < num; j++)
						{
							SkiAudioNode skiAudioNode = array[j];
							if (!skiAudioNode.hardwareDisabledComponent.disabled && skiAudioNode.visibilityComponent.inRange && skiAudioNode.groundedComponent.grounded)
							{
								float num5 = skiAudioNode.speedComponent.currentSpeed / skiAudioNode.maxSpeedComponent.maxSpeed;
								num2 += Mathf.Abs(num5);
								num3++;
								if (Mathf.Abs(skiAudioNode.steeringComponent.currentSteeringAngle) > 0.05f)
								{
									num4++;
								}
							}
						}
					}
					if (num3 > 0)
					{
						num2 = Mathf.Clamp(num2 / (float)num3, 0.0001f, 0.9999f);
					}
					UpdateturnSoundState(skiMachineAudioNode, num4);
					UpdateLoopSoundState(skiMachineAudioNode, num3);
					UpdateParams(skiMachineAudioNode, num2);
				}
				yield return null;
			}
		}

		private void UpdateParams(SkiMachineAudioNode audioNode, float speedScale)
		{
			if (audioNode.skiAudioStateComponent.loopSoundPlaying && Mathf.Abs(audioNode.skiAudioStateComponent.speedScale - speedScale) > 0.01f)
			{
				EventManager.get_Instance().SetParameter("Ski_Loop", "SPEED", speedScale, audioNode.audioGoComponent.audioGO);
				audioNode.skiAudioStateComponent.speedScale = speedScale;
			}
		}

		private void UpdateLoopSoundState(SkiMachineAudioNode audioNode, int groundedSkis)
		{
			if (!audioNode.skiAudioStateComponent.loopSoundPlaying && groundedSkis > 0)
			{
				EventManager.get_Instance().PostEvent("Ski_Loop", 0, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.skiAudioStateComponent.loopSoundPlaying = true;
			}
			else if (audioNode.skiAudioStateComponent.loopSoundPlaying && groundedSkis == 0)
			{
				EventManager.get_Instance().PostEvent("Ski_Loop", 1, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.skiAudioStateComponent.loopSoundPlaying = false;
			}
		}

		private void UpdateturnSoundState(SkiMachineAudioNode audioNode, int steeringSkis)
		{
			if (!audioNode.skiAudioStateComponent.turnSoundPlaying && steeringSkis > 0)
			{
				EventManager.get_Instance().PostEvent("Ski_Turn", 0, (object)null, audioNode.audioGoComponent.audioGO);
				audioNode.skiAudioStateComponent.turnSoundPlaying = true;
			}
			else if (audioNode.skiAudioStateComponent.turnSoundPlaying && steeringSkis == 0)
			{
				audioNode.skiAudioStateComponent.turnSoundPlaying = false;
			}
		}
	}
}
