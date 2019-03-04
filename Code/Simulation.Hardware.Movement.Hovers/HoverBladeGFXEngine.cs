using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverBladeGFXEngine : SingleEntityViewEngine<MachineHoverNode>, IQueryingEntityViewEngine, IEngine
	{
		private struct HoverStruct
		{
			public readonly GameObject hoverFX;

			public readonly IMachineFunctionalComponent info;

			public readonly IMachineStunComponent stunComponent;

			public readonly ParticleSystem _1;

			public readonly ParticleSystem _2;

			public float velsqrMagnitude;

			public float angsqrMagnitude;

			public readonly int machineID;

			private readonly Rigidbody rb;

			public HoverStruct(MachineHoverNode node, GameObject hoverFX)
			{
				this = default(HoverStruct);
				this.hoverFX = hoverFX;
				stunComponent = node.machineStunComponent;
				info = node.functionalComponent;
				rb = node.rigidbodyComponent.rb;
				ParticleSystem[] componentsInChildren = hoverFX.GetComponentsInChildren<ParticleSystem>();
				_1 = componentsInChildren[0];
				_2 = componentsInChildren[1];
				machineID = node.get_ID();
			}

			public void RefreshValues()
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				Vector3 velocity = rb.get_velocity();
				velsqrMagnitude = velocity.get_sqrMagnitude();
				Vector3 angularVelocity = rb.get_angularVelocity();
				angsqrMagnitude = angularVelocity.get_sqrMagnitude();
			}
		}

		private const float MAX_HOVER_HEIGHT_GFX = 7f;

		private readonly Dictionary<int, ITaskRoutine> _tasks;

		private Quaternion _initialSpinRot;

		private Func<GameObject> _onEffectAllocation;

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool goPool
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public HoverBladeGFXEngine()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			_tasks = new Dictionary<int, ITaskRoutine>();
			_initialSpinRot = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
			_onEffectAllocation = BuildEffect;
		}

		private GameObject BuildEffect()
		{
			GameObject val = gameObjectFactory.Build("T5_Hover_Ground_Dust");
			val.set_name("T5_Hover_Ground_Dust");
			val.SetActive(false);
			return val;
		}

		protected override void Add(MachineHoverNode node)
		{
			ITaskRoutine val = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			GameObject val2 = goPool.Use("T5_Hover_Ground_Dust", _onEffectAllocation);
			node.effectsComponent.dustEffect = val2;
			val.SetEnumerator(Update(new HoverStruct(node, val2)));
			val.Start((Action<PausableTaskException>)null, (Action)null);
			_tasks.Add(node.get_ID(), val);
		}

		protected override void Remove(MachineHoverNode node)
		{
			GameObject dustEffect = node.effectsComponent.dustEffect;
			dustEffect.SetActive(false);
			goPool.Recycle(dustEffect, "T5_Hover_Ground_Dust");
			ITaskRoutine val = _tasks[node.get_ID()];
			val.Stop();
			_tasks.Remove(node.get_ID());
		}

		private IEnumerator Update(HoverStruct hover)
		{
			while (true)
			{
				int itemsCount = 0;
				Vector3 averagePosition = default(Vector3);
				if (hover.info.functionalsEnabled && !hover.stunComponent.stunned)
				{
					hover.RefreshValues();
					int num = default(int);
					HoverGFXNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<HoverGFXNode>(hover.machineID, ref num);
					for (int i = 0; i < num; i++)
					{
						HoverGFXNode node = array[i];
						if (node.visibilityComponent.inRange && node.disabledComponent.enabled)
						{
							Vector3 val = ProcessOrienatationOffset(ref node);
							SpinBlade(ref node, ref hover);
							averagePosition += val;
							itemsCount++;
						}
					}
				}
				UpdateDustEffectState(itemsCount, ref hover);
				if (itemsCount > 0)
				{
					SetupDustEffects(averagePosition / (float)itemsCount, ref hover);
				}
				yield return null;
			}
		}

		private Vector3 ProcessOrienatationOffset(ref HoverGFXNode node)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			IHoverGFXComponent gfx = node.gfx;
			Vector3 position = node.info.forcePointTransform.get_position();
			Transform orientation = gfx.orientation;
			gfx.previousThrustPost = Vector3.Lerp(gfx.previousThrustPost, position, Time.get_deltaTime());
			Vector3 val = gfx.previousThrustPost - position;
			Vector3 val2 = orientation.get_position() + val + Vector3.get_down() * 10f;
			orientation.LookAt(val2);
			return position;
		}

		private void SpinBlade(ref HoverGFXNode node, ref HoverStruct hover)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			IHoverGFXComponent gfx = node.gfx;
			float num = hover.velsqrMagnitude + hover.angsqrMagnitude + 100f;
			float num2 = (gfx.minSpinVel + num * gfx.spinSpeedMult) * Time.get_deltaTime();
			gfx.currentSpinRotation += num2;
			Quaternion val = Quaternion.AngleAxis(gfx.currentSpinRotation, gfx.spinAxisV);
			gfx.spinBlade.set_localRotation(val * _initialSpinRot);
		}

		private void SetupDustEffects(Vector3 averagePosition, ref HoverStruct hover)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = hover.hoverFX.get_transform();
			float num = 7f;
			RaycastHit val = default(RaycastHit);
			Physics.Raycast(averagePosition, Vector3.get_down(), ref val, num * 2f, GameLayers.ENVIRONMENT_LAYER_MASK);
			Vector3 point = val.get_point();
			transform.set_position(point);
			transform.LookAt(transform.get_position() - val.get_normal());
			float num2 = Vector3.Distance(averagePosition, point);
			num2 = Mathf.Clamp01(1f - 1f / num * num2);
			ControlParticle(hover._1, num2);
			ControlParticle(hover._2, num2);
		}

		private void UpdateDustEffectState(int itemsCount, ref HoverStruct hover)
		{
			Transform transform = hover.hoverFX.get_transform();
			if (transform.get_gameObject().get_activeSelf() && itemsCount == 0)
			{
				transform.get_gameObject().SetActive(false);
			}
			else if (!transform.get_gameObject().get_activeSelf() && itemsCount > 0)
			{
				transform.get_gameObject().SetActive(true);
			}
		}

		private static void ControlParticle(ParticleSystem partSys, float dist)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Color startColor = partSys.get_startColor();
			startColor.a = dist;
			partSys.set_startColor(startColor);
		}

		public void Ready()
		{
		}
	}
}
