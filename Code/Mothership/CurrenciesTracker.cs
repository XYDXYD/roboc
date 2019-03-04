using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal sealed class CurrenciesTracker : ICurrenciesTracker, IWaitForFrameworkInitialization
	{
		internal Action<Wallet> OnWalletBalanceChanged;

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal CosmeticCreditsObservable cosmeticCreditsObservable
		{
			private get;
			set;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshUserWalletEnumerator);
		}

		public void RegisterWalletChangedListener(Action<Wallet> onWalletChangedCallback)
		{
			OnWalletBalanceChanged = (Action<Wallet>)Delegate.Combine(OnWalletBalanceChanged, onWalletChangedCallback);
			RetrieveCurrentWallet(onWalletChangedCallback);
		}

		public void UnRegisterWalletChangedListener(Action<Wallet> onWalletChangedCallback)
		{
			OnWalletBalanceChanged = (Action<Wallet>)Delegate.Remove(OnWalletBalanceChanged, onWalletChangedCallback);
		}

		public void RetrieveCurrentWallet(Action<Wallet> callbackOnLoaded)
		{
			loadingIconPresenter.NotifyLoading("WalletTrackerService");
			IServiceRequest serviceRequest = serviceFactory.Create<ILoadWalletRequest>().SetAnswer(new ServiceAnswer<Wallet>(delegate(Wallet userWallet)
			{
				loadingIconPresenter.NotifyLoadingDone("WalletTrackerService");
				callbackOnLoaded(userWallet);
			}, OnWalletLoadFailed));
			serviceRequest.Execute();
		}

		public void RefreshWallet(Action<Wallet> callback = null)
		{
			loadingIconPresenter.NotifyLoading("WalletTrackerService");
			ILoadWalletRequest loadWalletRequest = serviceFactory.Create<ILoadWalletRequest>();
			loadWalletRequest.ClearCache();
			ServiceAnswer<Wallet> answer = new ServiceAnswer<Wallet>(delegate(Wallet userWallet)
			{
				Console.Log("User wallet refreshed");
				loadingIconPresenter.NotifyLoadingDone("WalletTrackerService");
				if (OnWalletBalanceChanged != null)
				{
					OnWalletBalanceChanged(userWallet);
				}
				long cosmeticCreditsBalance = userWallet.CosmeticCreditsBalance;
				cosmeticCreditsObservable.Dispatch(ref cosmeticCreditsBalance);
				if (callback != null)
				{
					callback(userWallet);
				}
			}, OnWalletLoadFailed);
			IServiceRequest serviceRequest = loadWalletRequest.SetAnswer(answer);
			serviceRequest.Execute();
		}

		public IEnumerator RefreshUserWalletEnumerator()
		{
			ILoadWalletRequest loadWalletReq = serviceFactory.Create<ILoadWalletRequest>();
			loadWalletReq.ClearCache();
			TaskService<Wallet> loadWalletTS = new TaskService<Wallet>(loadWalletReq);
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadWalletTS, delegate
			{
				loadingIconPresenter.NotifyLoading("WalletTrackerService");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("WalletTrackerService");
			});
			yield return handleTSWithError.GetEnumerator();
			if (loadWalletTS.succeeded)
			{
				loadingIconPresenter.NotifyLoading("WalletTrackerService");
				Console.Log("User wallet refreshed");
				if (OnWalletBalanceChanged != null)
				{
					Wallet result = loadWalletTS.result;
					OnWalletBalanceChanged(result);
				}
				loadingIconPresenter.NotifyLoadingDone("WalletTrackerService");
			}
		}

		private void OnWalletLoadFailed(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("WalletTrackerService");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, StringTableBase<StringTable>.Instance.GetString("strRetry"), behaviour.alternativeText, delegate
			{
				loadingIconPresenter.NotifyLoading("WalletTrackerService");
				behaviour.MainBehaviour();
			}, behaviour.Alternative));
		}
	}
}
