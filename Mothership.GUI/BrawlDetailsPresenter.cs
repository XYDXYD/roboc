using Robocraft.GUI;
using ServerStateServiceLayer;
using Services;
using Simulation;
using Svelto.Command;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.GUI
{
	internal class BrawlDetailsPresenter : IGUIDisplay, IWaitForFrameworkDestruction, IComponent
	{
		private BrawlDetailsDataSource _dataSource;

		private IServiceEventContainer _serverStateEventContainer;

		internal BrawlDetailsView view
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
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
		internal LoadingIconPresenter loadingIconPresenter
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
		internal BrawlDetailsScreenFactory brawlDetailsScreenFactory
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

		public GuiScreens screenType => GuiScreens.BrawlDetails;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return view != null && view.get_gameObject().get_activeSelf();
		}

		public GUIShowResult Show()
		{
			view.get_gameObject().SetActive(true);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			view.get_gameObject().SetActive(false);
			return true;
		}

		public IEnumerator LoadGUIData()
		{
			_dataSource = new BrawlDetailsDataSource(serviceFactory);
			_dataSource.SetFallbackBrawlImage(view.fallbackBrawlImage);
			_dataSource.InvalidateCache();
			brawlDetailsScreenFactory.BuildAll(view, _dataSource);
			yield return Refresh();
			_serverStateEventContainer = serverStateEventContainerFactory.Create();
			_serverStateEventContainer.ListenTo<IBrawlChangedEventListener, bool>(OnBrawlModeChanged);
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(RefreshLocalizedStrings));
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(RefreshLocalizedStrings));
		}

		private void RefreshLocalizedStrings()
		{
			view.signalChain.DeepBroadcast<GenericComponentMessage>(new GenericComponentMessage(MessageType.RefreshData, "root", string.Empty));
		}

		private void OnBrawlModeChanged(bool locked)
		{
			_dataSource.InvalidateCache();
			TaskRunner.get_Instance().Run(BrawlModeChanged(locked));
		}

		private IEnumerator BrawlModeChanged(bool locked)
		{
			yield return BrawlOverridePreloader.LoadBrawlLanguageStringOverrides(serviceFactory, loadingIconPresenter);
			if (!locked)
			{
				yield return Refresh();
			}
		}

		private IEnumerator Refresh()
		{
			loadingIconPresenter.NotifyLoading("BrawlParameters");
			yield return _dataSource.RefreshDataWithEmumerator(OnRefreshSucceed, OnRefreshFailed);
		}

		private void OnRefreshFailed(ServiceBehaviour failBehaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
			ErrorWindow.ShowErrorWindow(new GenericErrorData("Brawl refresh failed", "Could not retrieve brawl details for " + GetType().Name, Localization.Get("strRetry", true), Localization.Get("strCancel", true), delegate
			{
				TaskRunner.get_Instance().Run(Refresh());
			}, delegate
			{
			}));
		}

		private void OnRefreshSucceed()
		{
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
		}

		internal void OnBackButtonClicked()
		{
			AskClose();
		}

		internal void OnPlayButtonClicked()
		{
			guiInputController.CloseCurrentScreen();
			guiInputController.ShowScreen(GuiScreens.PlayScreen);
			TryQueueForBrawlCommand tryQueueForBrawlCommand = commandFactory.Build<TryQueueForBrawlCommand>();
			tryQueueForBrawlCommand.Execute();
		}

		private void AskClose()
		{
			guiInputController.CloseCurrentScreen();
			guiInputController.ShowScreen(GuiScreens.PlayScreen);
		}
	}
}
