using InputMask;
using Services.Mothership;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;

namespace Mothership
{
	internal sealed class WeaponReorderDisplay : IGUIDisplay, ITickable, IComponent, ITickableBase
	{
		private WeaponReorderButton _selectedButton;

		private WeaponReorderCursor _cursor;

		private WeaponReorderView _view;

		private WeaponReorderButton[] _buttons;

		private int _newItemDescriptorKey;

		private bool _newWeaponAssigned = true;

		[Inject]
		internal ItemDescriptorSpriteUtility itemDescriptorSpriteUtility
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal WeaponOrderManager weaponOrderManager
		{
			private get;
			set;
		}

		[Inject]
		internal IInputActionMask inputActionMask
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.ManageLoadoutScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public event Action<WeaponReorderButton[]> onWeaponsReordered = delegate
		{
		};

		public event Action<int> onWeaponsDelete = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
		}

		GUIShowResult IGUIDisplay.Show()
		{
			InitWeaponOrder();
			_view.Show();
			return GUIShowResult.Showed;
		}

		bool IGUIDisplay.Hide()
		{
			if (_selectedButton != null)
			{
				_selectedButton.CancelDelete();
			}
			_cursor.Deactivate();
			OnClose();
			_view.Hide();
			return true;
		}

		bool IGUIDisplay.IsActive()
		{
			return internalIsActive();
		}

		private bool internalIsActive()
		{
			if (_view == null)
			{
				return false;
			}
			return _view.IsActive();
		}

