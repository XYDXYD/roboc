using Services.Analytics;
using Services.TechTree;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership.GUI
{
	internal class TechPointsPresenter : IGUIDisplay, IComponent
	{
		private readonly ITaskRoutine _waitForEnterPressedTR;

		private TechPointsView _view;

		private TechPointProp _prop;

		[Inject]
		internal IGUIInputControllerMothership guiInputControllerMothership
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
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

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public HudStyle battleHudStyle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool doesntHideOnSwitch => false;

		public bool hasBackground => false;

		public bool isScreenBlurred => true;

		public GuiScreens screenType => GuiScreens.LevelRewards;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public TechPointsPresenter()
		{
			_waitForEnterPressedTR = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return _view != null && _view.get_gameObject().get_activeSelf();
		}

		public GUIShowResult Show()
		{
			_waitForEnterPressedTR.Start((Action<PausableTaskException>)null, (Action)null);
			_prop.Show();
			_view.Show();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_prop.Hide();
			_view.get_gameObject().SetActive(false);
			return true;
		}

		public void SetView(TechPointsView view)
		{
			_view = view;
		}

		public void SetProp(TechPointProp prop)
		{
			_prop = prop;
		}

		public void Close()
		{
			_waitForEnterPressedTR.Stop();
			guiInputControllerMothership.ShowScreen(GuiScreens.Garage);
		}

		public IEnumerator ShowTechPointAwards()
		{
			loadingIconPresenter.NotifyLoading("TechPointsAwardLoading");
			IGetTechPointsAwardRequest getAwardReq = serviceRequestFactory.Create<IGetTechPointsAwardRequest>();
			TaskService<int> getAwardTS = new TaskService<int>(getAwardReq);
			HandleTaskServiceWithError getAwardTSWithError = new HandleTaskServiceWithError(getAwardTS, delegate
			{
				loadingIconPresenter.NotifyLoading("TechPointsAwardLoading");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("TechPointsAwardLoading");
			});
			yield return getAwardTSWithError.GetEnumerator();
			if (!getAwardTS.succeeded)
			{
				loadingIconPresenter.NotifyLoadingDone("TechPointsAwardLoading");
				yield break;
			}
			int tp = getAwardTS.result;
			if (tp > 0)
			{
				int updatedTechPointsBalance = 0;
				yield return techPointsTracker.RefreshUserTechPointsAmountEnumerator(delegate(int balance)
				{
					updatedTechPointsBalance = balance;
				});
				IMarkTechPointsAwardAsSeenRequest markAwardsAsSeenReq = serviceRequestFactory.Create<IMarkTechPointsAwardAsSeenRequest>();
				TaskService markAwardsAsSeenTS = new TaskService(markAwardsAsSeenReq);
				HandleTaskServiceWithError markAwardsAsSeenTSWithError = new HandleTaskServiceWithError(markAwardsAsSeenTS, delegate
				{
					loadingIconPresenter.NotifyLoading("TechPointsAwardLoading");
				}, delegate
				{
					loadingIconPresenter.NotifyLoadingDone("TechPointsAwardLoading");
				});
				yield return markAwardsAsSeenTSWithError.GetEnumerator();
				loadingIconPresenter.NotifyLoadingDone("TechPointsAwardLoading");
				if (tp == 1)
				{
					_view.SetTechPointInfo(StringTableBase<StringTable>.Instance.GetString("strTechPointAwarded"));
				}
				else
				{
					_view.SetTechPointInfo(StringTableBase<StringTable>.Instance.GetString("strTechPointsAwarded"));
				}
				_view.SetTechPointCount($"{tp}x");
				yield return HandleAnalytics(tp, updatedTechPointsBalance);
				guiInputControllerMothership.ShowScreen(GuiScreens.LevelRewards);
				while (IsActive())
				{
					yield return null;
				}
				while (guiInputControllerMothership.GetActiveScreen() == GuiScreens.TechTree)
				{
					yield return null;
				}
			}
			else
			{
				loadingIconPresenter.NotifyLoadingDone("TechPointsAwardLoading");
			}
		}

		public void OpenTechTree()
		{
			guiInputControllerMothership.ShowScreen(GuiScreens.TechTree);
		}

		private IEnumerator HandleAnalytics(int earnedTechPoints, int techPointsBalance)
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			int playerLevel = 0;
			yield return PlayerLevelHelper.LoadCurrentPlayerLevel(serviceRequestFactory, delegate(PlayerLevelAndProgress playerLevelData)
			{
				playerLevel = (int)playerLevelData.playerLevel;
			}, delegate
			{
				Console.LogError("Could not load playerLevel for analytics");
			});
			LogPlayerCurrencyEarnedDependency playerCurrencyEarnedDependency = new LogPlayerCurrencyEarnedDependency("TechPoints", earnedTechPoints, techPointsBalance, 0, "LevelUp", playerLevel.ToString());
			TaskService logPlayerCurrencyEarnedRequest = analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(playerCurrencyEarnedDependency).AsTask();
			yield return logPlayerCurrencyEarnedRequest;
			if (!logPlayerCurrencyEarnedRequest.succeeded)
			{
				Console.LogError("Log Player Earned Currency Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private IEnumerator Update()
		{
			while (true)
			{
				if (Input.GetKeyDown(13))
				{
					Close();
				}
				yield return null;
			}
		}
	}
}
