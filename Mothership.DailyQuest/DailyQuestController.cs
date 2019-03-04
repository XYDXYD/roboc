using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using Utility;

namespace Mothership.DailyQuest
{
	internal sealed class DailyQuestController
	{
		private PlayerDailyQuestsData _playerDailyQuestsData;

		[Inject]
		private IServiceRequestFactory serviceRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		private DailyQuestsObservable dailyQuestsObservable
		{
			get;
			set;
		}

		[Inject]
		private ICurrenciesTracker currenciesTracker
		{
			get;
			set;
		}

		[Inject]
		private DailyQuestAnalytics dailyQuestAnalytics
		{
			get;
			set;
		}

		public PlayerDailyQuestsData playerQuestData => _playerDailyQuestsData;

		public IEnumerator LoadData()
		{
			loadingIconPresenter.NotifyLoading("LoadingDailyQuests");
			TaskService<PlayerDailyQuestsData> task = serviceRequestFactory.Create<ILoadPlayerDailyQuestsRequest>().AsTask();
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("LoadingDailyQuests");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("LoadingDailyQuests");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("LoadingDailyQuests");
			_playerDailyQuestsData = task.result;
			yield return dailyQuestAnalytics.HandleAddQuestAnalytics(_playerDailyQuestsData.playerQuests);
			dailyQuestsObservable.Dispatch(ref _playerDailyQuestsData);
		}

		public IEnumerator ReplaceQuest(string questID)
		{
			loadingIconPresenter.NotifyLoading("ReplaceDailyQuests");
			TaskService task = serviceRequestFactory.Create<IReplaceDailyQuestRequest, string>(questID).AsTask();
			yield return task;
			loadingIconPresenter.NotifyLoadingDone("ReplaceDailyQuests");
			if (!task.succeeded && task.behaviour.errorCode == 21)
			{
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarningTitle"), StringTableBase<StringTable>.Instance.GetString("strErrorReplacingDailyQuestDesc")));
			}
			yield return dailyQuestAnalytics.HandleReplaceQuestAnalytics(_playerDailyQuestsData.playerQuests.Count, questID);
		}

		public IEnumerator ReceiveQuestRewards()
		{
			if (_playerDailyQuestsData.completedQuests.Count > 0)
			{
				loadingIconPresenter.NotifyLoading("MarkNotifiedCompletedDailyQuests");
				int previousLevel = -1;
				yield return PlayerLevelHelper.LoadCurrentPlayerLevel(serviceRequestFactory, delegate(PlayerLevelAndProgress playerLevelData)
				{
					previousLevel = (int)playerLevelData.playerLevel;
				}, delegate
				{
					Console.LogError("Could not load playerLevel for analytics");
				});
				long previousRobitsBalance = -1L;
				currenciesTracker.RetrieveCurrentWallet(delegate(Wallet wallet)
				{
					previousRobitsBalance = wallet.RobitsBalance;
				});
				TaskService task = serviceRequestFactory.Create<IMarkNotifiedCompletedQuestRequest>().AsTask();
				yield return new HandleTaskServiceWithError(task, delegate
				{
					loadingIconPresenter.NotifyLoading("MarkNotifiedCompletedDailyQuests");
				}, delegate
				{
					loadingIconPresenter.NotifyLoadingDone("MarkNotifiedCompletedDailyQuests");
				}).GetEnumerator();
				loadingIconPresenter.NotifyLoadingDone("MarkNotifiedCompletedDailyQuests");
				yield return dailyQuestAnalytics.HandleCompleteQuestAnalytics(_playerDailyQuestsData, previousRobitsBalance, previousLevel);
			}
		}
	}
}
