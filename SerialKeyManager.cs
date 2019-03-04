using Mothership;
using Services.Analytics;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

internal sealed class SerialKeyManager
{
	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ICurrenciesTracker robitsTracker
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
	internal ICubeInventory cubeInventory
	{
		private get;
		set;
	}

	[Inject]
	internal IAnalyticsRequestFactory analyticsRequestFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter _loadingIcon
	{
		private get;
		set;
	}

	public IEnumerator ApplySerialOrPromo(string code, Action<ApplyPromoCodeResponse> successCallback)
	{
		TaskService<ApplyPromoCodeResponse> applyTask = serviceFactory.Create<IApplyPromoCodeRequest, string>(code).AsTask();
		_loadingIcon.NotifyLoading("ApplyingPromoCode");
		yield return new HandleTaskServiceWithError(applyTask, delegate
		{
			_loadingIcon.NotifyLoading("ApplyingPromoCode");
		}, delegate
		{
			_loadingIcon.NotifyLoadingDone("ApplyingPromoCode");
		}).GetEnumerator();
		_loadingIcon.NotifyLoadingDone("ApplyingPromoCode");
		if (applyTask.succeeded)
		{
			ApplyPromoCodeResponse response = applyTask.result;
			if (!response.Success)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strApplyPromoCodeError"), GetErrorMessage(response.ResultCode)));
				yield break;
			}
			TaskRunner.get_Instance().Run(HandleAnalytics(response.PromoId, response.Value, response.BundleId));
			_loadingIcon.NotifyLoading("ApplyingPromoCode");
			robitsTracker.RefreshWallet();
			yield return premiumMembership.LoadPremiumUserState(forceRefresh: true);
			yield return cubeInventory.RefreshAndWait();
			_loadingIcon.NotifyLoadingDone("ApplyingPromoCode");
			successCallback(response);
		}
	}

	private static string GetErrorMessage(PromotionResultCode promotionErrorCode)
	{
		string key = "strErrorOccurred";
		switch (promotionErrorCode)
		{
		case PromotionResultCode.AlreadyAwarded:
			key = "strPromotionAlreadyUsed";
			break;
		case PromotionResultCode.Consumed:
			key = "strPromotionExhausted";
			break;
		case PromotionResultCode.Expired:
			key = "strPromotionExpired";
			break;
		case PromotionResultCode.InvalidBundleId:
			key = "strPromotionInvalid";
			break;
		case PromotionResultCode.InvalidPromotionId:
			key = "strPromotionInvalid";
			break;
		case PromotionResultCode.NotStarted:
			key = "strPromotionInactive";
			break;
		case PromotionResultCode.BundleIdAlreadyAwarded:
			key = "strPromotionBundleAlreadyAwarded";
			break;
		}
		return StringTableBase<StringTable>.Instance.GetString(key);
	}

	private IEnumerator HandleAnalytics(string promoId, float price, string bundleId)
	{
		LogPromoCodeActivatedDependency promoCodeActivatedDependency = new LogPromoCodeActivatedDependency(promoId, price, bundleId);
		TaskService logPromoCodeActivatedRequest = analyticsRequestFactory.Create<ILogPromoCodeActivatedRequest, LogPromoCodeActivatedDependency>(promoCodeActivatedDependency).AsTask();
		yield return logPromoCodeActivatedRequest;
		if (!logPromoCodeActivatedRequest.succeeded)
		{
			throw new Exception("Log Promo Code Activated Request failed", logPromoCodeActivatedRequest.behaviour.exceptionThrown);
		}
	}
}