		public void Tick(float deltaSec)
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.BuildModeInputAxis, 3) && InputRemapper.Instance.GetButtonDown("Manage Loadout") && ((guiInputController.GetActiveScreen() == GuiScreens.BuildMode && guiInputController.GetShortCutMode() == ShortCutMode.BuildShortCuts) || internalIsActive()))
			{
				guiInputController.ToggleScreen(GuiScreens.ManageLoadoutScreen);
			}
		}

		public void SetView(WeaponReorderView weaponReorderView)
		{
			_view = weaponReorderView;
			_buttons = weaponReorderView.buttons;
			_cursor = weaponReorderView.weaponCursor;
		}

		public void ShowNewItemDescriptor(int itemDescriptorKey)
		{
			_newWeaponAssigned = false;
			_newItemDescriptorKey = itemDescriptorKey;
			string sprite = itemDescriptorSpriteUtility.GetSprite(_newItemDescriptorKey);
			_cursor.ShowSpriteAndActivate(sprite);
			guiInputController.ShowScreen(GuiScreens.ManageLoadoutScreen);
		}

		public void ButtonClicked(ButtonType buttonType)
		{
			switch (buttonType)
			{
			case ButtonType.WeaponReorderDeleteConfirm:
				this.onWeaponsDelete(_selectedButton.itemDescriptorKey);
				UpdateButtons();
				_view.HideConfirmDeleteDialog();
				break;
			case ButtonType.Confirm:
				guiInputController.CloseCurrentScreen();
				break;
			case ButtonType.WeaponReorderDeleteCancel:
				_selectedButton.CancelDelete();
				_view.HideConfirmDeleteDialog();
				break;
			}
		}

		public void SlotClicked(WeaponReorderButton selectedButton)
		{
			_selectedButton = selectedButton;
			switch (selectedButton.buttonState)
			{
			case WeaponReorderButton.ButtonState.Select:
				SelectWeaponSlot(selectedButton);
				break;
			case WeaponReorderButton.ButtonState.SelectEmpty:
				OnEmptyButtonClicked(selectedButton);
				break;
			case WeaponReorderButton.ButtonState.Reorder:
				ReorderButton(selectedButton);
				this.onWeaponsReordered(_buttons);
				break;
			case WeaponReorderButton.ButtonState.Delete:
				ShowConfirmDelete(selectedButton);
				break;
			}
		}

		private void ShowConfirmDelete(WeaponReorderButton selectedButton)
		{
			int itemDescriptorKey = selectedButton.itemDescriptorKey;
			ItemDescriptorKey.GetItemInfoFromKey(itemDescriptorKey, out int itemCategory, out int itemSize);
			StringTable instance = StringTableBase<StringTable>.Instance;
			ItemSize itemSize2 = (ItemSize)itemSize;
			string arg = itemSize2.ToString();
			ItemCategory itemCategory2 = (ItemCategory)itemCategory;
			string @string = instance.GetString($"str{arg}{itemCategory2.ToString()}Name");
			string replaceString = StringTableBase<StringTable>.Instance.GetReplaceString("strWeaponReorderConfirmDeleteInfo", "[WeaponSubCategory]", @string);
			_view.ShowConfirmDeleteDialog(replaceString);
		}

		private void OnEmptyButtonClicked(WeaponReorderButton selectedButton)
		{
			if (!_newWeaponAssigned)
			{
				DisplayWeaponAtSlot(selectedButton, _newItemDescriptorKey);
				_cursor.Deactivate();
				_newWeaponAssigned = true;
				this.onWeaponsReordered(_buttons);
			}
			SelectWeaponSlot(selectedButton);
		}

		private void ReorderButton(WeaponReorderButton selectedButton)
		{
			int newIndex = selectedButton.newIndex;
			WeaponReorderButton weaponReorderButton = _buttons[newIndex];
			int itemDescriptorKey = weaponReorderButton.itemDescriptorKey;
			DisplayWeaponAtSlot(weaponReorderButton, selectedButton.itemDescriptorKey);
			DisplayWeaponAtSlot(selectedButton, itemDescriptorKey);
			SelectWeaponSlot(weaponReorderButton);
		}

		private void InitWeaponOrder()
		{
			UpdateButtons();
			SelectWeaponSlot(null);
			if (_newItemDescriptorKey == 0)
			{
				_cursor.Deactivate();
			}
		}

		private void UpdateButtons()
		{
			WeaponOrderMothership weaponOrder = weaponOrderManager.weaponOrder;
			for (int i = 0; i < _buttons.Length; i++)
			{
				int itemDescriptorKeyByIndex = weaponOrder.GetItemDescriptorKeyByIndex(i);
				WeaponReorderButton weaponReorderButton = _buttons[i];
				weaponReorderButton.index = i;
				weaponReorderButton.itemDescriptorKey = itemDescriptorKeyByIndex;
				DisplayWeaponAtSlot(weaponReorderButton, itemDescriptorKeyByIndex);
			}
		}

		private void SelectWeaponSlot(WeaponReorderButton selectedButt)
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				WeaponReorderButton weaponReorderButton = _buttons[i];
				if (selectedButt == null)
				{
					selectedButt = weaponReorderButton;
				}
				if (selectedButt == weaponReorderButton)
				{
					selectedButt.ShowSelectedSlot(selected: true);
					_view.HighlightButton(selectedButt.get_gameObject());
				}
				else
				{
					_buttons[i].ShowSelectedSlot(selected: false);
				}
			}
		}

		private void DisplayWeaponAtSlot(WeaponReorderButton butt, int itemDescriptorKey)
		{
			if (itemDescriptorKey == 0)
			{
				butt.ShowEmptyWeaponSlot();
				return;
			}
			ItemDescriptorKey.GetItemInfoFromKey(itemDescriptorKey, out int itemCategory, out int itemSize);
			StringTable instance = StringTableBase<StringTable>.Instance;
			ItemSize itemSize2 = (ItemSize)itemSize;
			string arg = itemSize2.ToString();
			ItemCategory itemCategory2 = (ItemCategory)itemCategory;
			string @string = instance.GetString($"str{arg}{itemCategory2.ToString()}Name");
			string sprite = itemDescriptorSpriteUtility.GetSprite(itemCategory, itemSize);
			string weaponTypeStr_ = @string;
			butt.ShowWeaponSlot(itemDescriptorKey, sprite, weaponTypeStr_);
		}

		private void OnClose()
		{
			if (_newWeaponAssigned)
			{
				this.onWeaponsReordered(_buttons);
			}
			else
			{
				this.onWeaponsDelete(_newItemDescriptorKey);
			}
		}
	}
}
