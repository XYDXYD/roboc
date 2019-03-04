using Robocraft.GUI;
using ServerStateServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BrawlButtonPresenter : IWaitForFrameworkDestruction, IInitialize
	{
		private BrawlButton _view;

		private BrawlDetailsDataSource _dataSource;

		private IServiceEventContainer _serverStateEventContainer;

		private bool _lockedByBrawl;

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
		internal IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal IServerStateEventContainerFactory serverStateEventContainerFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal BrawlButtonFactory brawlButtonFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			get;
			set;
		}

		[Inject]
		internal MachineEditorBuilder machineBuilder
		{
			get;
			set;
		}

		public void SetView(BrawlButton view)
		{
			_view = view;
			SetButtonLocked(locked: true, megabot: false);
		}

		public unsafe IEnumerator LoadGUIData()
		{
			_dataSource = new BrawlDetailsDataSource(serviceFactory);
			brawlButtonFactory.BuildAll(_view, _dataSource);
			_dataSource.SetFallbackBrawlImage(_view.fallbackBrawlImage);
			_dataSource.InvalidateCache();
			_serverStateEventContainer = serverStateEventContainerFactory.Create();
			_serverStateEventContainer.ListenTo<IBrawlChangedEventListener, bool>(OnBrawlModeChanged);
			Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			yield return RefreshState();
		}

		public void OnDependenciesInjected()
		{
			machineBuilder.OnMachineBuilt += OnRobotBuilt;
		}

		private void OnRobotBuilt(uint cpu)
		{
			if (_view.get_gameObject().get_activeInHierarchy())
			{
				UpdateLockState();
			}
		}

		public unsafe void OnFrameworkDestroyed()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void RefreshLocalizedStrings()
		{
			_view.signalChain.DeepBroadcast<GenericComponentMessage>(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}

		private void OnBrawlModeChanged(bool locked)
		{
			_dataSource.InvalidateCache();
			if (locked)
			{
				_lockedByBrawl = true;
				UpdateLockState();
			}
			else
			{
				TaskRunner.get_Instance().Run(RefreshState());
			}
		}

		private IEnumerator RefreshState()
		{
			loadingIconPresenter.NotifyLoading("BrawlParameters");
			yield return cpuPower.IsLoadedEnumerator();
			yield return _dataSource.RefreshDataWithEmumerator(OnReceiveBrawlParameters, OnRefreshFailed);
		}

		private void OnReceiveBrawlParameters()
		{
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
			_lockedByBrawl = _dataSource.QueryData<bool>(0, 0);
			UpdateLockState();
			if (!_lockedByBrawl)
			{
				bool firstVictoryVisible = _dataSource.QueryData<bool>(11, 0);
				SetFirstVictoryVisible(firstVictoryVisible);
			}
		}

		private void OnRefreshFailed(ServiceBehaviour failBehaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
			ErrorWindow.ShowErrorWindow(new GenericErrorData("Failed to update Brawl info", "Failed to refresh Brawl info for the Brawl button", Localization.Get("strRetry", true), Localization.Get("strCancel", true), delegate
			{
				TaskRunner.get_Instance().Run(RefreshState());
			}, delegate
			{
			}));
		}

		private void UpdateLockState()
		{
			bool flag = cpuPower.TotalActualCPUCurrentRobot > cpuPower.MaxCpuPower;
			SetButtonLocked(_lockedByBrawl || flag, flag);
		}

		private void SetButtonLocked(bool locked, bool megabot)
		{
			_view.lockedGameObject.SetActive(locked);
			_view.rosette.SetActive(!locked);
			_view.lockMessage.SetActive(!megabot);
			_view.lockMessageMegabot.SetActive(megabot);
			SetButtonClickable(!locked);
		}

		private void SetFirstVictoryVisible(bool setting)
		{
			for (int i = 0; i < _view.bonusAdviceLabels.Length; i++)
			{
				_view.bonusAdviceLabels[i].SetActive(setting);
			}
		}

		private void SetButtonClickable(bool clickable)
		{
			_view.GetComponent<Collider>().set_enabled(clickable);
		}

		internal void OnViewEnable()
		{
			UpdateLockState();
		}

		public void OnPlayClicked()
		{
			TryQueueForBrawlCommand tryQueueForBrawlCommand = commandFactory.Build<TryQueueForBrawlCommand>();
			tryQueueForBrawlCommand.Execute();
		}

		public void OnDetailsClicked()
		{
			guiInputController.ToggleScreen(GuiScreens.BrawlDetails);
		}
	}
}
