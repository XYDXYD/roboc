using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackAudioEngine : SingleEntityViewEngine<TankTrackAudioManagerNode>, IQueryingEntityViewEngine, IEngine
	{
		private const string AUDIO_PARAM_NAME = "MAXSPEED";

		private ITaskRoutine _task;

		private int _nodesCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public TankTrackAudioEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(TankTrackAudioManagerNode node)
		{
			if (_nodesCount++ == 0)
			{
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(TankTrackAudioManagerNode node)
		{
			StopTankTrackAudio(node);
			if (--_nodesCount == 0)
			{
				_task.Pause();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<TankTrackAudioManagerNode> nodes = entityViewsDB.QueryEntityViews<TankTrackAudioManagerNode>();
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					TankTrackAudioManagerNode node = nodes.get_Item(i);
					int count = default(int);
					TankTrackAudioNode[] tracks = entityViewsDB.QueryGroupedEntityViewsAsArray<TankTrackAudioNode>(node.get_ID(), ref count);
					if (count > 0)
					{
						int num = 0;
						float num2 = 0f;
						node.maxSpeedRatioComponent.maxSpeedRatio = 0f;
						Rigidbody rb = node.rigidBodyComponent.rb;
						float num3 = Vector3.Dot(rb.get_angularVelocity(), rb.get_transform().get_up());
						for (int j = 0; j < count; j++)
						{
							TankTrackAudioNode tankTrackAudioNode = tracks[j];
							if (!tankTrackAudioNode.hardwareDisabledComponent.disabled && tankTrackAudioNode.visibilityComponent.inRange)
							{
								num2 += (float)tankTrackAudioNode.partLevelComponent.level;
								float num4 = Mathf.Abs(tankTrackAudioNode.trackSpeedComponent.trackSpeed) / tankTrackAudioNode.maxSpeedComponent.maxSpeed;
								float num5 = Mathf.Abs(num3) / tankTrackAudioNode.maxTurnRateComponent.maxTurnRateStopped;
								node.maxSpeedRatioComponent.maxSpeedRatio = Mathf.Max(new float[3]
								{
									node.maxSpeedRatioComponent.maxSpeedRatio,
									num4,
									num5
								});
								num++;
							}
						}
						float totalLevel = 0f;
						float distance = 300f;
						if (num > 0)
						{
							totalLevel = num2 / (float)num;
							Camera main = Camera.get_main();
							if (main != null)
							{
								distance = Vector3.Distance(node.rigidBodyComponent.rb.get_worldCenterOfMass(), main.get_transform().get_position());
							}
						}
						if (num == 0 && node.tankTrackAudioPlayingComponent.audioPlaying)
						{
							StopTankTrackAudio(node);
						}
						else if (num > 0 && !node.tankTrackAudioPlayingComponent.audioPlaying)
						{
							PlayTankTrackAudio(node);
						}
						UpdateMotorParams(node, node.maxSpeedRatioComponent.maxSpeedRatio, totalLevel, distance);
					}
					yield return null;
				}
				yield return null;
			}
		}

		private void UpdateMotorParams(TankTrackAudioManagerNode audioNode, float totalRPMScale, float totalLevel, float distance)
		{
			if (audioNode.tankTrackAudioPlayingComponent.audioPlaying)
			{
				if (audioNode.tankTrackAudioPlayingComponent.avgLevel != totalLevel)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioEventComponent.audioEvent, "LEVEL", totalLevel, audioNode.audioGoComponent.audioGO);
					audioNode.tankTrackAudioPlayingComponent.avgLevel = totalLevel;
				}
				if (Mathf.Abs(audioNode.maxSpeedRatioComponent.prevMaxSpeedRatio - totalRPMScale) > 0.01f)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioEventComponent.audioEvent, "MAXSPEED", totalRPMScale, audioNode.audioGoComponent.audioGO);
					audioNode.maxSpeedRatioComponent.prevMaxSpeedRatio = totalRPMScale;
				}
				if (Mathf.Abs(audioNode.tankTrackAudioPlayingComponent.cameraDistance - distance) > 10f)
				{
					EventManager.get_Instance().SetParameter(audioNode.audioEventComponent.audioEvent, "Distance", distance, audioNode.audioGoComponent.audioGO);
					audioNode.tankTrackAudioPlayingComponent.cameraDistance = distance;
				}
			}
		}

		private void PlayTankTrackAudio(TankTrackAudioManagerNode node)
		{
			EventManager.get_Instance().PostEvent(node.audioEventComponent.audioEvent, 0, (object)null, node.audioGoComponent.audioGO);
			node.tankTrackAudioPlayingComponent.audioPlaying = true;
		}

		private void StopTankTrackAudio(TankTrackAudioManagerNode node)
		{
			EventManager.get_Instance().PostEvent(node.audioEventComponent.audioEvent, 1, (object)null, node.audioGoComponent.audioGO);
			node.tankTrackAudioPlayingComponent.audioPlaying = false;
		}
	}
}
