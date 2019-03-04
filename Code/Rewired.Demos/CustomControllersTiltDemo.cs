using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class CustomControllersTiltDemo : MonoBehaviour
	{
		public Transform target;

		public float speed = 10f;

		private CustomController controller;

		private Player player;

		public CustomControllersTiltDemo()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			Screen.set_orientation(3);
			player = ReInput.get_players().GetPlayer(0);
			ReInput.add_InputSourceUpdateEvent((Action)OnInputUpdate);
			controller = player.controllers.GetControllerWithTag(20, "TiltController");
		}

		private void Update()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			if (!(target == null))
			{
				Vector3 val = Vector3.get_zero();
				val.y = player.GetAxis("Tilt Vertical");
				val.x = player.GetAxis("Tilt Horizontal");
				if (val.get_sqrMagnitude() > 1f)
				{
					val.Normalize();
				}
				val *= Time.get_deltaTime();
				target.Translate(val * speed);
			}
		}

		private void OnInputUpdate()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Vector3 acceleration = Input.get_acceleration();
			controller.SetAxisValue(0, acceleration.x);
			controller.SetAxisValue(1, acceleration.y);
			controller.SetAxisValue(2, acceleration.z);
		}
	}
}
