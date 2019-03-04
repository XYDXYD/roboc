using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

internal sealed class PremiumMembership
{
	private bool _isLoading;

	private bool _loadedPremiumIncomeScales;

	private GameObject _loadingIcon;

	private DateTime _expirationTime;

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	public int premiumPercBonus
	{
		get;
		private set;
	}

	public bool hasSubscription
	{
		get;
		private set;
	}

	public bool hasPremiumForLife
	{
		get;
		private set;
	}

	public event Action<TimeSpan> onSubscriptionActivated = delegate
	{
	};

	public event Action onSubscriptionExpired = delegate
	{
	};

	public PremiumMembership()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
	}

	public TimeSpan GetLeftTime()
	{
		TimeSpan result = new TimeSpan(0, 0, 0);
		if (_expirationTime > DateTime.UtcNow)
		{
			return _expirationTime - DateTime.UtcNow;
		}
		return result;
	}

	public bool Loaded()
	{
		return !_isLoading && _loadedPremiumIncomeScales;
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			if (hasSubscription && DateTime.UtcNow > _expirationTime)
			{
				hasSubscription = false;
				this.onSubscriptionExpired();
			}
			yield return null;
		}
	}

	public IEnumerator Initialize()
	{
		yield return LoadPremiumCost();
		yield return LoadPremiumUserState();
	}

	public IEnumerator LoadPremiumUserState(bool forceRefresh = false)
	{
		ILoadPremiumDataRequest premiumInfo = serviceFactory.Create<ILoadPremiumDataRequest>();
		if (forceRefresh)
		{
			premiumInfo.ClearCache();
		}
		TaskService<PremiumInfoData> task = new TaskService<PremiumInfoData>(premiumInfo);
		yield return task;
		if (task.succeeded)
		{
			PremiumInfoData result = task.result;
			if (result.hasPremiumForLife || result.premiumTimeSpan != TimeSpan.Zero)
			{
				hasSubscription = true;
				hasPremiumForLife = result.hasPremiumForLife;
				_expirationTime = DateTime.UtcNow + result.premiumTimeSpan;
				this.onSubscriptionActivated(result.premiumTimeSpan);
			}
		}
		else
		{
			OnLoadingFailed(task.behaviour);
		}
	}

	private IEnumerator LoadPremiumCost()
	{
		_loadedPremiumIncomeScales = false;
		ILoadIncomeScalesPremiumFactorRequest incomeScaleRequest = serviceFactory.Create<ILoadIncomeScalesPremiumFactorRequest>();
		TaskService<IncomeScalesResponse> task = new TaskService<IncomeScalesResponse>(incomeScaleRequest);
		yield return task;
		if (task.succeeded)
		{
			OnIncomeScalesPremiumFactorLoaded(task.result);
		}
		else
		{
			OnLoadingFailed(task.behaviour);
		}
	}

	private void OnIncomeScalesPremiumFactorLoaded(IncomeScalesResponse data)
	{
		premiumPercBonus = data.PremiumXpBonusPercent;
		_loadedPremiumIncomeScales = true;
	}

	public void UpdatePremiumPurchase(PremiumPurchaseResponse response)
	{
		_isLoading = false;
		HideLoadingScreen();
		hasSubscription = true;
		hasPremiumForLife = response.hasPremiumForLife;
		_expirationTime = DateTime.UtcNow + response.timeLeft;
		this.onSubscriptionActivated(response.timeLeft);
	}

	private void OnLoadingFailed(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
		HideLoadingScreen();
		_isLoading = false;
	}

	public void ShowLoadingScreen()
	{
		_loadingIcon = gameObjectFactory.Build("FadeInLoadingDialog");
		_loadingIcon.SetActive(true);
	}

	public void HideLoadingScreen()
	{
		if (_loadingIcon != null)
		{
			Object.Destroy(_loadingIcon);
		}
	}
}
