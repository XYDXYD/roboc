using Fabric;
using Svelto.ECS;
using Svelto.ECS.Internal;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverBladeAFXEngine : MultiEntityViewsEngine<MachineHoverNode>, IQueryingEntityViewEngine, IEngine
	{
		private struct HoverStruct
		{
			public readonly IMachineFunctionalComponent info;

			public readonly IMachineStunComponent stunComponent;

			public readonly int machineID;

			private readonly Rigidbody rb;

			private IMachineInputComponent _machineInputComponent;

			public bool soundPlaying
			{
				get;
				internal set;
			}

			public GameObject audioGO
			{
				get;
				private set;
			}

			public float momentum
			{
				get;
				private set;
			}

			public float power
			{
				get;
				private set;
			}

			public int currentLevel
			{
				get;
				internal set;
			}

			public float lastMomentumValueSet
			{
				get;
				internal set;
			}

			public float lastPowerValueSet
			{
				get;
				internal set;
			}

			public string loopAudioName
			{
				get;
				private set;
			}

			public float cameraDistance
			{
				get;
				internal set;
			}

			public float lastCameraDistance
			{
				get;
				internal set;
			}

			public HoverStruct(MachineHoverNode node)
			{
				this = default(HoverStruct);
				info = node.functionalComponent;
				stunComponent = node.machineStunComponent;
				rb = node.rigidbodyComponent.rb;
				_machineInputComponent = node.inputComponent;
				audioGO = node.audioGOComponent.audioGO;
				machineID = node.get_ID();
				loopAudioName = ((!node.owner.ownedByMe) ? "HoverBlades_Timeline_Enemy" : "HoverBlades_Timeline");
				currentLevel = -1;
			}

			public void RefreshValues()
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				Vector3 velocity = rb.get_velocity();
				momentum = Mathf.Clamp(velocity.get_magnitude(), 0f, 15f) / 15f;
				Vector4 digitalInput = _machineInputComponent.digitalInput;
				power = Mathf.Clamp01(digitalInput.get_sqrMagnitude());
				Camera main = Camera.get_main();
				if (main != null)
				{
					cameraDistance = Vector3.Distance(rb.get_worldCenterOfMass(), main.get_transform().get_position());
				}
			}
		}

		private const float GENERAL_MAX_SPEED = 15f;

		public const string LOOP_AUDIO_NAME = "HoverBlades_Timeline";

		public const string LOOP_AUDIO_NAME_ENEMY = "HoverBlades_Timeline_Enemy";

		private readonly Dictionary<int, ITaskRoutine> _tasks;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public HoverBladeAFXEngine()
		{
			_tasks = new Dictionary<int, ITaskRoutine>();
		}

		protected override void Add(MachineHoverNode node)
		{
			ITaskRoutine val = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			val.SetEnumerator(Update(new HoverStruct(node)));
			val.Start((Action<PausableTaskException>)null, (Action)null);
			_tasks.Add(node.get_ID(), val);
		}

		protected override void Remove(MachineHoverNode node)
		{
			ITaskRoutine val = _tasks[node.get_ID()];
			val.Stop();
			_tasks.Remove(node.get_ID());
			string text = (!node.owner.ownedByMe) ? "HoverBlades_Timeline_Enemy" : "HoverBlades_Timeline";
			EventManager.get_Instance().PostEvent(text, 1, (object)null, node.audioGOComponent.audioGO);
		}

		private IEnumerator Update(HoverStruct hover)
		{
			while (true)
			{
				int hoverCount = 0;
				int hoverLevel = 0;
				if (hover.info.functionalsEnabled && !hover.stunComponent.stunned)
				{
					hover.RefreshValues();
					int num = default(int);
					HoverAFXNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<HoverAFXNode>((int)(short)hover.machineID, ref num);
					for (int i = 0; i < num; i++)
					{
						HoverAFXNode hoverAFXNode = array[i];
						if (hoverAFXNode.visibilityComponent.inRange && hoverAFXNode.disabledComponent.enabled)
						{
							hoverCount++;
							int level = hoverAFXNode.levelComponent.level;
							if (level > hoverLevel)
							{
								hoverLevel = level;
							}
						}
					}
				}
				UpdateLoopSoundState(ref hover, hoverCount);
				UpdateSoundParams(ref hover, hover.momentum, hover.power, hoverLevel);
				yield return null;
			}
		}

		private void UpdateLoopSoundState(ref HoverStruct hover, int activeHovers)
		{
			if (!hover.soundPlaying && activeHovers > 0)
			{
				EventManager.get_Instance().PostEvent(hover.loopAudioName, 0, (object)null, hover.audioGO);
				hover.soundPlaying = true;
			}
			else if (hover.soundPlaying && activeHovers == 0)
			{
				EventManager.get_Instance().PostEvent(hover.loopAudioName, 1, (object)null, hover.audioGO);
				hover.soundPlaying = false;
			}
		}

		private void UpdateSoundParams(ref HoverStruct hover, float momentum, float power, int currentLevel)
		{
			if (hover.soundPlaying)
			{
				if (hover.currentLevel != currentLevel)
				{
					EventManager.get_Instance().SetParameter(hover.loopAudioName, "LEVEL", (float)currentLevel, hover.audioGO);
					hover.currentLevel = currentLevel;
				}
				if (Mathf.Abs(hover.lastMomentumValueSet - momentum) > 0.01f)
				{
					EventManager.get_Instance().SetParameter(hover.loopAudioName, "MOMENTUM", momentum, hover.audioGO);
					hover.lastMomentumValueSet = momentum;
				}
				if (Mathf.Abs(hover.lastPowerValueSet - power) > 0.01f)
				{
					EventManager.get_Instance().SetParameter(hover.loopAudioName, "POWER", power, hover.audioGO);
					hover.lastPowerValueSet = power;
				}
				if (Mathf.Abs(hover.lastCameraDistance - hover.cameraDistance) > 10f)
				{
					EventManager.get_Instance().SetParameter(hover.loopAudioName, "Distance", hover.cameraDistance, hover.audioGO);
					hover.lastCameraDistance = hover.cameraDistance;
				}
			}
		}

		public void Ready()
		{
		}
	}
}
