using Mothership.GUI.Party;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.CustomGames
{
	internal class CustomGamePartyGUIView : MonoBehaviour, IChainRoot, IPartyGUIViewRoot, IChainListener
	{
		[Serializable]
		private class EditModeStyle
		{
			public GameObject container;
		}

		public enum StyleVersion
		{
			Garage,
			EditMode,
			CustomGameScreen,
			AllOtherLocations
		}

		[SerializeField]
		private GameObject TeamsPanelGO;

		[SerializeField]
		private EditModeStyle editModeStyle;

		[SerializeField]
		private EditModeStyle garageModeStyle;

		[SerializeField]
		private GameObject[] DragAndDropText;

		[SerializeField]
		private GameObject[] BackgroundGraphics;

		[SerializeField]
		private GameObject[] TeamHeaders;

		[SerializeField]
		private GameObject[] YourPartyIsWaitingIndicator;

		[SerializeField]
		private Color HighlightedDragAndDropTextColor;

		[SerializeField]
		private Color DarkDragAndDropTextColor;

		[SerializeField]
		public UIWidget TeamAUIWidget;

		[SerializeField]
		public UIWidget TeamBUIWidget;

		private SignalChain _signalChain;

		private CustomGamePartyGUIController _controller;

		private UIAnchorUtility.Anchors _defaultAnchors;

		public CustomGamePartyGUIView()
			: this()
		{
		}

		public void InjectController(CustomGamePartyGUIController controller)
		{
			_controller = controller;
		}

		public void Initialize()
		{
			UIWidget component = TeamsPanelGO.GetComponent<UIWidget>();
			_defaultAnchors = UIAnchorUtility.CloneAnchors(component);
		}

		public void SetYourPartyIsWaitingIndicatorVisibility(bool setting)
		{
			GameObject[] yourPartyIsWaitingIndicator = YourPartyIsWaitingIndicator;
			foreach (GameObject val in yourPartyIsWaitingIndicator)
			{
				val.SetActive(setting);
			}
		}

		private void ApplyStyleVisibilities(bool dragAndDropTextVisible, bool backgroundsVisible, bool teamHeadersVisible)
		{
			GameObject[] dragAndDropText = DragAndDropText;
			foreach (GameObject val in dragAndDropText)
			{
				val.SetActive(dragAndDropTextVisible);
			}
			GameObject[] backgroundGraphics = BackgroundGraphics;
			foreach (GameObject val2 in backgroundGraphics)
			{
				val2.SetActive(backgroundsVisible);
			}
			GameObject[] teamHeaders = TeamHeaders;
			foreach (GameObject val3 in teamHeaders)
			{
				val3.SetActive(teamHeadersVisible);
			}
		}

		public void ApplyStyle(int numTeams, StyleVersion styleVersion)
		{
			UIWidget component = TeamsPanelGO.GetComponent<UIWidget>();
			switch (styleVersion)
			{
			case StyleVersion.AllOtherLocations:
				component.SetAnchor(editModeStyle.container, 0, 0, 0, 0);
				break;
			case StyleVersion.CustomGameScreen:
				UIAnchorUtility.CopyAnchors(_defaultAnchors, component);
				break;
			case StyleVersion.EditMode:
				component.SetAnchor(editModeStyle.container, 0, 0, 0, 0);
				break;
			case StyleVersion.Garage:
				component.SetAnchor(garageModeStyle.container, 0, 0, 0, 0);
				break;
			}
			bool backgroundsVisible = false;
			bool dragAndDropTextVisible = false;
			bool teamHeadersVisible = false;
			if (styleVersion == StyleVersion.CustomGameScreen)
			{
				backgroundsVisible = true;
			}
			if (numTeams > 1 && styleVersion == StyleVersion.CustomGameScreen)
			{
				dragAndDropTextVisible = true;
				teamHeadersVisible = true;
			}
			ApplyStyleVisibilities(dragAndDropTextVisible, backgroundsVisible, teamHeadersVisible);
		}

		public void ShowTeamsPanel()
		{
			TeamsPanelGO.SetActive(true);
		}

		public void HideTeamsPanel()
		{
			TeamsPanelGO.SetActive(false);
		}

		public void BuildSignal()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			_signalChain = new SignalChain(this.get_transform());
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void DeepBroadcast<T>(T message)
		{
			_signalChain.DeepBroadcast<T>(message);
		}

		public void DragAndDropTextIsAvailable(bool availabilitySetting)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			GameObject[] dragAndDropText = DragAndDropText;
			foreach (GameObject val in dragAndDropText)
			{
				UILabel component = val.GetComponent<UILabel>();
				if (component != null)
				{
					if (availabilitySetting)
					{
						component.set_color(HighlightedDragAndDropTextColor);
					}
					else
					{
						component.set_color(DarkDragAndDropTextColor);
					}
				}
				UISprite component2 = val.GetComponent<UISprite>();
				if (component2 != null)
				{
					if (availabilitySetting)
					{
						component2.set_color(HighlightedDragAndDropTextColor);
					}
					else
					{
						component2.set_color(DarkDragAndDropTextColor);
					}
				}
			}
		}

		public void Broadcast(object message)
		{
			_signalChain.Broadcast<object>(message);
		}
	}
}
