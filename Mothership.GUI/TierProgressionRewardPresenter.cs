using Achievements;
using Fabric;
using Services.Analytics;
using Services.Web.Photon;
using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership.GUI
{
	internal class TierProgressionRewardPresenter : IGUIDisplay, IComponent
	{
		private TierProgressNotificationData _data;

		private ITaskRoutine _animationTask;

		private TierProgressionRewardScreen _view;

		[Inject]
		public LoadingIconPresenter _loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory _requestFactory
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputController _guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public IAchievementManager _achievementManager
		{
			private get;
			set;
		}

		[Inject]
		public IAnalyticsRequestFactory _analyticsRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public LoadingIconPresenter _loadingPresenter
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

		public GuiScreens screenType => GuiScreens.LeagueRewardsScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public unsafe void SetView(TierProgressionRewardScreen view)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			_view = view;
			_view.get_gameObject().SetActive(false);
			EventDelegate.Add(_view.continueButton.onClick, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public IEnumerator LoadDataAndShowRewards()
		{
			IGetTierProgressNotificationRequest request = _requestFactory.Create<IGetTierProgressNotificationRequest>();
			TaskService<TierProgressNotificationData> task = new TaskService<TierProgressNotificationData>(request);
			_loadingIconPresenter.NotifyLoading("LeagueRewardsScreen");
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIconPresenter.NotifyLoading("LeagueRewardsScreen");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("LeagueRewardsScreen");
			}).GetEnumerator();
			_loadingIconPresenter.NotifyLoadingDone("LeagueRewardsScreen");
			TierProgressNotificationData data = task.result;
			if (data == null)
			{
				Console.Log("No league progression to show");
				yield break;
			}
			Console.Log("Showing league progression");
			_data = data;
			CheckAchievement();
			_view.tierLabelText = StringTableBase<StringTable>.Instance.GetReplaceString("strCurrentRankInTier", "{1}", (_data.tier + 1).ToString());
			_view.rankLabelText = RRAndTiers.GetRankDisplayableName(_data.after.rank);
			_view.iconTierLabelText = RRAndTiers.ConvertTierIndexToStringNoMegabotCheck((uint)_data.tier);
			_guiInputController.ShowScreen(screenType);
			_animationTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator(Animate(data));
			_animationTask.Start((Action<PausableTaskException>)null, (Action)null);
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
			_view.get_gameObject().SetActive(true);
			return GUIShowResult.Showed;
		}

		private IEnumerator Animate(TierProgressNotificationData data)
		{
			float sign = 1f;
			if (data.before.rank == data.after.rank && data.before.progressInRank > data.after.progressInRank)
			{
				sign = -1f;
			}
			if (sign > 0f)
			{
				SetBarProgress(_view.progressBarsStatic, data.before.rank, data.before.progressInRank);
				SetBarProgress(_view.progressBarsAnimated, data.before.rank, data.before.progressInRank);
			}
			else
			{
				SetBarProgress(_view.progressBarsStatic, data.after.rank, data.after.progressInRank);
				SetBarProgress(_view.progressBarsAnimated, data.before.rank, data.before.progressInRank);
			}
			for (int i = 0; i <= data.before.rank; i++)
			{
				_view.rankIcons[i].set_color(Color.get_white());
			}
			yield return (object)new WaitForSecondsEnumerator(_view.animationDelay);
			int rank = data.before.rank;
			float ratio = data.before.progressInRank;
			float targetRatio = (rank == data.after.rank) ? data.after.progressInRank : 1f;
			StartFillSound();
			while (true)
			{
				if ((!(sign > 0f) || !(ratio < targetRatio)) && (!(sign < 0f) || !(ratio > targetRatio)))
				{
					if (rank >= data.after.rank)
					{
						break;
					}
					rank++;
					ratio = 0f;
					AnimationClip val = _view.rankAnimations[rank - 1];
					_view.rankAnimationPlayer.PlayQueued(val.get_name());
					if (rank >= _view.progressBarsAnimated.Length)
					{
						break;
					}
					targetRatio = ((rank == data.after.rank) ? data.after.progressInRank : 1f);
				}
				else
				{
					ratio = Mathf.Clamp01(ratio + Time.get_deltaTime() * _view.barFillSpeed * sign);
					_view.progressBarsAnimated[rank].set_fillAmount(ratio);
					ModulateFillSound(ratio);
					yield return null;
				}
			}
			if (rank < _view.progressBarsAnimated.Length)
			{
				_view.progressBarsAnimated[rank].set_fillAmount(targetRatio);
			}
			StopFillSound();
		}

		private void SetBarProgress(UISprite[] bars, int rank, float progressInRank)
		{
			for (int i = 0; i < bars.Length; i++)
			{
				bars[i].set_fillAmount((i >= rank) ? 0f : 1f);
			}
			if (rank < bars.Length)
			{
				bars[rank].set_fillAmount(progressInRank);
			}
		}

		private void JumpToFinalState(TierProgressNotificationData data)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			SetBarProgress(_view.progressBarsAnimated, data.after.rank, data.after.progressInRank);
			for (int i = 0; i < data.after.rank; i++)
			{
				_view.rankIcons[i].set_color(Color.get_white());
			}
			StopFillSound();
		}

		private void StartFillSound()
		{
			EventManager.get_Instance().PostEvent("GUI_ProgressBar_Loop", 0, (object)null, _view.get_gameObject());
		}

		private void StopFillSound()
		{
			EventManager.get_Instance().PostEvent("GUI_ProgressBar_Loop", 1, (object)null, _view.get_gameObject());
		}

		private void ModulateFillSound(float ratio)
		{
			EventManager.get_Instance().SetParameter("GUI_ProgressBar_Loop", "FILL", ratio, _view.get_gameObject());
		}

		private IEnumerator OnAskClose()
		{
			_animationTask.Stop();
			JumpToFinalState(_data);
			IMarkTierProgressNotificationSeenRequest request = _requestFactory.Create<IMarkTierProgressNotificationSeenRequest>();
			TaskService task = new TaskService(request);
			_loadingIconPresenter.NotifyLoading("LeagueRewardsScreen");
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIconPresenter.NotifyLoading("LeagueRewardsScreen");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("LeagueRewardsScreen");
			}).GetEnumerator();
			_loadingIconPresenter.NotifyLoadingDone("LeagueRewardsScreen");
			yield return HandleAnalytics();
			_guiInputController.CloseCurrentScreen();
		}

		private IEnumerator HandleAnalytics()
		{
			if (_data.after.rank > _data.before.rank)
			{
				_loadingPresenter.NotifyLoading("HandleAnalytics");
				string rank = RRAndTiers.GetRankDisplayableNameForAnalytics(_data.after.rank);
				uint tier = Convert.ToUInt32(_data.tier) + 1;
				LogTierRankUpDependency tierRankUpDependency = new LogTierRankUpDependency(tier, rank);
				TaskService logTierRankUpRequest = _analyticsRequestFactory.Create<ILogTierRankUpRequest, LogTierRankUpDependency>(tierRankUpDependency).AsTask();
				yield return logTierRankUpRequest;
				if (!logTierRankUpRequest.succeeded)
				{
					Console.LogError("Log Tier Rank Up Request failed. " + logTierRankUpRequest.behaviour.exceptionThrown);
				}
				_loadingPresenter.NotifyLoadingDone("HandleAnalytics");
			}
		}

		private void CheckAchievement()
		{
			if (_data.after.rank > _data.before.rank)
			{
				_achievementManager.ReachedRank(Convert.ToUInt32(_data.after.rank));
			}
			else
			{
				_achievementManager.ReachedRank(0u);
			}
		}

		public bool Hide()
		{
			_view.get_gameObject().SetActive(false);
			return true;
		}
	}
}
