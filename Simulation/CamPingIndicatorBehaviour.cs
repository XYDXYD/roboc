using System;
using UnityEngine;

namespace Simulation
{
	internal class CamPingIndicatorBehaviour : MonoBehaviour
	{
		public MapPingBehaviour mapPingBehaviour;

		public PingType type;

		public float divider;

		private GameObject _activeIndicator;

		private Camera _camera;

		private Transform _arrow;

		public CamPingIndicatorBehaviour()
			: this()
		{
		}

		private void Start()
		{
			_activeIndicator = this.get_transform().GetChild((int)type).get_gameObject();
			_arrow = this.get_transform().GetChild((int)type).GetChild(2);
			_camera = Camera.get_main();
			mapPingBehaviour.OnDestroy += OnDestroyed;
		}

		private void Update()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = mapPingBehaviour.get_transform().get_position() - _camera.get_transform().get_position();
			val = _camera.get_transform().InverseTransformDirection(val);
			Vector3 val2 = Vector3.get_right() * val.x + Vector3.get_up() * 0f + Vector3.get_forward() * val.z;
			Vector3 val3 = Vector3.get_right() * 0f + Vector3.get_up() * val.y + Vector3.get_forward() * val.z;
			Vector3 val4 = default(Vector3);
			val4._002Ector(val.x, val.y, 0f);
			float num = _camera.get_fieldOfView() * ((float)Math.PI / 180f);
			float num2 = 2f * Mathf.Atan(Mathf.Tan(num / 2f) * _camera.get_aspect());
			float num3 = 57.29578f * num2;
			float num4 = Vector3.Angle(Vector3.get_forward(), val2);
			float num5 = Vector3.Angle(Vector3.get_forward(), val3);
			if (num4 > num3 / 2f || num5 > _camera.get_fieldOfView() / 2f)
			{
				_activeIndicator.SetActive(true);
				float num6 = Vector3.Angle(Vector3.get_up(), val4);
				float num7 = Vector3.Dot(Vector3.get_right(), val4);
				if (num4 > num6)
				{
					num6 = num4;
				}
				if (num7 > 0f)
				{
					num6 = 0f - num6;
				}
				float num8 = num6;
				Vector3 zero = Vector3.get_zero();
				if (divider <= 1f)
				{
					zero = Vector3.get_zero() + Quaternion.Euler(0f, 0f, num6) * (Vector3.get_up() * (float)(Screen.get_height() / 2) * divider);
				}
				else
				{
					if (Mathf.Abs(num8) > 90f)
					{
						num8 = 90f - (Mathf.Abs(num6) - 90f);
					}
					float num9 = (float)(Screen.get_height() / 2) / Mathf.Cos((float)Math.PI / 180f * Mathf.Abs(num8));
					if (num9 > (float)(Screen.get_height() / 2) * divider)
					{
						num9 = (float)(Screen.get_height() / 2) * divider;
					}
					zero = Vector3.get_zero() + Quaternion.Euler(0f, 0f, num6) * (Vector3.get_up() * (num9 - 22f));
				}
				_arrow.set_localRotation(Quaternion.Euler(0f, 0f, 0f - (180f - num6)));
				_activeIndicator.get_transform().set_localPosition(zero);
			}
			else
			{
				_activeIndicator.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			mapPingBehaviour.OnDestroy -= OnDestroyed;
		}

		public void OnDestroyed()
		{
			Object.Destroy(this.get_gameObject());
		}
	}
}
