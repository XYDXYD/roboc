using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Utility;

namespace Mothership.GUI.Clan
{
	internal class SearchClansController : ClanSectionControllerBase, IWaitForFrameworkDestruction
	{
		private ClanListDataSource _clanListDataSource;

		private string _searchTextEntryField;

		private ShortCutMode _previousShortcutMode;

		[Inject]
		internal SearchClanLayoutFactory searchClanLayoutFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
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
		internal ClanController clansController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		[Inject]
		internal IMultiAvatarLoader AvatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver AvatarAvailableObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		public override ClanSectionType SectionType => ClanSectionType.SearchClan;

		public override void HandleGenericMessage(GenericComponentMessage receivedMessage)
		{
			switch (receivedMessage.Message)
			{
			case MessageType.Disable:
			case MessageType.Enable:
			case MessageType.Show:
			case MessageType.Hide:
				break;
			case MessageType.ButtonClicked:
				if (receivedMessage.Originator == "loadmore")
				{
					LoadMoreResults();
				}
				if (receivedMessage.Originator == "search")
				{
					ExecuteNewSearch();
				}
				break;
			case MessageType.ButtonWithinListClicked:
				if (receivedMessage.Originator == "clanlist" && receivedMessage.Message == MessageType.ButtonWithinListClicked)
				{
					ClanListItemComponentDataContainer.ClanListItemInfo clanListItemInfo = receivedMessage.Data.UnpackData<ClanListItemComponentDataContainer.ClanListItemInfo>();
					DealWithClickedOnClan(clanListItemInfo.index, clanListItemInfo.nameOfClan);
				}
				break;
			case MessageType.TextEntrySubmitted:
				if (receivedMessage.Originator == "searchtextentry")
				{
					ExecuteNewSearch();
				}
				break;
			case MessageType.TextEntryChanged:
				if (receivedMessage.Originator == "searchtextentry")
				{
					_searchTextEntryField = receivedMessage.Data.UnpackData<string>();
				}
				break;
			case MessageType.OnFocus:
				_previousShortcutMode = inputController.GetShortCutMode();
				inputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
				break;
			case MessageType.OnUnfocus:
				inputController.SetShortCutMode(_previousShortcutMode);
				break;
			}
		}

		public override void HandleClanMessageDerived(SocialMessage message)
		{
			SocialMessageType messageDispatched = message.messageDispatched;
			if (messageDispatched == SocialMessageType.ActivateSearchClansTab)
			{
				loadingIconPresenter.NotifyLoading("Clans");
				_clanListDataSource.RefreshData(OnRefreshSuccess, OnFailed);
			}
		}

		public override void OnSetupController()
		{
			_clanListDataSource = new ClanListDataSource(socialRequestFactory, AvatarLoader, AvatarAvailableObserver, PresetAvatarMapProvider.GetAvatarMap());
		}

		public override void OnViewSet(ClanSectionViewBase view)
		{
			SearchClansView view2 = view as SearchClansView;
			searchClanLayoutFactory.BuildAll(view2, _clanListDataSource);
			loadingIconPresenter.NotifyLoading("Clans");
			_clanListDataSource.RefreshData(OnRefreshSuccess, OnFailed);
		}

		private void DealWithClickedOnClan(int index, string nameOfClan)
		{
			Console.Log("switch to the Your Clan screen, with info about the clan at position " + index + " in the list, clan name was" + nameOfClan);
			SocialMessage message = new SocialMessage(SocialMessageType.ChangeTabTypeAndBringToTop, string.Empty, new ChangeTabTypeData(0, ClanSectionType.YourClan));
			clansController.DispatchAnyClanMessage(message);
			SocialMessage message2 = new SocialMessage(SocialMessageType.ConfigureYourClanData, nameOfClan);
			clansController.DispatchAnyClanMessage(message2);
		}

		private void LoadMoreResults()
		{
			loadingIconPresenter.NotifyLoading("Clans");
			_clanListDataSource.ExpandSearchHistory();
			_clanListDataSource.RefreshData(OnRefreshSuccessFromClickedSearch, OnFailed);
		}

		private void ExecuteNewSearch()
		{
			if (_searchTextEntryField == string.Empty)
			{
				_clanListDataSource.SetSearchTerm(string.Empty, onlyOpenClans: true);
			}
			else
			{
				_clanListDataSource.SetSearchTerm(_searchTextEntryField, onlyOpenClans: false);
			}
			loadingIconPresenter.NotifyLoading("Clans");
			_clanListDataSource.RefreshData(OnRefreshSuccess, OnFailed);
		}

		private void OnRefreshSuccessFromClickedSearch()
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "clanlist", string.Empty));
			DispatchGenericMessage(new GenericComponentMessage(MessageType.SetScroll, "clanlist", string.Empty, new ScrollBarPositionDataContainer(1f)));
		}

		private void OnRefreshSuccess()
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			DispatchGenericMessage(new GenericComponentMessage(MessageType.RefreshData, "clanlist", string.Empty));
		}

		private void OnFailed(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			string @string = StringTableBase<StringTable>.Instance.GetString("strRetry");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, @string, behaviour.alternativeText, delegate
			{
				loadingIconPresenter.NotifyLoading("Clans");
				behaviour.MainBehaviour();
			}, behaviour.Alternative));
		}

		public void OnFrameworkDestroyed()
		{
			_clanListDataSource.Dispose();
		}
	}
}
