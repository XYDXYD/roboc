using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class CustomControllerDemo : MonoBehaviour
	{
		public int playerId;

		public string controllerTag;

		public bool useUpdateCallbacks;

		private int buttonCount;

		private int axisCount;

		private float[] axisValues;

		private bool[] buttonValues;

		private TouchJoystickExample[] joysticks;

		private TouchButtonExample[] buttons;

		private CustomController controller;

		[NonSerialized]
		private bool initialized;

		public CustomControllerDemo()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			if ((int)SystemInfo.get_deviceType() == 1 && (int)Screen.get_orientation() != 3)
			{
				Screen.set_orientation(3);
			}
			Initialize();
		}

		private void Initialize()
		{
			ReInput.add_InputSourceUpdateEvent((Action)OnInputSourceUpdate);
			joysticks = this.GetComponentsInChildren<TouchJoystickExample>();
			buttons = this.GetComponentsInChildren<TouchButtonExample>();
			axisCount = joysticks.Length * 2;
			buttonCount = buttons.Length;
			axisValues = new float[axisCount];
			buttonValues = new bool[buttonCount];
			Player player = ReInput.get_players().GetPlayer(playerId);
			controller = player.controllers.GetControllerWithTag<CustomController>(controllerTag);
			if (controller == null)
			{
				Debug.LogError((object)("A matching controller was not found for tag \"" + controllerTag + "\""));
			}
			if (controller.get_buttonCount() != buttonValues.Length || controller.get_axisCount() != axisValues.Length)
			{
				Debug.LogError((object)"Controller has wrong number of elements!");
			}
			if (useUpdateCallbacks && controller != null)
			{
				controller.SetAxisUpdateCallback((Func<int, float>)GetAxisValueCallback);
				controller.SetButtonUpdateCallback((Func<int, bool>)GetButtonValueCallback);
			}
			initialized = true;
		}

		private void Update()
		{
			if (ReInput.get_isReady() && !initialized)
			{
				Initialize();
			}
		}

		private void OnInputSourceUpdate()
		{
			GetSourceAxisValues();
			GetSourceButtonValues();
			if (!useUpdateCallbacks)
			{
				SetControllerAxisValues();
				SetControllerButtonValues();
			}
		}

		private void GetSourceAxisValues()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < axisValues.Length; i++)
			{
				if (i % 2 != 0)
				{
					float[] array = axisValues;
					int num = i;
					Vector2 position = joysticks[i / 2].position;
					array[num] = position.y;
				}
				else
				{
					float[] array2 = axisValues;
					int num2 = i;
					Vector2 position2 = joysticks[i / 2].position;
					array2[num2] = position2.x;
				}
			}
		}

		private void GetSourceButtonValues()
		{
			for (int i = 0; i < buttonValues.Length; i++)
			{
				buttonValues[i] = buttons[i].isPressed;
			}
		}

		private void SetControllerAxisValues()
		{
			for (int i = 0; i < axisValues.Length; i++)
			{
				controller.SetAxisValue(i, axisValues[i]);
			}
		}

		private void SetControllerButtonValues()
		{
			for (int i = 0; i < buttonValues.Length; i++)
			{
				controller.SetButtonValue(i, buttonValues[i]);
			}
		}

		private float GetAxisValueCallback(int index)
		{
			if (index >= axisValues.Length)
			{
				return 0f;
			}
			return axisValues[index];
		}

		private bool GetButtonValueCallback(int index)
		{
			if (index >= buttonValues.Length)
			{
				return false;
			}
			return buttonValues[index];
		}
	}
}
