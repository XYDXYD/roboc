using Services.Simulation;
using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class WeaponOrderPresenter : IHudElement, IWaitForFrameworkDestruction
	{
		private HUDWeaponOrder _view;

		private Dictionary<int, WeaponOrderButton> _weaponOrderButtons = new Dictionary<int, WeaponOrderButton>();

		private WeaponOrderButton[] _buttons;

		[Inject]
		internal ItemDescriptorSpriteUtility itemDescriptorUtility
		{
			private get;
			set;
		}

		[Inject]
		internal IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public void SetView(HUDWeaponOrder view)
		{
			_view = view;
			_buttons = view.buttons;
			battleHudStyleController.AddHud(this);
		}

		public void ShowWeaponOrder(WeaponOrderSimulation weaponOrder)
		{
			_weaponOrderButtons.Clear();
			for (int i = 0; i < _buttons.Length; i++)
			{
				WeaponOrderButton weaponOrderButton = _buttons[i];
				int itemDescriptorKeyByIndex = weaponOrder.GetItemDescriptorKeyByIndex(i);
				if (itemDescriptorKeyByIndex == 0)
				{
					weaponOrderButton.ShowInactive();
					continue;
				}
				weaponOrderButton.Reset();
				weaponOrderButton.ShowDestroyed(playSound: false);
				weaponOrderButton.itemDescriptorKey = itemDescriptorKeyByIndex;
				weaponOrderButton.hotkeyLabel.set_text((i + 1).ToString());
				weaponOrderButton.weaponUISprite.set_spriteName(itemDescriptorUtility.GetSprite(itemDescriptorKeyByIndex));
				_weaponOrderButtons.Add(itemDescriptorKeyByIndex, weaponOrderButton);
			}
			_view.SetActive(active: true);
		}

		public void ShowSelectedItem(int itemDescriptorKey)
		{
			Dictionary<int, WeaponOrderButton>.Enumerator enumerator = _weaponOrderButtons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WeaponOrderButton value = enumerator.Current.Value;
				value.ShowSelected(value.itemDescriptorKey == itemDescriptorKey);
			}
		}

		public void ShowActivatableModule(ItemCategory moduleType, bool activable)
		{
			WeaponOrderButton buttonByModuleType = GetButtonByModuleType(moduleType);
			if (buttonByModuleType != null)
			{
				buttonByModuleType.ShowSelected(activable);
			}
		}

		public void SetCooldown(ItemCategory moduleType, float cooldownDuration)
		{
			WeaponOrderButton buttonByModuleType = GetButtonByModuleType(moduleType);
			if (buttonByModuleType != null)
			{
				buttonByModuleType.StartCooldown(cooldownDuration);
			}
		}

		public void SetModuleCooldownTime(ItemCategory moduleType, float time)
		{
			WeaponOrderButton buttonByModuleType = GetButtonByModuleType(moduleType);
			if (buttonByModuleType != null)
			{
				buttonByModuleType.SetTime(time);
			}
		}

		public void ResetCooldown(ItemCategory moduleType)
		{
			WeaponOrderButton buttonByModuleType = GetButtonByModuleType(moduleType);
			if (buttonByModuleType != null)
			{
				buttonByModuleType.ResetCooldown();
			}
		}

		public void PlayNotEnoughPowerAnimation(ItemCategory moduleType)
		{
			WeaponOrderButton buttonByModuleType = GetButtonByModuleType(moduleType);
			if (buttonByModuleType != null)
			{
				buttonByModuleType.PlayNotEnoughPowerAnimation();
			}
		}

		public void StartCooldown(ItemDescriptor itemDescriptor, float cooldownTime)
		{
			int key = itemDescriptor.GenerateKey();
			if (_weaponOrderButtons.ContainsKey(key))
			{
				WeaponOrderButton weaponOrderButton = _weaponOrderButtons[key];
				weaponOrderButton.StartCooldown(cooldownTime);
			}
		}

		public void SetWeaponCooldownTime(ItemDescriptor itemDescriptor, float time)
		{
			if (_weaponOrderButtons.TryGetValue(itemDescriptor.GenerateKey(), out WeaponOrderButton value))
			{
				value.SetTime(time);
			}
		}

		public void ResetCooldown(ItemDescriptor itemDescriptor)
		{
			int key = itemDescriptor.GenerateKey();
			if (_weaponOrderButtons.ContainsKey(key))
			{
				WeaponOrderButton weaponOrderButton = _weaponOrderButtons[key];
				weaponOrderButton.ResetCooldown();
			}
		}

		public void PlayNotEnoughPowerAnimation(ItemDescriptor itemDescriptor)
		{
			int key = itemDescriptor.GenerateKey();
			if (_weaponOrderButtons.ContainsKey(key))
			{
				WeaponOrderButton weaponOrderButton = _weaponOrderButtons[key];
				weaponOrderButton.PlayNotEnoughPowerAnimation();
			}
		}

		public void ShowDestroyedWeaponCategory(ItemDescriptor itemDescriptor)
		{
			int key = itemDescriptor.GenerateKey();
			if (_weaponOrderButtons.TryGetValue(key, out WeaponOrderButton value))
			{
				value.ShowDestroyed();
			}
		}

		public void ShowActiveWeaponCategory(ItemDescriptor itemDescriptor)
		{
			int key = itemDescriptor.GenerateKey();
			if (_weaponOrderButtons.TryGetValue(key, out WeaponOrderButton value))
			{
				value.ShowActive();
			}
		}

		public void ShowAllActive()
		{
			Dictionary<int, WeaponOrderButton>.Enumerator enumerator = _weaponOrderButtons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WeaponOrderButton value = enumerator.Current.Value;
				value.ShowActive();
			}
		}

		public void ResetAllAlpha()
		{
			Dictionary<int, WeaponOrderButton>.Enumerator enumerator = _weaponOrderButtons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.ResetAlpha();
			}
		}

		private WeaponOrderButton GetButtonByModuleType(ItemCategory itemCategory)
		{
			Dictionary<int, WeaponOrderButton>.Enumerator enumerator = _weaponOrderButtons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WeaponOrderButton value = enumerator.Current.Value;
				if (value.itemCategoryInt == (int)itemCategory)
				{
					return value;
				}
			}
			return null;
		}

		public void SetStyle(HudStyle style)
		{
			_view.SetStyle(style);
		}

		public void OnFrameworkDestroyed()
		{
			battleHudStyleController.RemoveHud(this);
		}
	}
}
