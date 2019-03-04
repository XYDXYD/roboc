using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal class TopSpeedEngine : SingleEntityViewEngine<TopSpeedNode>, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		public const float UNIT_MULTIPLIER = 11.4f;

		private Dictionary<int, FasterList<TopSpeedNode>> _nodesPerMachine = new Dictionary<int, FasterList<TopSpeedNode>>(20);

		private float _lerpValue = 1f;

		public IEntityViewsDB entityViewsDB
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

		public void OnDependenciesInjected()
		{
			ILoadMovementStatsRequest loadMovementStatsRequest = serviceFactory.Create<ILoadMovementStatsRequest>();
			loadMovementStatsRequest.SetAnswer(new ServiceAnswer<MovementStats>(delegate(MovementStats statsData)
			{
				_lerpValue = statsData.lerpValue;
			})).Execute();
		}

		public void Ready()
		{
			TaskRunner.get_Instance().AllocateNewTaskRoutine().SetScheduler(StandardSchedulers.get_physicScheduler())
				.SetEnumerator(FixedUpdate())
				.Start((Action<PausableTaskException>)null, (Action)null);
		}

		protected override void Add(TopSpeedNode node)
		{
			if (!node.partDisabledComponent.isPartDisabled.get_value())
			{
				AddNode(node);
			}
			node.partDisabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)OnMovementPartDisabled);
		}

		protected override void Remove(TopSpeedNode node)
		{
			node.partDisabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnMovementPartDisabled);
			RemoveNode(node);
		}

		private void OnMovementPartDisabled(int id, bool disabled)
		{
			TopSpeedNode node = default(TopSpeedNode);
			if (entityViewsDB.TryQueryEntityView<TopSpeedNode>(id, ref node))
			{
				if (disabled)
				{
					RemoveNode(node);
				}
				else
				{
					AddNode(node);
				}
			}
		}

		private void RemoveNode(TopSpeedNode node)
		{
			int machineId = node.ownerComponent.machineId;
			if (_nodesPerMachine.TryGetValue(node.ownerComponent.machineId, out FasterList<TopSpeedNode> value))
			{
				value.Remove(node);
			}
		}

		private void AddNode(TopSpeedNode node)
		{
			int machineId = node.ownerComponent.machineId;
			if (!_nodesPerMachine.TryGetValue(node.ownerComponent.machineId, out FasterList<TopSpeedNode> value))
			{
				value = new FasterList<TopSpeedNode>();
				_nodesPerMachine[machineId] = value;
			}
			value.Add(node);
		}

		private IEnumerator FixedUpdate()
		{
			while (true)
			{
				FasterReadOnlyList<MachineTopSpeedNode> machineNodes = entityViewsDB.QueryEntityViews<MachineTopSpeedNode>();
				for (int i = 0; i < machineNodes.get_Count(); i++)
				{
					MachineTopSpeedNode machineTopSpeedNode = machineNodes.get_Item(i);
					if (!_nodesPerMachine.TryGetValue(machineTopSpeedNode.get_ID(), out FasterList<TopSpeedNode> value) || value.get_Count() == 0)
					{
						continue;
					}
					Init(value, machineTopSpeedNode);
					if (machineTopSpeedNode.ownerComponent.ownedByMe)
					{
						bool flag = MustLimitMaxSpeed(value, dontCheckIsValid: false);
						UpdateMaxSpeedPerSide(machineTopSpeedNode, value, flag, dontCheckIsValid: false);
						float num;
						if (flag)
						{
							num = LimitMaxSpeed(machineTopSpeedNode);
							machineTopSpeedNode.topSpeedComponent.limitTimestamp = Time.get_timeSinceLevelLoad();
						}
						else
						{
							num = GetMaxSpeed(machineTopSpeedNode);
							if (num > 0f)
							{
								LimitMaxSpeed(machineTopSpeedNode, num);
							}
						}
						UpdateNodes(value, num, modifier: false);
					}
					else if (machineTopSpeedNode.ownerComponent.ownedByAi)
					{
						LimitMaxSpeed(machineTopSpeedNode);
					}
				}
				yield return null;
			}
		}

		private void LimitMaxSpeed(MachineTopSpeedNode machineNode, float maxSpeed)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp01((Time.get_timeSinceLevelLoad() - machineNode.topSpeedComponent.limitTimestamp) / 5f);
			Rigidbody rb = machineNode.rigidBodyComponent.rb;
			IMachineTopSpeedComponent topSpeedComponent = machineNode.topSpeedComponent;
			Vector3 velocity = rb.get_velocity();
			if (velocity.y < 0f)
			{
				velocity.y = 0f;
			}
			Vector3 val = rb.get_transform().InverseTransformVector(velocity);
			float magnitude = val.get_magnitude();
			Vector3 normalized = val.get_normalized();
			LimitMaxSpeed(machineNode, magnitude, normalized, Mathf.Lerp(maxSpeed, maxSpeed * 3f, 1f - num));
		}

		private void Init(FasterList<TopSpeedNode> nodes, MachineTopSpeedNode machineNode)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			if (machineNode.topSpeedComponent.initNeeded && nodes.get_Count() > 0)
			{
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					TopSpeedNode topSpeedNode = nodes.get_Item(i);
					Vector3 val = topSpeedNode.speedComponent.positiveAxisMaxSpeed;
					val.x *= topSpeedNode.statsComponent.horizontalMaxSpeed;
					val.y *= topSpeedNode.statsComponent.verticalMaxSpeed;
					val.z *= topSpeedNode.statsComponent.horizontalMaxSpeed;
					topSpeedNode.speedComponent.positiveAxisMaxSpeed = val / 11.4f;
					val = topSpeedNode.speedComponent.negativeAxisMaxSpeed;
					val.x *= topSpeedNode.statsComponent.horizontalMaxSpeed;
					val.y *= topSpeedNode.statsComponent.verticalMaxSpeed;
					val.z *= topSpeedNode.statsComponent.horizontalMaxSpeed;
					topSpeedNode.speedComponent.negativeAxisMaxSpeed = val / 11.4f;
				}
				bool limitMaxSpeed = MustLimitMaxSpeed(nodes, dontCheckIsValid: true);
				UpdateMaxSpeedPerSide(machineNode, nodes, limitMaxSpeed, dontCheckIsValid: true);
				float maxSpeed = GetMaxSpeed(machineNode);
				UpdateNodes(nodes, maxSpeed, modifier: true);
				machineNode.topSpeedComponent.initNeeded = false;
			}
		}

		private float LimitMaxSpeed(MachineTopSpeedNode machineNode)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = machineNode.rigidBodyComponent.rb;
			IMachineTopSpeedComponent topSpeedComponent = machineNode.topSpeedComponent;
			Vector3 velocity = rb.get_velocity();
			Vector3 val = rb.get_transform().InverseTransformVector(velocity);
			float magnitude = val.get_magnitude();
			Vector3 normalized = val.get_normalized();
			Vector3 val2 = default(Vector3);
			val2._002Ector(normalized.x, 0f, normalized.z);
			val2.Normalize();
			float num = Vector3.Angle(val2, Vector3.get_forward());
			float num2 = 0f;
			if (num >= 90f && val2.x >= 0f)
			{
				float num3 = (num - 90f) / 90f;
				Vector3 positiveAxisMaxSpeed = topSpeedComponent.positiveAxisMaxSpeed;
				float x = positiveAxisMaxSpeed.x;
				Vector3 negativeAxisMaxSpeed = topSpeedComponent.negativeAxisMaxSpeed;
				num2 = Mathf.Lerp(x, negativeAxisMaxSpeed.z, num3);
			}
			else if (num >= 0f && val2.x >= 0f)
			{
				float num4 = num / 90f;
				Vector3 positiveAxisMaxSpeed2 = topSpeedComponent.positiveAxisMaxSpeed;
				float z = positiveAxisMaxSpeed2.z;
				Vector3 positiveAxisMaxSpeed3 = topSpeedComponent.positiveAxisMaxSpeed;
				num2 = Mathf.Lerp(z, positiveAxisMaxSpeed3.x, num4);
			}
			else if (num >= 90f)
			{
				float num5 = (num - 90f) / 90f;
				Vector3 negativeAxisMaxSpeed2 = topSpeedComponent.negativeAxisMaxSpeed;
				float x2 = negativeAxisMaxSpeed2.x;
				Vector3 negativeAxisMaxSpeed3 = topSpeedComponent.negativeAxisMaxSpeed;
				num2 = Mathf.Lerp(x2, negativeAxisMaxSpeed3.z, num5);
			}
			else if (num >= 0f)
			{
				float num6 = num / 90f;
				Vector3 positiveAxisMaxSpeed4 = topSpeedComponent.positiveAxisMaxSpeed;
				float z2 = positiveAxisMaxSpeed4.z;
				Vector3 negativeAxisMaxSpeed4 = topSpeedComponent.negativeAxisMaxSpeed;
				num2 = Mathf.Lerp(z2, negativeAxisMaxSpeed4.x, num6);
			}
			if (normalized.y > 0f)
			{
				float num7 = num2;
				Vector3 positiveAxisMaxSpeed5 = topSpeedComponent.positiveAxisMaxSpeed;
				num2 = Mathf.Lerp(num7, positiveAxisMaxSpeed5.y, normalized.y);
			}
			else if (normalized.y < 0f)
			{
				float num8 = num2;
				Vector3 negativeAxisMaxSpeed5 = topSpeedComponent.negativeAxisMaxSpeed;
				num2 = Mathf.Lerp(num8, negativeAxisMaxSpeed5.y, 0f - normalized.y);
			}
			LimitMaxSpeed(machineNode, magnitude, normalized, num2);
			return num2;
		}

		private void LimitMaxSpeed(MachineTopSpeedNode machineNode, float currentSpeed, Vector3 normLocalVelocity, float maxSpeed)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = machineNode.rigidBodyComponent.rb;
			if (rb != null)
			{
				IMachineTopSpeedComponent topSpeedComponent = machineNode.topSpeedComponent;
				maxSpeed = Mathf.Max(maxSpeed - topSpeedComponent.prevSpeedDelta, 0f);
				float num = currentSpeed - maxSpeed;
				if (num > 0f)
				{
					Vector3 val = normLocalVelocity * -1f * num;
					Vector3 val2 = rb.get_transform().TransformVector(val);
					rb.AddForce(val2, 2);
				}
				topSpeedComponent.prevSpeedDelta = Mathf.Clamp(num, 0f, maxSpeed * 0.2f);
			}
		}

		private void UpdateNodes(FasterList<TopSpeedNode> nodes, float maxSpeed, bool modifier)
		{
			for (int i = 0; i < nodes.get_Count(); i++)
			{
				TopSpeedNode topSpeedNode = nodes.get_Item(i);
				if (!topSpeedNode.validMovComponent.affectsMaxSpeed || modifier)
				{
					topSpeedNode.speedComponent.maxSpeed = maxSpeed;
				}
			}
		}

		private void UpdateMaxSpeedPerSide(MachineTopSpeedNode machineNode, FasterList<TopSpeedNode> nodes, bool limitMaxSpeed, bool dontCheckIsValid)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			IMachineTopSpeedComponent topSpeedComponent = machineNode.topSpeedComponent;
			Vector3 zero = Vector3.get_zero();
			Vector3 zero2 = Vector3.get_zero();
			Vector3 val = Vector3.get_one() * float.MaxValue;
			Vector3 val2 = Vector3.get_one() * float.MaxValue;
			Vector3 zero3 = Vector3.get_zero();
			Vector3 zero4 = Vector3.get_zero();
			Vector3 zero5 = Vector3.get_zero();
			Vector3 zero6 = Vector3.get_zero();
			float num = 1f;
			for (int i = 0; i < nodes.get_Count(); i++)
			{
				TopSpeedNode topSpeedNode = nodes.get_Item(i);
				uint cpuRating = topSpeedNode.cpuComponent.cpuRating;
				bool flag = topSpeedNode.validMovComponent.isValid || dontCheckIsValid;
				bool flag2 = topSpeedNode.validMovComponent.affectsMaxSpeed || !limitMaxSpeed;
				if (flag)
				{
					for (int j = 0; j < 3; j++)
					{
						Vector3 positiveAxisMaxSpeed = topSpeedNode.speedComponent.positiveAxisMaxSpeed;
						float num2 = positiveAxisMaxSpeed.get_Item(j);
						if (num2 > 0f)
						{
							if (flag2)
							{
								int num3;
								zero3.set_Item(num3 = j, zero3.get_Item(num3) + (float)(double)cpuRating);
								int num4;
								zero.set_Item(num4 = j, zero.get_Item(num4) + num2 * (float)(double)cpuRating * topSpeedNode.speedModifier.minItemsModifier);
								if (num2 < val.get_Item(j))
								{
									val.set_Item(j, num2 * topSpeedNode.speedModifier.minItemsModifier);
								}
							}
							int num5;
							zero5.set_Item(num5 = j, zero5.get_Item(num5) + topSpeedNode.statsComponent.speedBoost);
						}
						Vector3 negativeAxisMaxSpeed = topSpeedNode.speedComponent.negativeAxisMaxSpeed;
						float num6 = negativeAxisMaxSpeed.get_Item(j);
						if (!(num6 > 0f))
						{
							continue;
						}
						if (flag2)
						{
							int num7;
							zero4.set_Item(num7 = j, zero4.get_Item(num7) + (float)(double)cpuRating);
							int num8;
							zero2.set_Item(num8 = j, zero2.get_Item(num8) + num6 * (float)(double)cpuRating * topSpeedNode.speedModifier.minItemsModifier);
							if (num6 < val2.get_Item(j))
							{
								val2.set_Item(j, num6 * topSpeedNode.speedModifier.minItemsModifier);
							}
						}
						int num9;
						zero6.set_Item(num9 = j, zero6.get_Item(num9) + topSpeedNode.statsComponent.speedBoost);
					}
				}
				if (num == 1f)
				{
					num = topSpeedNode.speedModifier.speedModifier;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				if (zero3.get_Item(k) != 0f)
				{
					int num10;
					zero.set_Item(num10 = k, zero.get_Item(num10) / zero3.get_Item(k));
					zero.set_Item(k, Mathf.Lerp(val.get_Item(k), zero.get_Item(k), _lerpValue));
					int num11;
					zero.set_Item(num11 = k, zero.get_Item(num11) * (1f + zero5.get_Item(k)));
					int num12;
					zero.set_Item(num12 = k, zero.get_Item(num12) * num);
				}
				if (zero4.get_Item(k) != 0f)
				{
					int num13;
					zero2.set_Item(num13 = k, zero2.get_Item(num13) / zero4.get_Item(k));
					zero2.set_Item(k, Mathf.Lerp(val2.get_Item(k), zero2.get_Item(k), _lerpValue));
					int num14;
					zero2.set_Item(num14 = k, zero2.get_Item(num14) * (1f + zero6.get_Item(k)));
					int num15;
					zero2.set_Item(num15 = k, zero2.get_Item(num15) * num);
				}
			}
			topSpeedComponent.positiveAxisMaxSpeed = zero;
			topSpeedComponent.negativeAxisMaxSpeed = zero2;
		}

		private float GetMaxSpeed(MachineTopSpeedNode machineNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			Vector3 positiveAxisMaxSpeed = machineNode.topSpeedComponent.positiveAxisMaxSpeed;
			Vector3 negativeAxisMaxSpeed = machineNode.topSpeedComponent.negativeAxisMaxSpeed;
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				if (positiveAxisMaxSpeed.get_Item(i) > num)
				{
					num = positiveAxisMaxSpeed.get_Item(i);
				}
				if (negativeAxisMaxSpeed.get_Item(i) > num)
				{
					num = negativeAxisMaxSpeed.get_Item(i);
				}
			}
			return num;
		}

		private bool MustLimitMaxSpeed(FasterList<TopSpeedNode> nodes, bool dontCheckIsValid)
		{
			for (int i = 0; i < nodes.get_Count(); i++)
			{
				TopSpeedNode topSpeedNode = nodes.get_Item(i);
				if (topSpeedNode.validMovComponent.affectsMaxSpeed && (topSpeedNode.validMovComponent.isValid || dontCheckIsValid))
				{
					return true;
				}
			}
			return false;
		}
	}
}
