using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class WeaponReorderView : MonoBehaviour, IInitialize, IChainListener
	{
		public WeaponReorderButton[] buttons;

		public WeaponReorderCursor weaponCursor;

		public GameObject confirmDeleteDialog;

		public UILabel confirmDeleteInfoLabel;

		public Transform toggleButtonGroup;

		[Inject]
		internal WeaponReorderDisplay weaponReorderDisplay
		{
			private get;
			set;
		}

		public WeaponReorderView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			weaponReorderDisplay.SetView(this);
		}

		void IChainListener.Listen(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)Convert.ToInt32(message);
				weaponReorderDisplay.ButtonClicked(buttonType);
			}
			else if (message is WeaponReorderButton)
			{
				weaponReorderDisplay.SlotClicked(message as WeaponReorderButton);
			}
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
			confirmDeleteDialog.SetActive(false);
		}

		public bool IsActive()
		{
			return this.get_gameObject().get_activeSelf();
		}

		public void ShowConfirmDeleteDialog(string infoString)
		{
			confirmDeleteDialog.SetActive(true);
			confirmDeleteInfoLabel.set_text(infoString);
		}

		public void HideConfirmDeleteDialog()
		{
			confirmDeleteDialog.SetActive(false);
		}

		public void HighlightButton(GameObject slot)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			new SignalChain(toggleButtonGroup).DeepBroadcast<GameObject>(slot);
		}
	}
}
