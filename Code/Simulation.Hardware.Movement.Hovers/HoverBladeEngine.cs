using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class HoverBladeEngine : MultiEntityViewsEngine<MachineHoverNode, HoverBladeNode>, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		private struct HoverStruct
		{
			public readonly IMachineFunctionalComponent info;

			public readonly IMachineStunComponent stunComponent;

			public readonly int machineID;

			private IMachineInputComponent _machineInputComponent;

			private float _forceYOffset;

			private float _heightTolerance;

			private float _totalCarryMass;

			private int _enabledItemsCount;

			public Rigidbody rb
			{
				get;
				private set;
			}

			public float massPerBlade
			{
				get;
				private set;
			}

			public float massRatio
			{
				get;
				private set;
			}

			public Vector4 input
			{
				get;
				private set;
			}

			public Vector3 forward
			{
				get;
				private set;
			}

			public Vector3 right
			{
				get;
				private set;
			}

			public Vector3 up
			{
				get;
				private set;
			}

			public float highestYPos
			{
				get;
				private set;
			}

			public float lowestYPos
			{
				get;
				private set;
			}

			public float maxValidHeight
			{
				get;
				private set;
			}

			public float modifiedMaxHoverHeight
			{
				get;
				private set;
			}

			public float lateralDampingValue
			{
				get;
				private set;
			}

			public float mass
			{
				get;
				private set;
			}

			public float accelerationValue
			{
				get;
				private set;
			}

			public float validBladesCount
			{
				get;
				private set;
			}

			public float maxAngularVelocityValue
			{
				get;
				private set;
			}

			public float turningVelocityScaleValue
			{
				get;
				private set;
			}

			public float smallAngleTurningVelocityScaleValue
			{
				get;
				private set;
			}

			public float validGroundScalar
			{
				get;
				private set;
			}

			public float angularDampingValue
			{
				get;
				private set;
			}

			public float turnTorqueValue
			{
				get;
				private set;
			}

			public Vector2 targetHeightRange
			{
				get;
				private set;
			}

			public float maxVerticalVelocityValue
			{
				get;
				private set;
			}

			public float hoverDampingValue
			{
				get;
				private set;
			}

			public Vector3 forceOffset
			{
				get;
				private set;
			}

			public float heightChangeSpeedValue
			{
				get;
				private set;
			}

			public float yAgularVelocity
			{
				get;
				private set;
			}

			public bool changingHoverHeight
			{
				get;
				internal set;
			}

			public float targetHoverHeightPercent
			{
				get;
				internal set;
			}

			public float maxHoverHeight
			{
				get;
				private set;
			}

			public float avgGroundDistance
			{
				get;
				private set;
			}

			public bool legacyControls
			{
				get;
				private set;
			}

			public HoverStruct(MachineHoverNode node, HoverData data, bool legacyControls)
			{
				this = default(HoverStruct);
				machineID = node.get_ID();
				this.legacyControls = legacyControls;
				info = node.functionalComponent;
				stunComponent = node.machineStunComponent;
				rb = node.rigidbodyComponent.rb;
				_machineInputComponent = node.inputComponent;
				lateralDampingValue = data.lateralDamping;
				accelerationValue = data.acceleration;
				maxAngularVelocityValue = data.maxAngularVelocity;
				turningVelocityScaleValue = data.turningScale;
				smallAngleTurningVelocityScaleValue = data.smallAngleTurningScale;
				angularDampingValue = data.angularDamping;
				turnTorqueValue = data.turnTorque;
				maxVerticalVelocityValue = data.maxVerticalVelocity / 11.4f;
				hoverDampingValue = data.hoverDamping;
				heightChangeSpeedValue = data.heightChangeSpeed;
				_forceYOffset = data.forceYOffset;
				maxHoverHeight = data.maxHoverHeight;
				_heightTolerance = data.heightTolerance;
				targetHoverHeightPercent = 0.5f;
				_enabledItemsCount = 0;
			}

			public void RefreshValues(int listCount, HoverBladeNode[] list)
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				_enabledItemsCount = UpdateOnItemsCountChanged(listCount, list);
				if (_enabledItemsCount == 0)
				{
					massPerBlade = 0f;
					return;
				}
				input = _machineInputComponent.analogInput;
				forward = rb.get_transform().get_forward();
				up = rb.get_transform().get_up();
				right = rb.get_transform().get_right();
				Vector3 angularVelocity = rb.get_angularVelocity();
				yAgularVelocity = angularVelocity.y;
				mass = rb.get_mass();
				massPerBlade = mass / (float)_enabledItemsCount;
				massRatio = Mathf.Clamp01(_totalCarryMass / mass);
				forceOffset = up * _forceYOffset;
				modifiedMaxHoverHeight = maxHoverHeight * massRatio;
				float num = Mathf.Lerp(0f, modifiedMaxHoverHeight, targetHoverHeightPercent);
				targetHeightRange = new Vector2(num - _heightTolerance, num + _heightTolerance);
				maxValidHeight = modifiedMaxHoverHeight + _heightTolerance;
				validGroundScalar = 0f;
				validBladesCount = 0f;
				avgGroundDistance = 0f;
			}

			private int UpdateOnItemsCountChanged(int listCount, HoverBladeNode[] list)
			{
				int num = 0;
				_totalCarryMass = 0f;
				highestYPos = 0f;
				lowestYPos = float.MaxValue;
				for (int i = 0; i < listCount; i++)
				{
					HoverBladeNode hoverBladeNode = list[i];
					if (hoverBladeNode.disabledComponent.enabled)
					{
						num++;
						float initialYPos = hoverBladeNode.info.initialYPos;
						if (initialYPos > highestYPos)
						{
							highestYPos = initialYPos;
						}
						if (initialYPos < lowestYPos)
						{
							lowestYPos = initialYPos;
						}
						_totalCarryMass += hoverBladeNode.massComponent.maxCarryMass;
					}
				}
				return num;
			}

			internal void UpdateValidItemsCount(float groundScalar, float distance)
			{
				if (groundScalar > 0f)
				{
					validGroundScalar += groundScalar;
					validBladesCount += 1f;
				}
				avgGroundDistance += distance / (float)_enabledItemsCount;
			}
		}

		private const float GRAVITY_MULTIPLIER = 2f;

		private const float MAX_SPEED_THRESHOLD = 0.25f;

		private const float NO_INPUT_THRESHOLD = 0.1f;

		private const float HOVER_DAMPING_MULTIPLIER = 2f;

		private const float MIN_DAMPING_RATIO = 0.1f;

		private const float FULL_SPEED_ROTATION_THRESHOLD_MULTIPLIER = 4f;

		private const float MIN_ROTATION_THRESHOLD_MULTIPLIER = 0.02f;

		private HoverData _data;

		private readonly Dictionary<int, ITaskRoutine> _tasks;

		[Inject]
		internal PlayerStrafeDirectionManager strafeDirectionManager
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public HoverBladeEngine()
		{
			_tasks = new Dictionary<int, ITaskRoutine>();
		}

		public void OnDependenciesInjected()
		{
			ILoadMovementStatsRequest loadMovementStatsRequest = serviceFactory.Create<ILoadMovementStatsRequest>();
			loadMovementStatsRequest.SetAnswer(new ServiceAnswer<MovementStats>(delegate(MovementStats statsData)
			{
				_data = (statsData.categoryData[ItemCategory.Hover] as HoverData);
			})).Execute();
		}

		protected override void Add(MachineHoverNode node)
		{
			if (node.owner.ownedByMe || node.owner.ownedByAi)
			{
				ITaskRoutine val = TaskRunner.get_Instance().AllocateNewTaskRoutine();
				bool legacyControls = !strafeDirectionManager.strafingEnabled || node.owner.ownedByAi;
				val.SetEnumerator(Update(new HoverStruct(node, _data, legacyControls))).SetScheduler(StandardSchedulers.get_physicScheduler());
				val.Start((Action<PausableTaskException>)null, (Action)null);
				_tasks.Add(node.get_ID(), val);
			}
		}

		protected override void Remove(MachineHoverNode node)
		{
			if (node.owner.ownedByMe || node.owner.ownedByAi)
			{
				ITaskRoutine val = _tasks[node.get_ID()];
				val.Stop();
				_tasks.Remove(node.get_ID());
			}
		}

		protected override void Add(HoverBladeNode entityView)
		{
			entityView.disabledComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnHoverEnabledChange);
		}

		protected override void Remove(HoverBladeNode entityView)
		{
			entityView.disabledComponent.isPartEnabled.StopNotify((Action<int, bool>)OnHoverEnabledChange);
		}

		private void OnHoverEnabledChange(int eid, bool enabled)
		{
			HoverBladeNode hoverBladeNode = entityViewsDB.QueryEntityView<HoverBladeNode>(eid);
			if (hoverBladeNode != null)
			{
				hoverBladeNode.info.resetLastPosUpdates = 2;
			}
		}

		private IEnumerator Update(HoverStruct hover)
		{
			while (true)
			{
				if (hover.info.functionalsEnabled && !hover.stunComponent.stunned)
				{
					int num = default(int);
					HoverBladeNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<HoverBladeNode>(hover.machineID, ref num);
					hover.RefreshValues(num, array);
					int num2 = 0;
					for (int i = 0; i < num; i++)
					{
						HoverBladeNode bladeNode = array[i];
						if (bladeNode.disabledComponent.enabled)
						{
							if (bladeNode.info.resetLastPosUpdates > 0)
							{
								bladeNode.info.resetLastPosUpdates--;
								bladeNode.info.lastPos = bladeNode.info.forcePointTransform.get_position();
							}
							else
							{
								num2++;
								UpdateDistanceToGround(ref bladeNode, ref hover);
								Hover(ref hover, ref bladeNode);
							}
						}
					}
					if (num2 > 0)
					{
						HoverHeight(ref hover);
						LateralMovement(ref hover);
						LateralDamping(ref hover);
						Turning(ref hover);
						AngularDamping(ref hover);
					}
				}
				yield return null;
			}
		}

		private void UpdateDistanceToGround(ref HoverBladeNode bladeNode, ref HoverStruct hover)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			IHoverInfoComponent info = bladeNode.info;
			Transform forcePointTransform = bladeNode.info.forcePointTransform;
			float num = hover.highestYPos - hover.lowestYPos;
			float num2 = hover.highestYPos - info.initialYPos;
			RaycastHit raycastHit = default(RaycastHit);
			bool flag = Physics.Raycast(forcePointTransform.get_position() + Vector3.get_up() * num2, Vector3.get_down(), ref raycastHit, hover.maxValidHeight * 2f + num, GameLayers.ENVIRONMENT_LAYER_MASK);
			info.distanceToGround = ((!flag) ? float.MaxValue : (raycastHit.get_distance() - num));
			info.raycastHit = raycastHit;
			float maxValidHeight = hover.maxValidHeight;
			info.validGroundScalar = 1f - Mathf.InverseLerp(maxValidHeight, maxValidHeight * 2f, info.distanceToGround);
			hover.UpdateValidItemsCount(info.validGroundScalar, info.distanceToGround);
		}

		private void HoverHeight(ref HoverStruct hover)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			Vector4 input = hover.input;
			if (Mathf.Abs(input.y) > 0.1f && hover.avgGroundDistance <= hover.maxValidHeight)
			{
				hover.targetHoverHeightPercent += input.y * hover.heightChangeSpeedValue / hover.maxHoverHeight * Time.get_fixedDeltaTime() * hover.massRatio;
				if (hover.targetHoverHeightPercent > 0f && hover.targetHoverHeightPercent < 1f)
				{
					hover.changingHoverHeight = true;
					float num = input.y * hover.heightChangeSpeedValue * hover.massRatio;
					Vector3 velocity = hover.rb.get_velocity();
					float y = velocity.y;
					float num2 = num - y;
					Vector3 gravity = Physics.get_gravity();
					Vector3 val = (num2 - gravity.y * Time.get_fixedDeltaTime()) * Vector3.get_up();
					hover.rb.AddForce(val, 2);
				}
				else if (hover.changingHoverHeight)
				{
					Vector3 velocity2 = hover.rb.get_velocity();
					float y2 = velocity2.y;
					Vector3 val2 = (0f - y2) * Vector3.get_up();
					hover.rb.AddForce(val2, 2);
					hover.changingHoverHeight = false;
				}
				hover.targetHoverHeightPercent = Mathf.Clamp01(hover.targetHoverHeightPercent);
			}
			else if (hover.changingHoverHeight)
			{
				Vector3 velocity3 = hover.rb.get_velocity();
				float y3 = velocity3.y;
				Vector3 val3 = (0f - y3) * Vector3.get_up();
				hover.rb.AddForce(val3, 2);
				hover.changingHoverHeight = false;
			}
		}

		private void Hover(ref HoverStruct hover, ref HoverBladeNode node)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			float num = HoverForce(ref hover, ref node);
			if (!hover.changingHoverHeight)
			{
				num += HoverDampingForce(ref hover, ref node);
			}
			Vector3 val = num * hover.massPerBlade * Physics.get_gravity();
			hover.rb.AddForceAtPosition(val, node.info.forcePointTransform.get_position() + hover.forceOffset);
			node.info.lastPos = node.info.forcePointTransform.get_position();
		}

		private float HoverDampingForce(ref HoverStruct hover, ref HoverBladeNode bladeNode)
		{
			float hoverDampingRatio = GetHoverDampingRatio(ref hover, ref bladeNode);
			return hover.hoverDampingValue * hoverDampingRatio;
		}

		private float GetHoverDampingRatio(ref HoverStruct hover, ref HoverBladeNode bladeNode)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			IHoverInfoComponent info = bladeNode.info;
			float num = 1f - Mathf.InverseLerp(hover.modifiedMaxHoverHeight, hover.modifiedMaxHoverHeight * 2f, info.distanceToGround);
			num = Mathf.Max(num, 0.1f);
			Vector3 position = info.forcePointTransform.get_position();
			float num2;
			if (info.resetLastPosUpdates > 0)
			{
				num2 = 0f;
			}
			else
			{
				float y = position.y;
				Vector3 lastPos = info.lastPos;
				num2 = (y - lastPos.y) / Time.get_fixedDeltaTime();
			}
			num *= num2 / hover.maxVerticalVelocityValue;
			if (num < 0f)
			{
				num *= 2f;
			}
			return num;
		}

		private float HoverForce(ref HoverStruct hover, ref HoverBladeNode node)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			float distanceToGround = node.info.distanceToGround;
			Vector2 targetHeightRange = hover.targetHeightRange;
			float x = targetHeightRange.x;
			float y = targetHeightRange.y;
			float num = 0f;
			if (distanceToGround < x)
			{
				num = 2f - distanceToGround / x;
			}
			else if (distanceToGround < y)
			{
				num = (y - distanceToGround) / (y - x);
			}
			else if (distanceToGround < hover.maxHoverHeight)
			{
				num = (hover.maxHoverHeight - distanceToGround) / (hover.maxHoverHeight - y) - 1f;
			}
			return -2f * num;
		}

		private void LateralMovement(ref HoverStruct hover)
		{
			if (hover.validBladesCount > 0f)
			{
				if (hover.legacyControls)
				{
					ApplyLegacyLateralMovement(ref hover);
				}
				else
				{
					ApplyNewLateralMovement(ref hover);
				}
			}
		}

		private void ApplyLegacyLateralMovement(ref HoverStruct hover)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			Vector4 input = hover.input;
			if (Mathf.Abs(input.z) > 0.1f)
			{
				ApplyLateralForce(ref hover, hover.forward * input.z);
			}
			if (Mathf.Abs(input.w) > 0.1f)
			{
				ApplyLateralForce(ref hover, -hover.right * input.w);
			}
		}

		private void ApplyNewLateralMovement(ref HoverStruct hover)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			Vector4 input = hover.input;
			if (Mathf.Abs(input.z) > 0.1f)
			{
				ApplyLateralForce(ref hover, strafeDirectionManager.forwardMovementDirection * input.z);
			}
			if (Mathf.Abs(input.x) > 0.1f)
			{
				ApplyLateralForce(ref hover, strafeDirectionManager.rightMovementDirection * input.x);
			}
		}

		private void ApplyLateralForce(ref HoverStruct hover, Vector3 direction)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = direction * (hover.accelerationValue * hover.massRatio * hover.massPerBlade * hover.validBladesCount);
			hover.rb.AddForce(val);
		}

		private void LateralDamping(ref HoverStruct hover)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Vector4 input = hover.input;
			bool applyForwardDamping = Mathf.Abs(input.z) < 0.1f;
			float num = (!hover.legacyControls) ? input.x : input.w;
			bool applySidewaydDamping = Mathf.Abs(num) < 0.1f;
			LateralDamping(ref hover, applyForwardDamping, applySidewaydDamping);
		}

		private void LateralDamping(ref HoverStruct hover, bool applyForwardDamping, bool applySidewaydDamping)
		{
			if (hover.legacyControls)
			{
				ApplyLegacyLateralDamping(ref hover, applyForwardDamping, applySidewaydDamping);
			}
			else
			{
				ApplyLateralDamping(ref hover, applyForwardDamping, applySidewaydDamping);
			}
		}

		private void ApplyLateralDamping(ref HoverStruct hover, bool applyForwardDamping, bool applySidewaydDamping)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			if (applyForwardDamping || applySidewaydDamping)
			{
				Rigidbody rb = hover.rb;
				Vector3 velocity = rb.get_velocity();
				float x = velocity.x;
				Vector3 velocity2 = rb.get_velocity();
				Vector3 val = default(Vector3);
				val._002Ector(x, 0f, velocity2.z);
				float magnitude = val.get_magnitude();
				float num = hover.lateralDampingValue * hover.mass;
				if (applyForwardDamping)
				{
					float num2 = Vector3.Dot(strafeDirectionManager.forwardMovementDirection, val) * magnitude;
					Vector3 val2 = -strafeDirectionManager.forwardMovementDirection * (num2 * num);
					rb.AddForce(val2);
				}
				if (applySidewaydDamping)
				{
					float num3 = Vector3.Dot(strafeDirectionManager.rightMovementDirection, val) * magnitude;
					Vector3 val3 = -strafeDirectionManager.rightMovementDirection * (num3 * num);
					rb.AddForce(val3);
				}
			}
		}

		private void ApplyLegacyLateralDamping(ref HoverStruct hover, bool applyForwardDamping, bool applySidewaydDamping)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			if (applyForwardDamping || applySidewaydDamping)
			{
				Rigidbody rb = hover.rb;
				Vector3 forward = hover.forward;
				Vector3 right = hover.right;
				Vector3 velocity = rb.get_velocity();
				float x = velocity.x;
				Vector3 velocity2 = rb.get_velocity();
				Vector3 val = default(Vector3);
				val._002Ector(x, 0f, velocity2.z);
				float magnitude = val.get_magnitude();
				float num = hover.lateralDampingValue * hover.mass;
				if (applyForwardDamping)
				{
					float num2 = Vector3.Dot(forward, val) * magnitude;
					Vector3 val2 = -forward * (num2 * num);
					rb.AddForce(val2);
				}
				if (applySidewaydDamping)
				{
					float num3 = Vector3.Dot(right, val) * magnitude;
					Vector3 val3 = -right * (num3 * num);
					rb.AddForce(val3);
				}
			}
		}

		private void Turning(ref HoverStruct hover)
		{
			if (hover.validGroundScalar > 0f)
			{
				if (hover.legacyControls)
				{
					ApplyLegacyTurning(ref hover);
				}
				else
				{
					ApplyTurning(ref hover);
				}
			}
		}

		private void ApplyTurning(ref HoverStruct hover)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (strafeDirectionManager.IsAngleToStraightGreaterThanThreshold(0.02f))
			{
				Rigidbody rb = hover.rb;
				float num = 0f - Mathf.Sign(strafeDirectionManager.angleToStraight);
				float turningRatio = GetTurningRatio(hover.yAgularVelocity, num, hover.maxAngularVelocityValue);
				Vector3 val = hover.up * (turningRatio * hover.validGroundScalar * num * hover.turnTorqueValue * hover.massPerBlade * hover.massRatio);
				if (strafeDirectionManager.IsAngleToStraightGreaterThanThreshold(4f))
				{
					rb.AddTorque(val);
				}
				else if (strafeDirectionManager.IsAngleToStraightGreaterThanThreshold())
				{
					rb.AddTorque(val * hover.turningVelocityScaleValue);
				}
				else
				{
					rb.AddTorque(val * hover.smallAngleTurningVelocityScaleValue);
				}
			}
		}

		private void ApplyLegacyTurning(ref HoverStruct hover)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			Vector4 input = hover.input;
			float x = input.x;
			if (Mathf.Abs(x) > 0.1f)
			{
				float turningRatio = GetTurningRatio(hover.yAgularVelocity, x, hover.maxAngularVelocityValue);
				hover.rb.AddTorque(hover.up * (turningRatio * hover.validGroundScalar * x * hover.turnTorqueValue * hover.massPerBlade * hover.massRatio));
			}
		}

		private float GetTurningRatio(float angularY, float turningInput, float maxAngularVelocity)
		{
			if (Mathf.Abs(angularY) < maxAngularVelocity || angularY * turningInput < 0f)
			{
				return 1f;
			}
			return 1f - Mathf.Clamp01((Mathf.Abs(angularY) - maxAngularVelocity) / (maxAngularVelocity * 0.25f));
		}

		private void AngularDamping(ref HoverStruct hover)
		{
			if (hover.legacyControls)
			{
				ApplyLegacyAngularDamping(ref hover);
			}
			else
			{
				ApplyAngularDamping(ref hover);
			}
		}

		private void ApplyLegacyAngularDamping(ref HoverStruct hover)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			Vector4 input = hover.input;
			bool condition = Mathf.Abs(input.x) > 0.1f;
			Vector4 input2 = hover.input;
			float angularDampingRatio = GetAngularDampingRatio(condition, input2.x, hover.yAgularVelocity, hover.maxAngularVelocityValue);
			Vector3 up = hover.up;
			Vector3 val = up * (Mathf.Sign(up.y) * (0f - angularDampingRatio) * hover.angularDampingValue * hover.mass * hover.massRatio);
			hover.rb.AddTorque(val);
		}

		private void ApplyAngularDamping(ref HoverStruct hover)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			float angularDampingRatio = GetAngularDampingRatio(strafeDirectionManager.IsAngleToStraightGreaterThanThreshold(), 0f - strafeDirectionManager.angleToStraight, hover.yAgularVelocity, hover.maxAngularVelocityValue);
			Vector3 up = hover.up;
			Vector3 val = up * (Mathf.Sign(up.y) * (0f - angularDampingRatio) * hover.angularDampingValue * hover.mass * hover.massRatio);
			hover.rb.AddTorque(val);
		}

		private float GetAngularDampingRatio(bool condition, float direction, float yAngular, float maxAngularVelocityValue)
		{
			float num = Mathf.Sign(direction);
			if (condition && num == Mathf.Sign(yAngular))
			{
				float num2 = num * yAngular - maxAngularVelocityValue;
				return Mathf.Max(num2, 0f) * num / maxAngularVelocityValue;
			}
			return yAngular / maxAngularVelocityValue;
		}

		public void Ready()
		{
		}
	}
}
