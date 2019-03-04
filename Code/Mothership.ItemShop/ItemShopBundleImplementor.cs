using Game.ECS.GUI.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership.ItemShop
{
	internal sealed class ItemShopBundleImplementor : MonoBehaviour, IItemShopBundleComponent, IItemShopBundleGuiComponent, IButtonComponent
	{
		[SerializeField]
		private UISprite iconSprite;

		[SerializeField]
		private UISprite fullSizeIconSprite;

		[SerializeField]
		private UILabel productCategoryLabel;

		[SerializeField]
		private UILabel productNameLabel;

		[SerializeField]
		private UIButton uiButton;

		[SerializeField]
		private GameObject lockedGO;

		[SerializeField]
		private GameObject robitsGO;

		[SerializeField]
		private GameObject ccGO;

		[SerializeField]
		private GameObject limitedEditionGO;

		[SerializeField]
		private UILabel costLabel;

		[SerializeField]
		private GameObject discountGO;

		[SerializeField]
		private UILabel discountPercentLabel;

		[SerializeField]
		private GameObject nonDiscountedCostGO;

		[SerializeField]
		private UILabel nonDiscountedCostLabel;

		[SerializeField]
		private UILabel discountDaysLeftLabel;

		public DispatchOnChange<bool> buttonPressed
		{
			get;
			private set;
		}

		public ItemShopBundle bundle
		{
			get;
			set;
		}

		public CurrencyType currencyType
		{
			set
			{
				ccGO.SetActive(value == CurrencyType.CosmeticCredits);
				robitsGO.SetActive(value == CurrencyType.Robits);
			}
		}

		public string categoryText
		{
			set
			{
				productCategoryLabel.set_text(value);
			}
		}

		public bool limitedEdition
		{
			set
			{
				limitedEditionGO.SetActive(value);
			}
		}

		public bool discounted
		{
			set
			{
				discountGO.SetActive(value);
				nonDiscountedCostGO.SetActive(value);
			}
		}

		public string discountPercentText
		{
			set
			{
				discountPercentLabel.set_text(value);
			}
		}

		public string discountDaysLeftText
		{
			set
			{
				discountDaysLeftLabel.set_text(value);
			}
		}

		public string nonDiscountedCostText
		{
			set
			{
				nonDiscountedCostLabel.set_text(value);
			}
		}

		public string nameText
		{
			set
			{
				productNameLabel.set_text(value);
			}
		}

		public string costText
		{
			set
			{
				costLabel.set_text(value);
			}
		}

		public bool isFullSizeSprite
		{
			set
			{
				fullSizeIconSprite.get_gameObject().SetActive(value);
				iconSprite.get_gameObject().SetActive(!value);
			}
		}

		public string spriteName
		{
			set
			{
				if (fullSizeIconSprite.get_gameObject().get_activeSelf())
				{
					fullSizeIconSprite.set_spriteName(value);
				}
				else
				{
					iconSprite.set_spriteName(value);
				}
			}
		}

		public bool locked
		{
			set
			{
				lockedGO.SetActive(value);
			}
		}

		public ItemShopBundleImplementor()
			: this()
		{
		}

		public unsafe void Initialise(int entityId)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			buttonPressed = new DispatchOnChange<bool>(entityId);
			uiButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private unsafe void OnDestroy()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			uiButton.onClick.Remove(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		private void BuyBundleClicked()
		{
			buttonPressed.set_value(true);
			buttonPressed.set_value(false);
		}
	}
}
