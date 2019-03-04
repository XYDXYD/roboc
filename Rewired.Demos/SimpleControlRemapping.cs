using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class SimpleControlRemapping : MonoBehaviour
	{
		private class Row
		{
			public InputAction action;

			public AxisRange actionRange;

			public Button button;

			public Text text;
		}

		private const string category = "Default";

		private const string layout = "Default";

		private InputMapper inputMapper = new InputMapper();

		public GameObject buttonPrefab;

		public GameObject textPrefab;

		public RectTransform fieldGroupTransform;

		public RectTransform actionGroupTransform;

		public Text controllerNameUIText;

		public Text statusUIText;

		private ControllerType selectedControllerType;

		private int selectedControllerId;

		private List<Row> rows = new List<Row>();

		private Player player => ReInput.get_players().GetPlayer(0);

		private ControllerMap controllerMap
		{
			get
			{
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				if (controller == null)
				{
					return null;
				}
				return player.controllers.maps.GetMap(controller.get_type(), controller.id, "Default", "Default");
			}
		}

		private Controller controller => player.controllers.GetController(selectedControllerType, selectedControllerId);

		public SimpleControlRemapping()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown


		private void OnEnable()
		{
			if (ReInput.get_isReady())
			{
				inputMapper.get_options().set_timeout(5f);
				inputMapper.get_options().set_ignoreMouseXAxis(true);
				inputMapper.get_options().set_ignoreMouseYAxis(true);
				ReInput.add_ControllerConnectedEvent((Action<ControllerStatusChangedEventArgs>)OnControllerChanged);
				ReInput.add_ControllerDisconnectedEvent((Action<ControllerStatusChangedEventArgs>)OnControllerChanged);
				inputMapper.add_InputMappedEvent((Action<InputMappedEventData>)OnInputMapped);
				inputMapper.add_StoppedEvent((Action<StoppedEventData>)OnStopped);
				InitializeUI();
			}
		}

		private void OnDisable()
		{
			inputMapper.Stop();
			inputMapper.RemoveAllEventListeners();
			ReInput.remove_ControllerConnectedEvent((Action<ControllerStatusChangedEventArgs>)OnControllerChanged);
			ReInput.remove_ControllerDisconnectedEvent((Action<ControllerStatusChangedEventArgs>)OnControllerChanged);
		}

		private unsafe void RedrawUI()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Expected O, but got Unknown
			if (controller == null)
			{
				ClearUI();
				return;
			}
			controllerNameUIText.set_text(controller.get_name());
			for (int i = 0; i < rows.Count; i++)
			{
				Row row = rows[i];
				InputAction action = rows[i].action;
				string text = string.Empty;
				int actionElementMapId = -1;
				foreach (ActionElementMap item in controllerMap.ElementMapsWithAction(action.get_id()))
				{
					if (item.ShowInField(row.actionRange))
					{
						text = item.get_elementIdentifierName();
						actionElementMapId = item.get_id();
						break;
					}
				}
				row.text.set_text(text);
				row.button.get_onClick().RemoveAllListeners();
				int index = i;
				_003CRedrawUI_003Ec__AnonStorey0 _003CRedrawUI_003Ec__AnonStorey;
				row.button.get_onClick().AddListener(new UnityAction((object)_003CRedrawUI_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void ClearUI()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)selectedControllerType == 2)
			{
				controllerNameUIText.set_text("No joysticks attached");
			}
			else
			{
				controllerNameUIText.set_text(string.Empty);
			}
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].text.set_text(string.Empty);
			}
		}

		private void InitializeUI()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Invalid comparison between Unknown and I4
			IEnumerator enumerator = actionGroupTransform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					Object.Destroy(val.get_gameObject());
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			IEnumerator enumerator2 = fieldGroupTransform.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Transform val2 = enumerator2.Current;
					Object.Destroy(val2.get_gameObject());
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			foreach (InputAction action in ReInput.get_mapping().get_Actions())
			{
				if ((int)action.get_type() == 0)
				{
					CreateUIRow(action, 0, action.get_descriptiveName());
					CreateUIRow(action, 1, string.IsNullOrEmpty(action.get_positiveDescriptiveName()) ? (action.get_descriptiveName() + " +") : action.get_positiveDescriptiveName());
					CreateUIRow(action, 2, string.IsNullOrEmpty(action.get_negativeDescriptiveName()) ? (action.get_descriptiveName() + " -") : action.get_negativeDescriptiveName());
				}
				else if ((int)action.get_type() == 1)
				{
					CreateUIRow(action, 1, action.get_descriptiveName());
				}
			}
			RedrawUI();
		}

		private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Object.Instantiate<GameObject>(textPrefab);
			val.get_transform().SetParent(actionGroupTransform);
			val.get_transform().SetAsLastSibling();
			val.GetComponent<Text>().set_text(label);
			GameObject val2 = Object.Instantiate<GameObject>(buttonPrefab);
			val2.get_transform().SetParent(fieldGroupTransform);
			val2.get_transform().SetAsLastSibling();
			rows.Add(new Row
			{
				action = action,
				actionRange = actionRange,
				button = val2.GetComponent<Button>(),
				text = val2.GetComponentInChildren<Text>()
			});
		}

		private void SetSelectedController(ControllerType controllerType)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			bool flag = false;
			if (controllerType != selectedControllerType)
			{
				selectedControllerType = controllerType;
				flag = true;
			}
			int num = selectedControllerId;
			if ((int)selectedControllerType == 2)
			{
				if (player.controllers.get_joystickCount() > 0)
				{
					selectedControllerId = player.controllers.get_Joysticks()[0].id;
				}
				else
				{
					selectedControllerId = -1;
				}
			}
			else
			{
				selectedControllerId = 0;
			}
			if (selectedControllerId != num)
			{
				flag = true;
			}
			if (flag)
			{
				inputMapper.Stop();
				RedrawUI();
			}
		}

		public void OnControllerSelected(int controllerType)
		{
			SetSelectedController(controllerType);
		}

		private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (index >= 0 && index < rows.Count && controller != null)
			{
				InputMapper obj = inputMapper;
				Context val = new Context();
				val.set_actionId(rows[index].action.get_id());
				val.set_controllerMap(controllerMap);
				val.set_actionRange(rows[index].actionRange);
				val.set_actionElementMapToReplace(controllerMap.GetElementMap(actionElementMapToReplaceId));
				obj.Start(val);
				statusUIText.set_text("Listening...");
			}
		}

		private void OnControllerChanged(ControllerStatusChangedEventArgs args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			SetSelectedController(selectedControllerType);
		}

		private void OnInputMapped(InputMappedEventData data)
		{
			RedrawUI();
		}

		private void OnStopped(StoppedEventData data)
		{
			statusUIText.set_text(string.Empty);
		}
	}
}
