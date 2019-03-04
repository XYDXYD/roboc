using Game.RoboPass.GUI.Implementors;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;

internal class PurchaseConfirmedDisplay : MonoBehaviour, IInitialize, IChainListener
{
	public RealMoneyStoreExtraDataScriptableObject SpriteData;

	[SerializeField]
	private UILabel Title;

	[SerializeField]
	private UILabel Description;

	[Header("RoboPass")]
	[SerializeField]
	private GameObject RewardTemplateGo;

	[SerializeField]
	private GameObject RoboPassGO;

	[SerializeField]
	private UIButton PageDecrementButton;

	[SerializeField]
	private UIButton PageIncrementButton;

	[SerializeField]
	private UIButtonSounds PageDecrementButtonSound;

	[SerializeField]
	private UIButtonSounds PageIncrementButtonSound;

	[Header("Cosmetic Credits")]
	[SerializeField]
	private GameObject CosmeticCreditsGO;

	[SerializeField]
	private UILabel CosmeticCreditsAmountLabel;

	[Header("Premium")]
	[SerializeField]
	private GameObject PremiumGO;

	[SerializeField]
	private GameObject LifeTimePremiumRosetteGO;

	[SerializeField]
	private GameObject TimeLimitedPremiumRosetteGO;

	[SerializeField]
	private UILabel PremiumTimeQuantity;

	[SerializeField]
	private UILabel PremiumTimeDenomination;

	private const int COLUMN_LIMIT = 4;

	private const string KEY_DESC_THANK = "strConfirmPurchaseDesc";

	private const string KEY_DESC_UNLOCKED = "strRoboPassPlusItemsEarned";

	private const string KEY_TITLE_THANK = "strThankyou";

	private const string KEY_TITLE_UNLOCKED = "strRoboPassUnlocked";

	private readonly Type TYPE = typeof(ButtonType);

	private readonly Color _enabledArrowHover = new Color(1f, 1f, 1f);

	private readonly Color _enabledArrowNormal = new Color(0.255f, 0.549f, 0.792f);

	private RoboPassRewardView[] _roboPassRewardViews;

	private RoboPassSeasonRewardData[] _roboPassDeluxeRewards;

	private int _currentPageIndex;

	private int _maxPageNumber;

	private int _maxSeasonGrades;

	private int _playerCurrentGradeIndex;

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceReqFactory
	{
		private get;
		set;
	}

	[Inject]
	internal PurchaseConfirmedController purchaseConfirmedController
	{
		private get;
		set;
	}

	public PurchaseConfirmedDisplay()
		: this()
	{
	}//IL_0020: Unknown result type (might be due to invalid IL or missing references)
	//IL_0025: Unknown result type (might be due to invalid IL or missing references)
	//IL_003a: Unknown result type (might be due to invalid IL or missing references)
	//IL_003f: Unknown result type (might be due to invalid IL or missing references)


	public void OnDependenciesInjected()
	{
		purchaseConfirmedController.SetView(this);
	}

