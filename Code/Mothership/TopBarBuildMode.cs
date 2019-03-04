using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class TopBarBuildMode : MonoBehaviour
	{
		public GameObject inventoryButton;

		public UIToggleButtonGroup buttonGroup;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public TopBarBuildMode()
			: this()
		{
		}

		public void Build()
		{
			guiInputController.OnScreenStateChange += OnScreenStateChanged;
			Hide();
		}

		private void OnScreenStateChanged()
		{
			if (guiInputController.GetActiveScreen() == GuiScreens.InventoryScreen)
			{
				InventoryShown();
			}
		}

		private void InventoryShown()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			new SignalChain(this.get_transform()).DeepBroadcast<GameObject>(inventoryButton);
		}

		internal void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		internal void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
