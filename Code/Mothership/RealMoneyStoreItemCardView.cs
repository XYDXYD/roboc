using Game.ECS.GUI.Components;
using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RealMoneyStoreItemCardView : MonoBehaviour, IButtonComponent
	{
		public enum PremiumDisplayStatus
		{
			LifeTime,
			TimeLimited
		}

		public RealMoneyStoreExtraDataScriptableObject SpriteData;

		[SerializeField]
		private GameObject LifeTimeSprite;

		[SerializeField]
		private GameObject TimeLimitedSprite;

		[SerializeField]
		private GameObject AlreadyOwnedOverlay;

		[SerializeField]
		private GameObject NotAvailableOverlay;

		[SerializeField]
		private GameObject BestValueOverlay;

		[SerializeField]
		private GameObject MostPopularOverlay;

		[SerializeField]
		private GameObject PercentSaveLabelOverlay;

		[SerializeField]
		private GameObject PercentDiscountLabelOverlay;

		[SerializeField]
		private GameObject OriginalPriceWidget;

		[SerializeField]
		private GameObject BottomUI;

		[SerializeField]
		private UILabel PromotionLabel;

		[SerializeField]
		private UILabel RealPriceLabel;

		[SerializeField]
		private UILabel ItemNameLabel;

		[SerializeField]
		private UILabel SavePercentageLabel;

		[SerializeField]
		private UILabel DiscountPercentageLabel;

		[Tooltip("When on discount, this is the crossed-out price")]
		[SerializeField]
		private UILabel OriginalPriceLabel;

		[SerializeField]
		private UISprite BackgroundSprite;

		[SerializeField]
		private UILabel PremiumTimeQuantity;

		[SerializeField]
		private UILabel PremiumTimeDenomination;

		[SerializeField]
		private UIButton uiButton;

		[Inject]
		internal IRealMoneyStoreCardController storeCardController
		{
			private get;
			set;
		}

		public DispatchOnChange<bool> buttonPressed
		{
			get;
			private set;
		}

		public RealMoneyStoreItemCardView()
			: this()
		{
		}

		public unsafe void Initialise(int slotId, RealMoneyStoreSlotDisplayType viewDisplayType, IRealMoneyStoreItemDataSource dataSource)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			storeCardController.RegisterView(this, slotId, viewDisplayType);
			storeCardController.SetDataSource(dataSource);
			buttonPressed = new DispatchOnChange<bool>(slotId);
			uiButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			ResetBannerOverlays();
		}

		public void ResetBannerOverlays()
		{
			if (BestValueOverlay != null)
			{
				BestValueOverlay.SetActive(false);
			}
			if (PercentDiscountLabelOverlay != null)
			{
				PercentDiscountLabelOverlay.SetActive(false);
			}
			if (MostPopularOverlay != null)
			{
				MostPopularOverlay.SetActive(false);
			}
			if (PercentSaveLabelOverlay != null)
			{
				PercentSaveLabelOverlay.SetActive(false);
			}
		}

		public void ShowLifetimePremium()
		{
			LifeTimeSprite.SetActive(true);
			TimeLimitedSprite.SetActive(false);
		}

		public void SetSpriteBySku(string shopItemBundleSku)
		{
			int num = -1;
			for (int i = 0; i < SpriteData.StoreItemSkus.Length; i++)
			{
				if (SpriteData.StoreItemSkus[i].CompareTo(shopItemBundleSku) == 0)
				{
					num = i;
				}
			}
			BackgroundSprite.set_spriteName(SpriteData.SpriteNames[num]);
		}

		public void ShowTimeLimitedPremium(int numberOfDays)
		{
			LifeTimeSprite.SetActive(false);
			TimeLimitedSprite.SetActive(true);
			int num = numberOfDays % 30;
			int num2 = numberOfDays / 30;
			int num3 = numberOfDays % 7;
			int num4 = numberOfDays / 7;
			if (num == 0)
			{
				PremiumTimeQuantity.set_text(num2.ToString());
				if (num2 == 1)
				{
					PremiumTimeDenomination.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreTimeLabelMonthsSingular"));
				}
				else
				{
					PremiumTimeDenomination.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreTimeLabelMonthsPlural"));
				}
			}
			else if (num3 == 0)
			{
				PremiumTimeQuantity.set_text(num4.ToString());
				if (num4 == 1)
				{
					PremiumTimeDenomination.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreTimeLabelWeeksSingular"));
				}
				else
				{
					PremiumTimeDenomination.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreTimeLabelWeeksPlural"));
				}
			}
			else
			{
				PremiumTimeQuantity.set_text(numberOfDays.ToString());
				if (numberOfDays == 1)
				{
					PremiumTimeDenomination.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreTimeLabelDaysSingular"));
				}
				else
				{
					PremiumTimeDenomination.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreTimeLabelDaysPlural"));
				}
			}
		}

		public void SetAlreadyOwned(bool status)
		{
			if (AlreadyOwnedOverlay != null)
			{
				AlreadyOwnedOverlay.SetActive(status);
			}
		}

		public void SetAvailable(bool available)
		{
			if (!(NotAvailableOverlay != null))
			{
				return;
			}
			NotAvailableOverlay.SetActive(!available);
			if (!available)
			{
				if (BestValueOverlay != null)
				{
					BestValueOverlay.SetActive(true);
				}
				if (MostPopularOverlay != null)
				{
					MostPopularOverlay.SetActive(true);
				}
				if (PercentDiscountLabelOverlay != null)
				{
					PercentDiscountLabelOverlay.SetActive(false);
				}
				if (PercentSaveLabelOverlay != null)
				{
					PercentSaveLabelOverlay.SetActive(false);
				}
				if (AlreadyOwnedOverlay != null)
				{
					AlreadyOwnedOverlay.SetActive(false);
				}
				if (BottomUI != null)
				{
					BottomUI.SetActive(false);
				}
				OriginalPriceWidget.SetActive(false);
				RealPriceLabel.set_enabled(false);
				PromotionLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strSeasonEndedTitle"));
			}
			else
			{
				if (BottomUI != null)
				{
					BottomUI.SetActive(true);
				}
				RealPriceLabel.set_enabled(true);
				PromotionLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStorePromotionTitle"));
			}
		}

		public void SetPriceText(string priceLabel)
		{
			RealPriceLabel.set_text(priceLabel);
		}

		public void ShowBestValueOverlay()
		{
			if (BestValueOverlay != null)
			{
				BestValueOverlay.SetActive(true);
			}
		}

		public void ShowMostPopularOverlay()
		{
			if (MostPopularOverlay != null)
			{
				MostPopularOverlay.SetActive(true);
			}
		}

		public void ShowSavingOverlay(int savingPercentage)
		{
			if (PercentSaveLabelOverlay != null)
			{
				PercentSaveLabelOverlay.SetActive(true);
				SavePercentageLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreSavingPercentage"), savingPercentage));
			}
		}

		public void ShowDiscountOverlay(int discountPercentage)
		{
			PercentDiscountLabelOverlay.SetActive(true);
			DiscountPercentageLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreDiscountPercentage"), discountPercentage));
		}

		public void ShowOriginalPriceWidget(bool visible)
		{
			OriginalPriceWidget.SetActive(visible);
		}

		public void SetOriginalPrice(string priceLabel)
		{
			OriginalPriceLabel.set_text(priceLabel);
		}

		public void SetItemName(string itemName)
		{
			ItemNameLabel.set_text(itemName);
		}

		private void BuyItemClicked()
		{
			buttonPressed.set_value(true);
			buttonPressed.set_value(false);
		}
	}
}
