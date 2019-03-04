using Services;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class WeaponReorderButton : MonoBehaviour, IChainListener
	{
		public enum ButtonState
		{
			Empty,
			Occupied,
			Delete,
			Reorder,
			Select,
			SelectEmpty
		}

		public int index;

		public int itemDescriptorKey;

		public UISprite weaponSprite;

		public UILabel hotkeyLabel;

		public UILabel weaponTypeLabel;

		public GameObject backgroundGO;

		public GameObject buttonsParentGO;

		public GameObject deleteButtonGO;

		public Transform listenerParent;

		private int _newIndex;

		private ButtonState _currentState;

		public ButtonState buttonState => _currentState;

		public int newIndex => _newIndex;

		public WeaponReorderButton()
			: this()
		{
		}

		void IChainListener.Listen(object message)
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			if (!(message is ButtonType))
			{
				return;
			}
			switch (Convert.ToInt32(message))
			{
			case 37:
				ChangeState(ButtonState.Delete);
				break;
			case 38:
				_newIndex = (_newIndex - 1) % WeaponOrder.MAX_WEAPON_CATEGORY_PER_MACHINE;
				ChangeState(ButtonState.Reorder);
				break;
			case 48:
				_newIndex = (_newIndex + 1) % WeaponOrder.MAX_WEAPON_CATEGORY_PER_MACHINE;
				ChangeState(ButtonState.Reorder);
				break;
			case 50:
				if (_currentState == ButtonState.Empty || _currentState == ButtonState.SelectEmpty)
				{
					ChangeState(ButtonState.SelectEmpty);
				}
				else
				{
					ChangeState(ButtonState.Select);
				}
				break;
			}
			if (buttonState != 0)
			{
				new SignalChain(listenerParent).Broadcast<WeaponReorderButton>(this);
			}
		}

		public void ShowEmptyWeaponSlot()
		{
			itemDescriptorKey = 0;
			weaponSprite.set_spriteName(string.Empty);
			weaponTypeLabel.set_text(string.Empty);
			buttonsParentGO.SetActive(false);
			ChangeState(ButtonState.Empty);
			_newIndex = index;
		}

		public void ShowSelectedSlot(bool selected)
		{
			bool active = itemDescriptorKey > 0;
			deleteButtonGO.SetActive(active);
			buttonsParentGO.SetActive(selected);
			_newIndex = index;
		}

		public void ShowWeaponSlot(int itemDescriptorKey_, string spriteName_, string weaponTypeStr_)
		{
			itemDescriptorKey = itemDescriptorKey_;
			weaponSprite.set_spriteName(spriteName_);
			weaponTypeLabel.set_text(weaponTypeStr_);
			_newIndex = index;
			ChangeState(ButtonState.Occupied);
		}

		public void CancelDelete()
		{
			ChangeState(ButtonState.Occupied);
		}

		private void ChangeState(ButtonState newState)
		{
			_currentState = newState;
		}
	}
}
