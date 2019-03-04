using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class MachinePropellersAudioEngine : SingleEntityViewEngine<MachinePropellerAudioNode>, IQueryingEntityViewEngine, IEngine
	{
		private ITaskRoutine _task;

		private int _nodesCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public MachinePropellersAudioEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(MachinePropellerAudioNode entityView)
		{
			if (_nodesCount++ == 0)
			{
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(MachinePropellerAudioNode entityView)
		{
			UpdatePlayState(entityView, -1);
			if (--_nodesCount == 0)
			{
				_task.Pause();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<MachinePropellerAudioNode> audioNodes = entityViewsDB.QueryEntityViews<MachinePropellerAudioNode>();
				for (int i = 0; i < audioNodes.get_Count(); i++)
				{
					float maxSpinning = 0f;
					int maxLevelPlaying = -1;
					MachinePropellerAudioNode node = audioNodes.get_Item(i);
					int count = default(int);
					PropellerAudioNode[] propellerNodes = entityViewsDB.QueryGroupedEntityViewsAsArray<PropellerAudioNode>(node.get_ID(), ref count);
					if (count > 0)
					{
						for (int j = 0; j < count; j++)
						{
							PropellerAudioNode propellerAudioNode = propellerNodes[j];
							if (propellerAudioNode.disabledComponent.enabled)
							{
								int level = propellerAudioNode.levelComponent.level;
								if (propellerAudioNode.spinningComponent.normalizedSpinningSpeed > maxSpinning)
								{
									maxSpinning = propellerAudioNode.spinningComponent.normalizedSpinningSpeed;
								}
								if (propellerAudioNode.spinningComponent.normalizedSpinningSpeed > 0f && level > maxLevelPlaying)
								{
									maxLevelPlaying = level;
								}
							}
						}
						float distance = 300f;
						if (maxLevelPlaying > 0)
						{
							Camera main = Camera.get_main();
							if (main != null)
							{
								distance = Vector3.Distance(node.rbComponent.rb.get_worldCenterOfMass(), main.get_transform().get_position());
							}
						}
						UpdatePlayState(node, maxLevelPlaying);
						UpdateParameters(node, maxSpinning, maxLevelPlaying, distance);
					}
					yield return null;
				}
				yield return null;
			}
		}

		private void UpdatePlayState(MachinePropellerAudioNode node, int maxLevelPlaying)
		{
			IPropellerAudioNamesComponent audioNamesComponent = node.audioNamesComponent;
			IPropellerAudioStateComponent audioStateComponent = node.audioStateComponent;
			GameObject audioGO = node.audioGoComponent.audioGO;
			if (maxLevelPlaying > 0 && !audioStateComponent.isPlaying)
			{
				audioStateComponent.isPlaying = true;
				EventManager.get_Instance().PostEvent(audioNamesComponent.soundEvent, 0, (object)null, audioGO);
			}
			else if (maxLevelPlaying == 0 && audioStateComponent.isPlaying)
			{
				audioStateComponent.isPlaying = false;
				EventManager.get_Instance().PostEvent(audioNamesComponent.soundEvent, 1, (object)null, audioGO);
			}
		}

		public void UpdateParameters(MachinePropellerAudioNode node, float maxSpinning, int maxLevelPlaying, float distance)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			if (node.audioStateComponent.isPlaying)
			{
				if (!Mathf.Approximately(maxSpinning, node.audioStateComponent.lastSpinParam))
				{
					EventManager.get_Instance().SetParameter(node.audioNamesComponent.soundEvent, "SPIN", maxSpinning, node.audioGoComponent.audioGO);
					node.audioStateComponent.lastSpinParam = maxSpinning;
				}
				Vector3 forward = node.rbComponent.rb.get_transform().get_forward();
				Vector3 val = forward - node.audioStateComponent.previousForward;
				Vector3 val2 = Vector3.get_forward() + val;
				Quaternion val3 = Quaternion.FromToRotation(Vector3.get_forward(), val2);
				Vector3 eulerAngles = val3.get_eulerAngles();
				node.audioStateComponent.previousForward = forward;
				float normalizedValue = GetNormalizedValue(eulerAngles.x);
				if (!Mathf.Approximately(normalizedValue, node.audioStateComponent.lastLiftParam))
				{
					EventManager.get_Instance().SetParameter(node.audioNamesComponent.soundEvent, "LIFT", normalizedValue, node.audioGoComponent.audioGO);
					node.audioStateComponent.lastLiftParam = normalizedValue;
				}
				float normalizedValue2 = GetNormalizedValue(eulerAngles.y);
				if (!Mathf.Approximately(normalizedValue2, node.audioStateComponent.lastTurnParam))
				{
					EventManager.get_Instance().SetParameter(node.audioNamesComponent.soundEvent, "TURN", normalizedValue2, node.audioGoComponent.audioGO);
					node.audioStateComponent.lastTurnParam = normalizedValue2;
				}
				if (node.audioStateComponent.maxLevelPlaying != maxLevelPlaying)
				{
					EventManager.get_Instance().SetParameter(node.audioNamesComponent.soundEvent, "LEVEL", (float)maxLevelPlaying, node.audioGoComponent.audioGO);
					node.audioStateComponent.maxLevelPlaying = maxLevelPlaying;
				}
				if (Mathf.Abs(node.audioStateComponent.cameraDistance - distance) > 10f)
				{
					EventManager.get_Instance().SetParameter(node.audioNamesComponent.soundEvent, "Distance", distance, node.audioGoComponent.audioGO);
					node.audioStateComponent.cameraDistance = distance;
				}
			}
		}

		private float GetNormalizedValue(float angle)
		{
			float num = Mathf.Abs(angle);
			if (num > 180f)
			{
				num -= 360f;
			}
			return Mathf.Abs(num / 5f);
		}
	}
}
