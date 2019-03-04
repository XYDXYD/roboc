using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilUpdateEngine : MultiEntityViewsEngine<LocalAerofoilComponentNode, LocalMachineAerofoilNode>, IQueryingEntityViewEngine, IEngine
	{
		private const float DEFAULT_ANGULAR_DRAG = 0.05f;

		private const float ANGULAR_DRAG_SCALAR = 5f;

		private const float LIGHT_PLANE_MASS = 400f;

		private const float HEAVY_PLANE_MASS = 1200f;

		private const float CAM_TILT_SCALAR = 1.35f;

		private const float CAM_ELEVATE_BIAS = 0.15f;

		private const float CAM_FORWARD_BIAS = 0.1f;

		private const float ROB_FORWARD_BIAS = 0.5f;

		private readonly float[] _aDragParams = new float[4];

		private LocalMachineAerofoilNode _machineAerofoilNode;

		private float _initialMass;

		private ITaskRoutine _task;

		[Inject]
		internal PlayerStrafeDirectionManager strafeDirectionManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public AerofoilUpdateEngine()
		{
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(LocalAerofoilComponentNode obj)
		{
			_machineAerofoilNode.aerofoilComponent.updateRequired = true;
			obj.hardwareDisabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)OnAerofoilDestroyed);
		}

		protected override void Remove(LocalAerofoilComponentNode obj)
		{
			obj.hardwareDisabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnAerofoilDestroyed);
		}

		protected override void Add(LocalMachineAerofoilNode obj)
		{
			_machineAerofoilNode = obj;
			_initialMass = obj.rigidbodyComponent.rb.get_mass();
			_task.Start((Action<PausableTaskException>)null, (Action)null);
		}

		protected override void Remove(LocalMachineAerofoilNode obj)
		{
			_machineAerofoilNode = null;
			_task.Stop();
		}

		private void OnAerofoilDestroyed(int arg1, bool b)
		{
			_machineAerofoilNode.aerofoilComponent.updateRequired = true;
		}

		private IEnumerator Update()
		{
			while (true)
			{
				IMachineAerofoilComponent aerofoilComponent = _machineAerofoilNode.aerofoilComponent;
				if (aerofoilComponent.updateRequired)
				{
					Init();
					aerofoilComponent.updateRequired = false;
				}
				if (aerofoilComponent.numAerofoils > 0)
				{
					UpdateInput();
				}
				yield return null;
			}
		}

		private void UpdateInput()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = _machineAerofoilNode.rigidbodyComponent.rb;
			Transform t = _machineAerofoilNode.transformComponent.T;
			IMachineInputComponent inputComponent = _machineAerofoilNode.inputComponent;
			Vector4 analogInput = inputComponent.analogInput;
			if (strafeDirectionManager.strafingEnabled)
			{
				Transform transform = Camera.get_main().get_transform();
				float num = Mathf.Clamp(Vector3.Dot(rb.get_velocity(), t.get_forward()), -1f, 1f) * Mathf.Sign(Vector3.Dot(transform.get_forward(), t.get_forward()));
				if (strafeDirectionManager.sidewaysDrivingEnabled)
				{
					Vector3 val = Vector3.Cross(Vector3.get_up(), t.get_forward());
					Vector3 normalized = val.get_normalized();
					Vector3 val2 = Vector3.Cross(t.get_right(), Vector3.get_up());
					Vector3 normalized2 = val2.get_normalized();
					Vector3 forward = transform.get_forward();
					float x = forward.x;
					Vector3 forward2 = transform.get_forward();
					Vector3 val3 = default(Vector3);
					val3._002Ector(x, 0f, forward2.z);
					Vector3 normalized3 = val3.get_normalized();
					Vector3 right = transform.get_right();
					float x2 = right.x;
					Vector3 right2 = transform.get_right();
					Vector3 val4 = default(Vector3);
					val4._002Ector(x2, 0f, right2.z);
					Vector3 normalized4 = val4.get_normalized();
					Vector3 val5 = analogInput.z * normalized3 + analogInput.x * normalized4;
					Vector3 normalized5 = val5.get_normalized();
					Vector3 val6 = normalized;
					Vector3 val7 = normalized5 + normalized3 * num * 0.1f;
					analogInput.x = Vector3.Dot(val6, val7.get_normalized());
					if (normalized5.get_magnitude() > 0.01f)
					{
						Vector3 val8 = normalized2;
						Vector3 val9 = normalized5 + normalized2 * 0.5f;
						analogInput.z = Mathf.Sign(Vector3.Dot(val8, val9.get_normalized()));
					}
				}
				else
				{
					analogInput.x = (0f - Mathf.Clamp(strafeDirectionManager.angleToStraight / 90f, -1f, 1f)) * Mathf.Sign(Vector3.Dot(t.get_forward(), rb.get_velocity()));
				}
				if (strafeDirectionManager.verticalStrafingEnabled)
				{
					analogInput.y = Mathf.Clamp(analogInput.y + (num * (Vector3.Dot(transform.get_forward(), Vector3.get_up()) * 1.35f) + 0.15f), -1f, 1f);
				}
			}
			_machineAerofoilNode.aerofoilComponent.aerofoilInput = Vector4.op_Implicit(analogInput);
		}

		private void Init()
		{
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			if (_initialMass == 0f)
			{
				return;
			}
			IMachineAerofoilComponent aerofoilComponent = _machineAerofoilNode.aerofoilComponent;
			Rigidbody rb = _machineAerofoilNode.rigidbodyComponent.rb;
			if (!(rb != null))
			{
				return;
			}
			Transform transform = rb.get_transform();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			float num12 = 0f;
			float num13 = 0f;
			float num14 = 0f;
			float num15 = 0f;
			float num16 = 0f;
			float num17 = 0f;
			float num18 = 0f;
			float num19 = 0f;
			float num20 = 0f;
			float num21 = 0f;
			float num22 = 0f;
			float num23 = 0f;
			float num24 = 0f;
			float num25 = 0f;
			float num26 = 0f;
			float num27 = 0f;
			float num28 = 0f;
			Vector3 val = Vector3.get_zero();
			FasterReadOnlyList<LocalAerofoilComponentNode> val2 = entityViewsDB.QueryEntityViews<LocalAerofoilComponentNode>();
			for (int i = 0; i < val2.get_Count(); i++)
			{
				LocalAerofoilComponentNode localAerofoilComponentNode = val2.get_Item(i);
				if (localAerofoilComponentNode.hardwareDisabledComponent.disabled)
				{
					continue;
				}
				IAerofoilComponent aerofoilComponent2 = localAerofoilComponentNode.aerofoilComponent;
				Vector3 position = aerofoilComponent2.forceT.get_position();
				Vector3 centerOfMass = rb.get_centerOfMass();
				float maxCarryMass = localAerofoilComponentNode.carryMassComponent.maxCarryMass;
				if (Mathf.Abs(Vector3.Dot(aerofoilComponent2.forceT.get_right(), transform.get_up())) > 0.95f)
				{
					num10 += maxCarryMass;
					num17 += aerofoilComponent2.bankSpeed;
					num18 += aerofoilComponent2.bankSpeedHeavy;
					num13 += maxCarryMass;
					num19 += aerofoilComponent2.elevationSpeed;
					num20 += aerofoilComponent2.elevationSpeedHeavy;
					val += transform.InverseTransformPoint(position);
					Vector3 val3 = transform.InverseTransformPoint(position);
					if (val3.x < centerOfMass.x)
					{
						num3++;
					}
					else
					{
						num4++;
					}
					Vector3 val4 = transform.InverseTransformPoint(position);
					if (val4.z > centerOfMass.z)
					{
						num++;
					}
					else
					{
						num2++;
					}
					float num29 = num28;
					Vector3 val5 = transform.InverseTransformPoint(position);
					num28 = num29 + Mathf.Abs(val5.z - centerOfMass.z);
					if (Mathf.Abs(Vector3.Dot(aerofoilComponent2.forceT.get_up(), transform.get_right())) > 0.95f)
					{
						num6++;
						num15 += aerofoilComponent2.barrelSpeed;
						num16 += aerofoilComponent2.barrelSpeedHeavy;
						num11 += maxCarryMass;
						float num30 = num27;
						Vector3 val6 = transform.InverseTransformPoint(position);
						num27 = num30 + Mathf.Abs(val6.x - centerOfMass.x);
						num9 += maxCarryMass;
						num23 += aerofoilComponent2.thrust;
						num24 += aerofoilComponent2.thrustHeavy;
					}
					num8++;
					num25 += aerofoilComponent2.vtolVelocity;
				}
				else if (Mathf.Abs(Vector3.Dot(aerofoilComponent2.forceT.get_right(), transform.get_right())) > 0.95f && Mathf.Abs(Vector3.Dot(aerofoilComponent2.forceT.get_forward(), transform.get_forward())) > 0.95f)
				{
					num5++;
					num8++;
					num17 += aerofoilComponent2.bankSpeed;
					num18 += aerofoilComponent2.bankSpeedHeavy;
					num13 += maxCarryMass;
					num21 += aerofoilComponent2.rudderSpeed;
					num22 += num22;
					num15 += aerofoilComponent2.barrelSpeed;
					num16 += aerofoilComponent2.barrelSpeedHeavy;
					num11 += maxCarryMass;
					float num31 = num27;
					Vector3 val7 = transform.InverseTransformPoint(position);
					num27 = num31 + Mathf.Abs(val7.z - centerOfMass.z);
					num14 += maxCarryMass;
					num23 += aerofoilComponent2.thrust;
					num24 += aerofoilComponent2.thrustHeavy;
				}
				else if (Mathf.Abs(Vector3.Dot(aerofoilComponent2.forceT.get_right(), transform.get_forward())) > 0.95f)
				{
					num7++;
					num8++;
					num12 += maxCarryMass;
				}
			}
			aerofoilComponent.liftCapacityRatio = Mathf.Clamp01(num9 / _initialMass);
			aerofoilComponent.lateralCapacityRatio = Mathf.Clamp01(num10 / _initialMass);
			aerofoilComponent.barrelCapacityRatio = Mathf.Clamp01(num11 / _initialMass);
			aerofoilComponent.stopperCapacityRatio = Mathf.Clamp01(num12 / _initialMass);
			aerofoilComponent.bankCapacityRatio = Mathf.Clamp01(num13 / _initialMass);
			aerofoilComponent.rudderCapacityRatio = Mathf.Clamp01(num14 / _initialMass);
			float num32 = Mathf.InverseLerp(400f, 1200f, _initialMass);
			if (num6 > 0 || num5 > 0)
			{
				num15 = Mathf.Lerp(num15, num16, num32);
				aerofoilComponent.barrelSpeed = num15 / (float)(num6 + num5) * aerofoilComponent.barrelCapacityRatio;
				aerofoilComponent.barrelOffset = num27 / (float)(num6 + num5);
			}
			if (num3 > 0 || num4 > 0 || num5 > 0)
			{
				num17 = Mathf.Lerp(num17, num18, num32);
				aerofoilComponent.bankSpeed = num17 / (float)(num3 + num4 + num5) * aerofoilComponent.bankCapacityRatio;
			}
			if (num6 > 0 || num5 > 0)
			{
				num23 = Mathf.Lerp(num23, num24, num32);
				aerofoilComponent.thrust = num23 / (float)(num6 + num5);
			}
			if (num5 > 0)
			{
				num21 = Mathf.Lerp(num21, num22, num32);
				aerofoilComponent.rudderSpeed = num21 / (float)num5;
			}
			if (num3 > 0 || num4 > 0)
			{
				num25 = Mathf.Lerp(num25, num26, num32);
				aerofoilComponent.vtolVelocity = num25 / (float)(num3 + num4);
				val /= (float)(num3 + num4);
			}
			if (num6 > 0)
			{
				num19 = Mathf.Lerp(num19, num20, num32);
				aerofoilComponent.elevationSpeed = num19 / (float)num6 * aerofoilComponent.liftCapacityRatio;
				aerofoilComponent.elevationOffset = num28 / (float)num6;
			}
			Vector3 zero = Vector3.get_zero();
			if (num == 0 || num2 == 0)
			{
				zero.z = val.z;
			}
			if (num3 == 0 || num4 == 0)
			{
				zero.x = val.x;
			}
			aerofoilComponent.lateralForceOffset = zero;
			aerofoilComponent.numAerofoils = num8;
			SetAngularDrag(aerofoilComponent);
		}

		private void SetAngularDrag(IMachineAerofoilComponent aeroFoilComponent)
		{
			_aDragParams[0] = 0.05f;
			_aDragParams[1] = aeroFoilComponent.lateralCapacityRatio * 5f;
			_aDragParams[2] = aeroFoilComponent.rudderCapacityRatio * 5f;
			_aDragParams[3] = aeroFoilComponent.lateralCapacityRatio * 5f;
			aeroFoilComponent.aDragParams = _aDragParams;
		}
	}
}
