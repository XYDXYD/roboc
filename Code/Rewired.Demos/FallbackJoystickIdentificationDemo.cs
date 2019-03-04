using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class FallbackJoystickIdentificationDemo : MonoBehaviour
	{
		private const float windowWidth = 250f;

		private const float windowHeight = 250f;

		private const float inputDelay = 1f;

		private bool identifyRequired;

		private Queue<Joystick> joysticksToIdentify;

		private float nextInputAllowedTime;

		private GUIStyle style;

		public FallbackJoystickIdentificationDemo()
			: this()
		{
		}

		private void Awake()
		{
			if (ReInput.get_unityJoystickIdentificationRequired())
			{
				ReInput.add_ControllerConnectedEvent((Action<ControllerStatusChangedEventArgs>)JoystickConnected);
				ReInput.add_ControllerDisconnectedEvent((Action<ControllerStatusChangedEventArgs>)JoystickDisconnected);
				IdentifyAllJoysticks();
			}
		}

		private void JoystickConnected(ControllerStatusChangedEventArgs args)
		{
			IdentifyAllJoysticks();
		}

		private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			IdentifyAllJoysticks();
		}

		public void IdentifyAllJoysticks()
		{
			Reset();
			if (ReInput.get_controllers().get_joystickCount() != 0)
			{
				Joystick[] joysticks = ReInput.get_controllers().GetJoysticks();
				if (joysticks != null)
				{
					identifyRequired = true;
					joysticksToIdentify = new Queue<Joystick>(joysticks);
					SetInputDelay();
				}
			}
		}

		private void SetInputDelay()
		{
			nextInputAllowedTime = Time.get_time() + 1f;
		}

		private unsafe void OnGUI()
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			if (!identifyRequired)
			{
				return;
			}
			if (joysticksToIdentify == null || joysticksToIdentify.Count == 0)
			{
				Reset();
				return;
			}
			Rect val = default(Rect);
			val._002Ector((float)Screen.get_width() * 0.5f - 125f, (float)Screen.get_height() * 0.5f - 125f, 250f, 250f);
			GUILayout.Window(0, val, new WindowFunction((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), "Joystick Identification Required", (GUILayoutOption[])new GUILayoutOption[0]);
			GUI.FocusWindow(0);
			if (!(Time.get_time() < nextInputAllowedTime) && ReInput.get_controllers().SetUnityJoystickIdFromAnyButtonOrAxisPress(joysticksToIdentify.Peek().id, 0.8f, false))
			{
				joysticksToIdentify.Dequeue();
				SetInputDelay();
				if (joysticksToIdentify.Count == 0)
				{
					Reset();
				}
			}
		}

		private void DrawDialogWindow(int windowId)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			if (identifyRequired)
			{
				if (style == null)
				{
					style = new GUIStyle(GUI.get_skin().get_label());
					style.set_wordWrap(true);
				}
				GUILayout.Space(15f);
				GUILayout.Label("A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:", style, (GUILayoutOption[])new GUILayoutOption[0]);
				Joystick val = joysticksToIdentify.Peek();
				GUILayout.Label("Press any button on \"" + val.get_name() + "\" now.", style, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Skip", (GUILayoutOption[])new GUILayoutOption[0]))
				{
					joysticksToIdentify.Dequeue();
				}
			}
		}

		private void Reset()
		{
			joysticksToIdentify = null;
			identifyRequired = false;
		}
	}
}
