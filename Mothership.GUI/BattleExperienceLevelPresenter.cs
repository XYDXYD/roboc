using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Fabric;
using Robocraft.GUI;
using Services.Analytics;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.GUI
{
	internal class BattleExperienceLevelPresenter
	{
		private BattleExperienceLevelView _view;

		private IDataSource _dataSource;

		private IDictionary<uint, uint> _localLevels;

		private int _visibleLevel;

		private Sequence _progressSequence;

		private DOGetter<float> _getVisibleExperienceProgress;

		private DOSetter<float> _setVisibleExperienceProgress;

		private TweenCallback _progressBeginCallback;

		private TweenCallback _levelCompleteCallback;

		private TweenCallback _progressCompleteCallback;

		[Inject]
		internal IServiceRequestFactory serviceFactory
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
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public void SetView(BattleExperienceLevelView view)
		{
			_view = view;
			PreallocateCallbacks();
		}

		public IEnumerator LoadGUIData()
		{
			loadingIconPresenter.NotifyLoading("BattleExperience");
			ILoadPlayerLevelDataRequest loadPlayerLevelsRequest = serviceFactory.Create<ILoadPlayerLevelDataRequest>();
			TaskService<IDictionary<uint, uint>> loadPlayerLevelsRequestTask = new TaskService<IDictionary<uint, uint>>(loadPlayerLevelsRequest);
			yield return new HandleTaskServiceWithError(loadPlayerLevelsRequestTask, delegate
			{
				loadingIconPresenter.NotifyLoading("BattleExperience");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("BattleExperience");
			}).GetEnumerator();
			loadingIconPresenter.NotifyLoadingDone("BattleExperience");
			_localLevels = loadPlayerLevelsRequestTask.result;
		}

		public void Listen(object message)
		{
			if (message is ShowBattleExperienceMessage)
			{
				InitializeProgress();
				PlayProgressAnimation(_view.animationDelay);
			}
		}

		public void SetDataSource(IDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		internal void InitializeProgress()
		{
			int num = _dataSource.QueryData<int>(7, 0);
			int num2 = num + _dataSource.QueryData<int>(8, 0);
			if (num < _localLevels.Count - 1)
			{
				int xp = _dataSource.QueryData<int>(6, 0);
				InitializeProgress(num, GetRelativeXP(num, xp));
			}
			else
			{
				InitializeProgress(num, 0f);
			}
			if (num2 > num)
			{
				TaskRunner.get_Instance().Run(HandleAnalytics(num2));
			}
		}

		private void InitializeProgress(int levelFrom, float xpFrom)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (_progressSequence != null)
			{
				KillProgressSequence();
			}
			_view.numberLabel.set_color(_view.previousProgressBar.get_color());
			SetVisiblePreviousProgress(xpFrom);
			_setVisibleExperienceProgress.Invoke(xpFrom);
			SetVisibleLevel(levelFrom);
		}

		internal void PlayProgressAnimation(float delay = 0f)
		{
			int num = _dataSource.QueryData<int>(7, 0);
			int num2 = num + _dataSource.QueryData<int>(8, 0);
			if (num + 1 < _localLevels.Count)
			{
				int num3 = _dataSource.QueryData<int>(6, 0);
				int num4 = _dataSource.QueryData<int>(5, 0);
				PlayProgressAnimation(num2, GetRelativeXP(num2, num3 + num4), delay);
			}
		}

		private void PlayProgressAnimation(int levelTo, float xpTo, float delay = 0f)
		{
			_progressSequence = DOTween.Sequence();
			if (delay > 0f)
			{
				TweenSettingsExtensions.AppendInterval(_progressSequence, delay);
			}
			TweenSettingsExtensions.AppendCallback(_progressSequence, _progressBeginCallback);
			float num;
			if (_visibleLevel < levelTo)
			{
				num = _view.levelFillDuration * (1f - _getVisibleExperienceProgress.Invoke());
				for (int i = _visibleLevel; i < levelTo; i++)
				{
					TweenSettingsExtensions.Append(_progressSequence, TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(_getVisibleExperienceProgress, _setVisibleExperienceProgress, 1f, num), 1), _levelCompleteCallback));
					num = _view.levelFillDuration;
				}
				num = _view.levelFillDuration * xpTo;
			}
			else
			{
				num = _view.levelFillDuration * (xpTo - _getVisibleExperienceProgress.Invoke());
			}
			TweenSettingsExtensions.OnComplete<Sequence>(TweenSettingsExtensions.Append(_progressSequence, DOTween.To(_getVisibleExperienceProgress, _setVisibleExperienceProgress, xpTo, num)), _progressCompleteCallback);
			TweenExtensions.Play<Sequence>(_progressSequence);
		}

		private float GetRelativeXP(int level, int xp)
		{
			if (level + 1 < _localLevels.Count)
			{
				int num = (int)_localLevels[(uint)level];
				int num2 = (int)_localLevels[(uint)(level + 1)];
				return (float)(xp - num) / (float)(num2 - num);
			}
			return 0f;
		}

		private unsafe void PreallocateCallbacks()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			_getVisibleExperienceProgress = new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			_setVisibleExperienceProgress = new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			_levelCompleteCallback = new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			_progressBeginCallback = new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			_progressCompleteCallback = new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
		}

		private void KillProgressSequence()
		{
			TweenExtensions.Kill(_progressSequence, false);
			_progressSequence = null;
		}

		private int GetVisibleLevel()
		{
			return _visibleLevel;
		}

		private void SetVisibleLevel(int level)
		{
			_visibleLevel = level;
			_view.numberLabel.set_text(level.ToString());
		}

		private void SetVisiblePreviousProgress(float ratio)
		{
			_view.previousProgressBar.set_fillAmount(ratio);
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

		private void PlayLevelUpSound()
		{
			EventManager.get_Instance().PostEvent("GUI_Level_UP", 0, (object)null, _view.get_gameObject());
		}

		private IEnumerator HandleAnalytics(int level)
		{
			IGetABTestGroupRequest abTestRequest = serviceFactory.Create<IGetABTestGroupRequest>();
			TaskService<ABTestData> abTestTask = abTestRequest.AsTask();
			yield return abTestTask;
			if (!abTestTask.succeeded)
			{
				throw new Exception("Retrieve AB test group failed", abTestTask.behaviour.exceptionThrown);
			}
			ABTestData abData = abTestTask.result;
			TaskService logLevelUpRequest = analyticsRequestFactory.Create<ILogLevelUpRequest, LogLevelUpDependency>(new LogLevelUpDependency(level, abData.ABTest, abData.ABTestGroup)).AsTask();
			yield return logLevelUpRequest;
			if (!logLevelUpRequest.succeeded)
			{
				throw new Exception("Log Level Up Request failed", logLevelUpRequest.behaviour.exceptionThrown);
			}
		}
	}
}
