using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Cosmetic.Balloon
{
	internal class BalloonEngine : IQueryingEntityViewEngine, IEngine
	{
		private const float SPRING_STENGTH_SCALAR = 0.01f;

		private const float MAX_ROTATION_ANGLE = 18f;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
			TaskRunnerExtensions.Run(Update());
		}

		private IEnumerator Update()
		{
			while (true)
			{
				if (Time.get_timeScale() > 0.9f)
				{
					FasterListEnumerator<BalloonNode> enumerator = entityViewsDB.QueryEntityViews<BalloonNode>().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							BalloonNode current = enumerator.get_Current();
							ProcessBalloon(current);
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				yield return null;
			}
		}

		private void ProcessBalloon(BalloonNode balloonNode)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (!balloonNode.visibilityComponent.offScreen && balloonNode.hardwareDisabledComponent.enabled)
			{
				Quaternion localRotation = balloonNode.balloonComponent.joints[0].get_localRotation();
				Vector3 cleanEuler = GetCleanEuler(localRotation.get_eulerAngles());
				Vector3 stalkRotationVelocity = balloonNode.balloonComponent.stalkRotationVelocity;
				float springStength = balloonNode.balloonComponent.springStength;
				float damping = balloonNode.balloonComponent.damping;
				stalkRotationVelocity += CalculateAVDeltaFromSpring(springStength, cleanEuler);
				stalkRotationVelocity += CalculateAVDeltaFromLateralMovement(balloonNode);
				stalkRotationVelocity *= damping;
				balloonNode.balloonComponent.stalkRotationVelocity = stalkRotationVelocity;
				Vector3 euler = cleanEuler + stalkRotationVelocity;
				Transform[] joints = balloonNode.balloonComponent.joints;
				foreach (Transform val in joints)
				{
					val.set_localEulerAngles(ClampEuler(euler));
				}
			}
		}

		private Vector3 GetCleanEuler(Vector3 euler)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = euler;
			if (result.x % 360f > 180f)
			{
				result.x = result.x % 360f - 360f;
			}
			if (result.y % 360f > 180f)
			{
				result.y = result.y % 360f - 360f;
			}
			if (result.z % 360f > 180f)
			{
				result.z = result.z % 360f - 360f;
			}
			return result;
		}

		private Vector3 CalculateAVDeltaFromSpring(float springStength, Vector3 euler)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(Mathf.Pow(euler.x, 2f) * (0f - Mathf.Sign(euler.x)) * springStength * 0.01f, 0f, Mathf.Pow(euler.z, 2f) * (0f - Mathf.Sign(euler.z)) * springStength * 0.01f);
		}

		private Vector3 CalculateAVDeltaFromLateralMovement(BalloonNode balloonNode)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			Vector3 lastPos = balloonNode.balloonComponent.lastPos;
			float lastTime = balloonNode.balloonComponent.lastTime;
			Vector3 lastVelocity = balloonNode.balloonComponent.lastVelocity;
			Transform t = balloonNode.transformComponent.T;
			float lateralInfluence = balloonNode.balloonComponent.lateralInfluence;
			if (lastPos != t.get_position())
			{
				Vector3 val = Vector3.get_zero();
				if (Time.get_time() - lastTime > 0f)
				{
					val = (lastPos - t.get_position()) / (Time.get_time() - lastTime);
				}
				Vector3 val2 = lastVelocity - val;
				balloonNode.balloonComponent.lastVelocity = val;
				balloonNode.balloonComponent.lastTime = Time.get_time();
				balloonNode.balloonComponent.lastPos = t.get_position();
				float num = Vector3.Dot(val2, -t.get_forward()) * lateralInfluence;
				float num2 = Vector3.Dot(val2, t.get_right()) * lateralInfluence;
				return new Vector3(num, 0f, num2);
			}
			balloonNode.balloonComponent.lastTime = Time.get_time();
			return Vector3.get_zero();
		}

		private Vector3 ClampEuler(Vector3 euler)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = default(Vector3);
			result._002Ector(Mathf.Clamp(euler.x, -18f, 18f), 0f, Mathf.Clamp(euler.z, -18f, 18f));
			return result;
		}
	}
}