	public void Listen(object message)
	{
		if (message.GetType() == TYPE)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.HideAPanel)
			{
				Hide();
			}
		}
	}

	public void RoboPassIncreasePageNumber()
	{
		if (_currentPageIndex + 1 < _maxPageNumber)
		{
			_currentPageIndex++;
			RefreshRewards();
		}
	}

	public void RoboPassDecreasePageNumber()
	{
		if (_currentPageIndex > 0)
		{
			_currentPageIndex--;
			RefreshRewards();
		}
	}

	internal void Show()
	{
		this.get_gameObject().SetActive(true);
	}

	internal void Hide()
	{
		this.get_gameObject().SetActive(false);
	}

	internal bool IsActive()
	{
		return this.get_gameObject().get_activeSelf();
	}

	internal void SetForLifeTimePremium()
	{
		SetTitleAndDescriptionStrings("strThankyou", "strConfirmPurchaseDesc");
		LifeTimePremiumRosetteGO.SetActive(true);
		TimeLimitedPremiumRosetteGO.SetActive(false);
		PremiumGO.SetActive(true);
		CosmeticCreditsGO.SetActive(false);
		RoboPassGO.SetActive(false);
	}

	internal void SetForPremiumTimeLimited(int numberOfDays)
	{
		SetTitleAndDescriptionStrings("strThankyou", "strConfirmPurchaseDesc");
		LifeTimePremiumRosetteGO.SetActive(false);
		TimeLimitedPremiumRosetteGO.SetActive(true);
		PremiumGO.SetActive(true);
		CosmeticCreditsGO.SetActive(false);
		RoboPassGO.SetActive(false);
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

	internal IEnumerator SetForRoboPass()
	{
		SetTitleAndDescriptionStrings("strRoboPassUnlocked", "strRoboPassPlusItemsEarned");
		LifeTimePremiumRosetteGO.SetActive(false);
		TimeLimitedPremiumRosetteGO.SetActive(false);
		CosmeticCreditsGO.SetActive(false);
		ILoadRoboPassSeasonConfigRequest loadRoboPassSeasonConfigReq = serviceReqFactory.Create<ILoadRoboPassSeasonConfigRequest>();
		TaskService<RoboPassSeasonData> loadRoboPassSeasonConfigTS = loadRoboPassSeasonConfigReq.AsTask();
		yield return loadRoboPassSeasonConfigTS;
		if (!loadRoboPassSeasonConfigTS.succeeded)
		{
			throw new Exception("Failed to get RoboPass season data");
		}
		ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = serviceReqFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
		TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
		yield return loadPlayerRoboPassSeasonTS;
		if (!loadPlayerRoboPassSeasonTS.succeeded)
		{
			throw new Exception("Failed to get RoboPass player data");
		}
		RoboPassSeasonData roboPassSeasonData = loadRoboPassSeasonConfigTS.result;
		RoboPassSeasonRewardData[][] gradesRewards = roboPassSeasonData.gradesRewards;
		_maxSeasonGrades = gradesRewards.Length;
		_roboPassDeluxeRewards = new RoboPassSeasonRewardData[_maxSeasonGrades];
		for (int i = 0; i < _maxSeasonGrades; i++)
		{
			RoboPassSeasonRewardData[] array = gradesRewards[i];
			RoboPassSeasonRewardData[] array2 = array;
			foreach (RoboPassSeasonRewardData roboPassSeasonRewardData in array2)
			{
				if (roboPassSeasonRewardData.isDeluxe)
				{
					_roboPassDeluxeRewards[i] = roboPassSeasonRewardData;
				}
			}
		}
		UIGrid roboPassUIGrid = RoboPassGO.GetComponentInChildren<UIGrid>();
		Transform rewardsUiGridTransform = roboPassUIGrid.get_transform();
		_playerCurrentGradeIndex = loadPlayerRoboPassSeasonTS.result.currentGradeIndex;
		_maxPageNumber = Mathf.CeilToInt((float)_playerCurrentGradeIndex / 4f);
		_roboPassRewardViews = new RoboPassRewardView[4];
		for (int k = 0; k < 4; k++)
		{
			GameObject val = gameObjectFactory.Build(RewardTemplateGo);
			RoboPassRewardView component = val.GetComponent<RoboPassRewardView>();
			_roboPassRewardViews[k] = component;
			component.Initialize(isDeluxeCell_: true);
			Transform transform = val.get_transform();
			transform.set_parent(rewardsUiGridTransform);
			transform.set_localPosition(Vector3.get_zero());
			transform.set_localRotation(Quaternion.get_identity());
			transform.set_localScale(Vector3.get_one());
			component.rewardGradeLabel = (k + 1).ToString();
			RoboPassSeasonRewardData roboPassSeasonRewardData2 = _roboPassDeluxeRewards[k];
			component.isSpriteFullSize = roboPassSeasonRewardData2.spriteFullSize;
			component.rewardName = roboPassSeasonRewardData2.rewardName;
			component.rewardSprite = roboPassSeasonRewardData2.spriteName;
			component.rewardType = roboPassSeasonRewardData2.categoryName;
			component.visible = true;
			val.SetActive(k <= _playerCurrentGradeIndex);
		}
		roboPassUIGrid.Reposition();
		if (_playerCurrentGradeIndex >= 4)
		{
			PageDecrementButton.get_transform().get_parent().get_gameObject()
				.SetActive(true);
		}
		_currentPageIndex = 0;
		DisableArrow(PageDecrementButton, PageDecrementButtonSound);
		EnableArrow(PageIncrementButton, PageDecrementButtonSound);
		RoboPassGO.SetActive(true);
		PremiumGO.SetActive(false);
	}

	internal void SetForCosmeticCredits(int ccQuantity)
	{
		SetTitleAndDescriptionStrings("strThankyou", "strConfirmPurchaseDesc");
		LifeTimePremiumRosetteGO.SetActive(false);
		TimeLimitedPremiumRosetteGO.SetActive(false);
		CosmeticCreditsGO.SetActive(true);
		RoboPassGO.SetActive(false);
		PremiumGO.SetActive(false);
		CosmeticCreditsAmountLabel.set_text(ccQuantity.ToString());
	}

	private void SetTitleAndDescriptionStrings(string titleKey, string descKey)
	{
		string @string = StringTableBase<StringTable>.Instance.GetString(titleKey);
		Title.set_text(@string);
		string string2 = StringTableBase<StringTable>.Instance.GetString(descKey);
		Description.set_text(string2);
	}

	private void RefreshRewards()
	{
		if (_currentPageIndex + 1 >= _maxPageNumber)
		{
			DisableArrow(PageIncrementButton, PageIncrementButtonSound);
		}
		else
		{
			EnableArrow(PageIncrementButton, PageIncrementButtonSound);
		}
		if (_currentPageIndex <= 0)
		{
			DisableArrow(PageDecrementButton, PageDecrementButtonSound);
		}
		else
		{
			EnableArrow(PageDecrementButton, PageDecrementButtonSound);
		}
		int num = 4 * _currentPageIndex;
		int num2 = num;
		for (int i = 0; i < 4; i++)
		{
			RoboPassRewardView roboPassRewardView = _roboPassRewardViews[i];
			if (num2 <= _playerCurrentGradeIndex && num2 < _maxSeasonGrades)
			{
				RoboPassSeasonRewardData roboPassSeasonRewardData = _roboPassDeluxeRewards[num2];
				roboPassRewardView.isSpriteFullSize = roboPassSeasonRewardData.spriteFullSize;
				roboPassRewardView.rewardName = roboPassSeasonRewardData.rewardName;
				roboPassRewardView.rewardSprite = roboPassSeasonRewardData.spriteName;
				roboPassRewardView.rewardType = roboPassSeasonRewardData.categoryName;
				roboPassRewardView.rewardGradeLabel = (num2 + 1).ToString();
				roboPassRewardView.visible = true;
			}
			else
			{
				roboPassRewardView.visible = false;
			}
			num2++;
		}
	}

	private void EnableArrow(UIButtonColor pageButton, UIButtonSounds pageButtonSound)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Color disabledColor = pageButton.disabledColor;
		pageButton.set_defaultColor(_enabledArrowNormal);
		pageButton.hover = _enabledArrowHover;
		pageButton.pressed = _enabledArrowHover;
		pageButtonSound.playOnClick = true;
		pageButtonSound.playOnHover = true;
	}

	private static void DisableArrow(UIButtonColor pageButton, UIButtonSounds pageButtonSound)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Color disabledColor = pageButton.disabledColor;
		pageButton.set_defaultColor(disabledColor);
		pageButton.hover = disabledColor;
		pageButton.pressed = disabledColor;
		pageButtonSound.playOnClick = false;
		pageButtonSound.playOnHover = false;
	}
}
