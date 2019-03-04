using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilLocalGFXEngine : SingleEntityViewEngine<LocalMachineAerofoilNode>, IQueryingEntityViewEngine, IEngine
	{
		private LocalMachineAerofoilNode _localMachineNode;

		private Vector3 _input;

		private ITaskRoutine _taskRoutine;

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

		public AerofoilLocalGFXEngine()
		{
			_taskRoutine = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(LocalMachineAerofoilNode node)
		{
			_localMachineNode = node;
			_taskRoutine.Start((Action<PausableTaskException>)null, (Action)null);
		}

		protected override void Remove(LocalMachineAerofoilNode node)
		{
			_taskRoutine.Stop();
			_localMachineNode = null;
		}

		private void SetThrustGFXPlayer(LocalAerofoilGraphicsNode node)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			GameObject onSFXGameObject = node.aerofoilGfxComponent.onSFXGameObject;
			GameObject thrustSFXGameObject = node.aerofoilGfxComponent.thrustSFXGameObject;
			Transform transform = node.rigidbodyComponent.rb.get_transform();
			Rigidbody rb = node.rigidbodyComponent.rb;
			if (onSFXGameObject == null || thrustSFXGameObject == null)
			{
				return;
			}
			if (Mathf.Abs(Vector3.Dot(t.get_forward(), transform.get_forward())) > 0.99f)
			{
				if (!onSFXGameObject.get_activeSelf())
				{
					onSFXGameObject.SetActive(true);
				}
				Vector3 val = default(Vector3);
				val._002Ector(0f, _input.y, _input.z);
				float magnitude = val.get_magnitude();
				if (magnitude >= 0.2f && !thrustSFXGameObject.get_activeSelf())
				{
					thrustSFXGameObject.SetActive(true);
				}
				else if (magnitude < 0.2f && thrustSFXGameObject.get_activeSelf())
				{
					thrustSFXGameObject.SetActive(false);
				}
			}
			else if (onSFXGameObject.get_activeSelf() || thrustSFXGameObject.get_activeSelf())
			{
				onSFXGameObject.SetActive(false);
				thrustSFXGameObject.SetActive(false);
			}
		}

		private void SetFlapOrientationPlayer(LocalAerofoilGraphicsNode node)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform transform = node.rigidbodyComponent.rb.get_transform();
			Transform t = node.transformComponent.T;
			Transform flapT = node.aerofoilGfxComponent.flapT;
			if (flapT == null)
			{
				return;
			}
			Vector3 val = transform.get_forward() * 0.01f;
			Vector3 val2 = flapT.get_position() - rb.get_worldCenterOfMass();
			bool flag = false;
			float num = -0.01f;
			if (Mathf.Abs(Vector3.Dot(t.get_forward(), transform.get_forward())) > 0.99f && Mathf.Abs(Vector3.Dot(t.get_up(), transform.get_right())) > 0.99f)
			{
				float num2 = Mathf.Clamp(Vector3.Dot(rb.get_velocity(), transform.get_forward()) / 15f, -1f, 1f);
				float num3 = Mathf.Sign(num2);
				if (Vector3.Dot(val2, transform.get_right()) > 0f)
				{
					val += num3 * transform.get_forward() * Mathf.Abs(_input.x) + transform.get_up() * (0f - _input.x);
				}
				else
				{
					val += num3 * transform.get_forward() * Mathf.Abs(_input.x) + transform.get_up() * _input.x;
					num *= -1f;
				}
				val = ((!(Vector3.Dot(val2, transform.get_forward()) < 0f)) ? (val + num2 * (num3 * transform.get_forward() * Mathf.Abs(_input.y) + num3 * transform.get_up() * _input.y) * Mathf.Sign(Vector3.Dot(rb.get_velocity(), transform.get_forward()))) : (val + num2 * (num3 * transform.get_forward() * Mathf.Abs(_input.y) + num3 * transform.get_up() * (0f - _input.y)) * Mathf.Sign(Vector3.Dot(rb.get_velocity(), transform.get_forward()))));
				val += transform.get_forward() * _input.z;
				val += (1f - Mathf.Abs(num2)) * Vector3.get_up() + transform.get_forward() * 0.5f;
				flag = true;
			}
			if (Mathf.Abs(Vector3.Dot(t.get_forward(), transform.get_forward())) > 0.99f && Mathf.Abs(Vector3.Dot(t.get_up(), transform.get_up())) > 0.99f)
			{
				val = ((!(Vector3.Dot(val2, transform.get_forward()) < 0f)) ? (val + (transform.get_forward() * Mathf.Ceil(Mathf.Abs(_input.x)) + transform.get_right() * _input.x) * Mathf.Sign(Vector3.Dot(rb.get_velocity(), transform.get_forward()))) : (val + (transform.get_forward() * Mathf.Ceil(Mathf.Abs(_input.x)) + transform.get_right() * (0f - _input.x)) * Mathf.Sign(Vector3.Dot(rb.get_velocity(), transform.get_forward()))));
				val += transform.get_forward() * _input.z;
				flag = true;
			}
			if (flag)
			{
				float num4 = Vector3.Dot(val.get_normalized(), flapT.get_up()) + num;
				flapT.Rotate(num4 * 7.5f, 0f, 0f);
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				_input = _localMachineNode.aerofoilComponent.aerofoilInput;
				FasterReadOnlyList<LocalAerofoilGraphicsNode> localAerofoilGraphicsNodes = entityViewsDB.QueryEntityViews<LocalAerofoilGraphicsNode>();
				for (int i = 0; i < localAerofoilGraphicsNodes.get_Count(); i++)
				{
					LocalAerofoilGraphicsNode localAerofoilGraphicsNode = localAerofoilGraphicsNodes.get_Item(i);
					if (!localAerofoilGraphicsNode.disabledComponent.disabled)
					{
						SetThrustGFXPlayer(localAerofoilGraphicsNode);
						SetFlapOrientationPlayer(localAerofoilGraphicsNode);
					}
				}
				yield return null;
			}
		}
	}
}
