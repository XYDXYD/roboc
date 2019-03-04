using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class MachineThrusterAudioEngine : SingleEntityViewEngine<MachineThrusterAudioNode>, IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private const float UPDATE_INTERVAL = 0.25f;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(MachineThrusterAudioNode node)
		{
		}

		protected override void Remove(MachineThrusterAudioNode node)
		{
			StopAudio(node);
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MachineThrusterAudioNode> val = entityViewsDB.QueryEntityViews<MachineThrusterAudioNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				MachineThrusterAudioNode machineThrusterAudioNode = val.get_Item(i);
				if (machineThrusterAudioNode.timeToUpdateComponent.timeToUpdate < 0f)
				{
					machineThrusterAudioNode.timeToUpdateComponent.timeToUpdate = 0.25f;
					int num = -1;
					int num2 = 0;
					int num3 = 0;
					int num4 = default(int);
					ThrusterAudioNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<ThrusterAudioNode>(machineThrusterAudioNode.get_ID(), ref num4);
					for (int j = 0; j < num4; j++)
					{
						ThrusterAudioNode thrusterAudioNode = array[j];
						if (thrusterAudioNode.disabledComponent.disabled)
						{
							continue;
						}
						num3++;
						num2 += thrusterAudioNode.levelComponent.level;
						if (thrusterAudioNode.inputReceivedComponent.received != 0f || thrusterAudioNode.forceAppliedComponent.forceApplied)
						{
							int level = thrusterAudioNode.levelComponent.level;
							if (level > num)
							{
								num = level;
							}
						}
					}
					IThrusterAudioNamesComponent audioNamesComponent = machineThrusterAudioNode.audioNamesComponent;
					IThrusterAudioStateComponent audioStateComponent = machineThrusterAudioNode.audioStateComponent;
					GameObject audioGO = machineThrusterAudioNode.audioGoComponent.audioGO;
					if (num3 > 0 && !audioStateComponent.isJetPlaying)
					{
						audioStateComponent.isJetPlaying = true;
						EventManager.get_Instance().PostEvent(audioNamesComponent.loopSound, 0, (object)null, audioGO);
					}
					else if (num3 == 0 && audioStateComponent.isJetPlaying)
					{
						audioStateComponent.isJetPlaying = false;
						EventManager.get_Instance().PostEvent(audioNamesComponent.loopSound, 1, (object)null, audioGO);
					}
					if (num >= 0 && !audioStateComponent.isJetPlaying)
					{
						audioStateComponent.playStopTimePerLevel = Time.get_time();
					}
					else if (num < 0 && audioStateComponent.isJetPlaying)
					{
						audioStateComponent.playStopTimePerLevel = Time.get_time();
						EventManager.get_Instance().SetParameter(machineThrusterAudioNode.audioNamesComponent.loopSound, "JetPower", 0f, machineThrusterAudioNode.audioGoComponent.audioGO);
						audioStateComponent.lastRamp = 0f;
					}
					if (audioStateComponent.isJetPlaying)
					{
						float distance = 300f;
						Camera main = Camera.get_main();
						if (main != null)
						{
							distance = Vector3.Distance(audioGO.get_transform().get_position(), main.get_transform().get_position());
						}
						UpdateParameters(machineThrusterAudioNode, num2 / num3, distance);
					}
				}
				else
				{
					machineThrusterAudioNode.timeToUpdateComponent.timeToUpdate -= Time.get_deltaTime();
				}
			}
		}

		public void UpdateParameters(MachineThrusterAudioNode node, int level, float distance)
		{
			IThrusterAudioStateComponent audioStateComponent = node.audioStateComponent;
			IRampAndFadeTimeComponent rampAndFadeTimeComponent = node.rampAndFadeTimeComponent;
			float num = Mathf.Clamp01((Time.get_time() - audioStateComponent.playStopTimePerLevel) / rampAndFadeTimeComponent.totalRampUpTime);
			if (num != audioStateComponent.lastRamp)
			{
				EventManager.get_Instance().SetParameter(node.audioNamesComponent.loopSound, "JetPower", num, node.audioGoComponent.audioGO);
				audioStateComponent.lastRamp = num;
			}
			if (level != audioStateComponent.level)
			{
				EventManager.get_Instance().SetParameter(node.audioNamesComponent.loopSound, "LEVEL", (float)level, node.audioGoComponent.audioGO);
				audioStateComponent.level = level;
			}
			if (Mathf.Abs(audioStateComponent.cameraDistance - distance) > 10f)
			{
				EventManager.get_Instance().SetParameter(node.audioNamesComponent.loopSound, "Distance", distance, node.audioGoComponent.audioGO);
				audioStateComponent.cameraDistance = distance;
			}
		}

		private void StopAudio(MachineThrusterAudioNode node)
		{
			IThrusterAudioNamesComponent audioNamesComponent = node.audioNamesComponent;
			IThrusterAudioStateComponent audioStateComponent = node.audioStateComponent;
			GameObject audioGO = node.audioGoComponent.audioGO;
			if (audioStateComponent.isJetPlaying)
			{
				audioStateComponent.isJetPlaying = false;
				EventManager.get_Instance().PostEvent(audioNamesComponent.loopSound, 1, (object)null, audioGO);
			}
		}
	}
}
