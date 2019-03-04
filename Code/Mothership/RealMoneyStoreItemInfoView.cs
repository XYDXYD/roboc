using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RealMoneyStoreItemInfoView : MonoBehaviour, IRealMoneyStoreInfoViewButtonComponents
	{
		public RealMoneyStoreExtraDataScriptableObject SpriteData;

		[SerializeField]
		private GameObject PremiumItemCardGO;

		[SerializeField]
		private GameObject CosmeticCreditsItemCardGO;

		[SerializeField]
		private GameObject RoboPassGO;

		[SerializeField]
		private GameObject LifeTimePremiumSprite;

		[SerializeField]
		private GameObject TimeLimitedPremiumSprite;

		[SerializeField]
		private UISprite CosmeticCreditBackgroundSprite;

		[SerializeField]
		private UILabel PremiumTimeDenomination;

		[SerializeField]
		private UILabel PremiumTimeQuantity;

		public GameObject RoboPassPossibleItemTemplateGO;

		public UIGrid RoboPassPossibleItemsContainer;

		[SerializeField]
		private UILabel ItemTitle;

		[SerializeField]
		private UILabel ItemDescription;

		[SerializeField]
		private UILabel ItemPrice;

		[SerializeField]
		private UIButton buyButton;

		[SerializeField]
		private UIButton gobackButton;

		public DispatchOnChange<bool> isShown
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> buyButtonPressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> goBackButtonPressed
		{
			get;
			private set;
		}

		public RealMoneyStoreItemInfoView()
			: this()
		{
		}

		public unsafe void Initialise(int id)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			isShown = new DispatchOnChange<bool>(id);
			buyButtonPressed = new DispatchOnChange<bool>(0);
			goBackButtonPressed = new DispatchOnChange<bool>(0);
			buyButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			gobackButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			RoboPassPossibleItemTemplateGO.SetActive(false);
		}

		public void BuyButtonClicked()
		{
			buyButtonPressed.set_value(true);
			buyButtonPressed.set_value(false);
		}

		public void GoBackButtonClicked()
		{
			goBackButtonPressed.set_value(true);
			goBackButtonPressed.set_value(false);
		}

		public void SetItemIcon(RealMoneyStoreSlotDisplayType slotDisplayType_)
		{
			if (PremiumItemCardGO != null)
			{
				PremiumItemCardGO.SetActive(slotDisplayType_ == RealMoneyStoreSlotDisplayType.PremiumRow);
			}
			if (CosmeticCreditsItemCardGO != null)
			{
				CosmeticCreditsItemCardGO.SetActive(slotDisplayType_ == RealMoneyStoreSlotDisplayType.CosmeticCreditsRow);
			}
			if (RoboPassGO != null)
			{
				RoboPassGO.SetActive(slotDisplayType_ == RealMoneyStoreSlotDisplayType.Robopass);
			}
		}

		public void ConfigureCosmeticCreditDetails(string shopItemBundleSku)
		{
			int num = -1;
			for (int i = 0; i < SpriteData.StoreItemSkus.Length; i++)
			{
				if (SpriteData.StoreItemSkus[i].CompareTo(shopItemBundleSku) == 0)
				{
					num = i;
				}
			}
			CosmeticCreditBackgroundSprite.set_spriteName(SpriteData.SpriteNames[num]);
		}

		public void ConfigurePremiumDetails(bool isLifeTimePremium, int numberOfDays)
		{
			if (isLifeTimePremium)
			{
				LifeTimePremiumSprite.SetActive(true);
				TimeLimitedPremiumSprite.SetActive(false);
				return;
			}
			LifeTimePremiumSprite.SetActive(false);
			TimeLimitedPremiumSprite.SetActive(true);
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

		public void SetItemInfo(string itemTitle_, string description_, string itemPrice_)
		{
			ItemTitle.set_text(itemTitle_);
			ItemDescription.set_text(description_);
			ItemPrice.set_text(itemPrice_);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
