using Mothership;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;

public sealed class UIShowUserBalance : MonoBehaviour
{
	[SerializeField]
	private UILabel techPointsUILabel;

	[SerializeField]
	private UILabel premiumRemainingTimeUILabel;

	[SerializeField]
	private UILabel robitsUILabel;

	[SerializeField]
	private UILabel cosmeticCreditsUILabel;

	private const string STR_ARGS = "{0}{1}";

	private StringBuilder _timeLeftSB;

	private WaitForSecondsEnumerator _waitForSecondsEnumerator01;

	private WaitForSecondsEnumerator _waitForSecondsEnumerator02;

	[Inject]
	internal ICurrenciesTracker currenciesTracker
	{
		private get;
		set;
	}

	[Inject]
	internal LocalisationWrapper localiseWrapper
	{
		private get;
		set;
	}

	[Inject]
	internal PremiumMembership premiumMembership
	{
		private get;
		set;
	}

	[Inject]
	internal TechPointsTracker techPointsTracker
	{
		private get;
		set;
	}

	public UIShowUserBalance()
		: this()
	{
	}

	private void Start()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		currenciesTracker.RegisterWalletChangedListener(SetCurrenciesLabel);
		if (techPointsUILabel != null)
		{
			techPointsTracker.OnUserTechPointsAmountChanged += SetTechPointsLabel;
		}
		if (premiumRemainingTimeUILabel != null)
		{
			_timeLeftSB = new StringBuilder();
			premiumRemainingTimeUILabel.set_text("---");
			premiumMembership.onSubscriptionActivated += ForcePremiumLabelUpdate;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(SetPremiumLabel));
			_waitForSecondsEnumerator01 = new WaitForSecondsEnumerator(1f);
			_waitForSecondsEnumerator02 = new WaitForSecondsEnumerator(29f);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdatePremiumTimer);
		}
	}

	private void OnDestroy()
	{
		if (techPointsUILabel != null)
		{
			techPointsTracker.OnUserTechPointsAmountChanged -= SetTechPointsLabel;
		}
		if (premiumRemainingTimeUILabel != null)
		{
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(SetPremiumLabel));
			premiumMembership.onSubscriptionActivated -= ForcePremiumLabelUpdate;
		}
		currenciesTracker.UnRegisterWalletChangedListener(SetCurrenciesLabel);
	}

	private void ForcePremiumLabelUpdate(TimeSpan t)
	{
		SetPremiumLabel();
	}

	private IEnumerator UpdatePremiumTimer()
	{
		while (true)
		{
			yield return _waitForSecondsEnumerator01;
			SetPremiumLabel();
			yield return _waitForSecondsEnumerator02;
		}
	}

	private void SetPremiumLabel()
	{
		if (premiumMembership.hasSubscription)
		{
			if (premiumMembership.hasPremiumForLife)
			{
				premiumRemainingTimeUILabel.set_text(StringTableBase<StringTable>.Instance.GetString("strTopBarPremiumForLife"));
				return;
			}
			TimeSpan leftTime = premiumMembership.GetLeftTime();
			_timeLeftSB.Length = 0;
			if (leftTime.TotalDays >= 1.0)
			{
				_timeLeftSB.AppendFormat("{0}{1}", GameUtility.CommaSeparate((int)Math.Ceiling(leftTime.TotalDays)), StringTableBase<StringTable>.Instance.GetString("strDayShort"));
			}
			else if (leftTime.Hours >= 1)
			{
				_timeLeftSB.AppendFormat("{0}{1}", GameUtility.CommaSeparate(leftTime.Hours), StringTableBase<StringTable>.Instance.GetString("strHrsShort"));
			}
			else
			{
				_timeLeftSB.AppendFormat("{0}{1}", GameUtility.CommaSeparate(leftTime.Minutes), StringTableBase<StringTable>.Instance.GetString("strMinsShort"));
			}
			premiumRemainingTimeUILabel.set_text(_timeLeftSB.ToString());
		}
		else
		{
			premiumRemainingTimeUILabel.set_text(string.Format("0{0}", StringTableBase<StringTable>.Instance.GetString("strDayShort")));
		}
	}

	private void SetCurrenciesLabel(Wallet userWallet)
	{
		robitsUILabel.set_text(GameUtility.CommaSeparate(userWallet.RobitsBalance));
		cosmeticCreditsUILabel.set_text(GameUtility.CommaSeparate(userWallet.CosmeticCreditsBalance));
	}

	private void SetTechPointsLabel(int techPoints)
	{
		techPointsUILabel.set_text(techPoints.ToString(CultureInfo.InvariantCulture));
	}
}
