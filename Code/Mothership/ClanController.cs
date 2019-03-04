using Mothership.GUI.Social;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class ClanController : IInitialize, IFloatingWidget
	{
		public const int MAX_CLAN_TABS = 2;

		private static bool _clanAnalyticEventSent;

		public Action<IAnchorUISource> OnAnchorAllChildControllers = delegate
		{
		};

		private BubbleSignal<ISocialRoot> _bubble;

		private bool _singleTabShownOnTop;

		private ClanSectionType _singleTabShownType;

		private bool _isInClan;

		private bool _hasPendingInvites;

		private int _currentTab;

		private ClanSectionType[] _controllerTabType = new ClanSectionType[2];

		private ClanScreen _mainView;

		private bool _canCloseScreen = true;

		private IServiceEventContainer _socialEventContainer;

		private bool _getMyClanSucceeded;

		private bool _clanInvitesSucceeded;

		[Inject]
		public LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public ISocialEventContainerFactory socialEventContainerFactory
		{
			private get;
			set;
		}

		[Inject]
		public AvatarSelectionPresenter avatarSelectionPresenter
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			_controllerTabType[0] = ClanSectionType.CreateClan;
			_controllerTabType[1] = ClanSectionType.SearchClan;
			_currentTab = 0;
			_singleTabShownOnTop = false;
			_singleTabShownType = ClanSectionType.YourClan;
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ListenTo<IClanInviteReceivedEventListener, ClanInvite, ClanInvite[]>(HandleOnClanInviteReceivedEvent);
			_socialEventContainer.ListenTo<IClanInviteCancelledEventListener, ClanInvite[]>(HandleOnClanInviteCancelledEvent);
		}

		public void SetMainView(ClanScreen view)
		{
			_mainView = view;
			_bubble = new BubbleSignal<ISocialRoot>(_mainView.get_transform());
			UpdateTabLabels();
		}

		public bool IsActive()
		{
			return _mainView.IsActive();
		}

		public void HandleClanMessage(SocialMessage message)
		{
			switch (message.messageDispatched)
			{
			case SocialMessageType.ClanLeft:
				_isInClan = false;
				BubbleUpSocialMessage(message);
				break;
			case SocialMessageType.ClanJoined:
				_isInClan = true;
				BubbleUpSocialMessage(message);
				break;
			case SocialMessageType.ClanCreated:
				_isInClan = true;
				BubbleUpSocialMessage(message);
				break;
			case SocialMessageType.AllInvitationsDealtWith:
				_hasPendingInvites = false;
				break;
			case SocialMessageType.SingleTabOnTopBackClicked:
				HideSingleTabAndDisplayMultitabScreen();
				DispatchTabActivationMessage(_controllerTabType[_currentTab], new AdditionalClanSectionActivationInfo(shouldPushCurrentState_: false, shouldRestorePreviousState_: true));
				break;
			case SocialMessageType.ChangeTabTypeAndBringToTop:
			{
				ChangeTabTypeData changeTabTypeData2 = message.extraData as ChangeTabTypeData;
				DispatchTabActivationMessage(changeTabTypeData2.typeToChangeTo, new AdditionalClanSectionActivationInfo(shouldPushCurrentState_: true, shouldRestorePreviousState_: false));
				ShowSingleTabOnTop(changeTabTypeData2.typeToChangeTo, tabHeaderBarVisible: true);
				break;
			}
			case SocialMessageType.ChangeTabTypeAndSelect:
			{
				ChangeTabTypeData changeTabTypeData = message.extraData as ChangeTabTypeData;
				SwitchTabType(changeTabTypeData.tabIndex, changeTabTypeData.typeToChangeTo);
				SelectTab(changeTabTypeData.tabIndex);
				break;
			}
			case SocialMessageType.MaximizeClanScreen:
				MaximizeClanScreen();
				break;
			case SocialMessageType.MaximizeFriendScreen:
				Close();
				break;
			case SocialMessageType.SelectTab1:
				SelectTab(0);
				break;
			case SocialMessageType.SelectTab2:
				SelectTab(1);
				break;
			case SocialMessageType.CloseSocialScreens:
				if (message.extraDetails == "ChatRoot")
				{
					Close();
				}
				break;
			case SocialMessageType.ClickedOutsideSocial:
				if (_canCloseScreen)
				{
					Close();
				}
				break;
			case SocialMessageType.NewClanAvatarSelected:
				_canCloseScreen = false;
				TaskRunner.get_Instance().Run((Func<IEnumerator>)ReEnablingCloseOnOutsideClick);
				break;
			case SocialMessageType.SingleTabOnTopCloseClicked:
				HideSingleTabAndDisplayMultitabScreen();
				MaximizeClanScreen();
				break;
			case SocialMessageType.SocialViewDisabled:
				if (_mainView.IsActive())
				{
					Close();
				}
				break;
			}
		}

		public void DispatchAnyClanMessage(SocialMessage message)
		{
			if (message != null)
			{
				_mainView.SendClanMessage(message);
			}
		}

		public void DispatchSignalChainMessage<M>(M message)
		{
			_bubble.Dispatch<M>(message);
		}

		public void BubbleUpSocialMessage(SocialMessage message)
		{
			_mainView.BubbleUpSocialMessage(message);
		}

		public void DispatchTabActivationMessage(ClanSectionType sectionType, AdditionalClanSectionActivationInfo additionalInfo = null)
		{
			SocialMessage socialMessage = null;
			switch (sectionType)
			{
			case ClanSectionType.CreateClan:
				socialMessage = new SocialMessage(SocialMessageType.ActivateCreateClanTab, string.Empty, additionalInfo);
				break;
			case ClanSectionType.SearchClan:
				socialMessage = new SocialMessage(SocialMessageType.ActivateSearchClansTab, string.Empty, additionalInfo);
				break;
			case ClanSectionType.YourClan:
				socialMessage = new SocialMessage(SocialMessageType.ActivateYourClanTab, string.Empty, additionalInfo);
				break;
			case ClanSectionType.ClanInvites:
				socialMessage = new SocialMessage(SocialMessageType.ActivateClanInvitesTab, string.Empty, additionalInfo);
				break;
			}
			if (socialMessage != null)
			{
				_mainView.SendClanMessage(socialMessage);
			}
		}

		public IEnumerator LoadData()
		{
			_getMyClanSucceeded = (_clanInvitesSucceeded = false);
			loadingIconPresenter.NotifyLoading("LoadClanData");
			yield return (object)new ParallelTaskCollection(new IEnumerator[2]
			{
				GetMyClanInfo(),
				GetMyClanInvites()
			});
			while (_getMyClanSucceeded && !_clanInvitesSucceeded)
			{
				Console.LogWarning("waiting for clan data to be loaded");
				yield return null;
			}
			Console.Log("clan data loaded");
			loadingIconPresenter.NotifyLoadingDone("LoadClanData");
		}

		public IEnumerator GetMyClanInfo()
		{
			TaskService<ClanInfo> requestTask = socialRequestFactory.Create<IGetMyClanInfoRequest>().AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				HandleOnClanInfoRetrieved(requestTask.result);
			}
			else
			{
				HandleLoadRequestFailed(requestTask.behaviour);
			}
		}

		public IEnumerator GetMyClanInvites()
		{
			TaskService<ClanInvite[]> requestTask = socialRequestFactory.Create<IGetClanInvitesRequest>().AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				HandleOnClanInvitesRetrieved(requestTask.result);
			}
			else
			{
				HandleLoadRequestFailed(requestTask.behaviour);
			}
		}

		private void HandleOnClanInviteCancelledEvent(ClanInvite[] allInvites)
		{
			if (allInvites.GetLength(0) == 0)
			{
				_hasPendingInvites = false;
			}
			else
			{
				_hasPendingInvites = true;
			}
		}

		private void HandleOnClanInviteReceivedEvent(ClanInvite invite, ClanInvite[] allInvites)
		{
			if (allInvites.GetLength(0) == 0)
			{
				_hasPendingInvites = false;
			}
			else
			{
				_hasPendingInvites = true;
			}
		}

		private void HideSingleTabAndDisplayMultitabScreen()
		{
			_singleTabShownOnTop = false;
			_mainView.SetSingleTabHeaderVisible(tabHeaderVisible: false);
			_mainView.SetTabsVisibile(status: true);
		}

		private void MaximizeClanScreen()
		{
			OnAnchorAllChildControllers(_mainView);
			if (!_mainView.IsActive())
			{
				if (!_singleTabShownOnTop)
				{
					ConfigureFirstTabAsCreateOrYourClan();
				}
				if (_hasPendingInvites)
				{
					DispatchTabActivationMessage(ClanSectionType.ClanInvites, new AdditionalClanSectionActivationInfo(shouldPushCurrentState_: true, shouldRestorePreviousState_: false));
					ShowSingleTabOnTop(ClanSectionType.ClanInvites, tabHeaderBarVisible: false);
				}
				else if (_singleTabShownOnTop)
				{
					DispatchTabActivationMessage(_singleTabShownType, new AdditionalClanSectionActivationInfo(shouldPushCurrentState_: false, shouldRestorePreviousState_: false));
					ShowSingleTabOnTop(_singleTabShownType, (_singleTabShownType != ClanSectionType.ClanInvites) ? true : false);
				}
				else
				{
					DispatchTabActivationMessage(_controllerTabType[_currentTab]);
				}
				_mainView.ShowMainPanel();
				guiInputController.AddFloatingWidget(this);
			}
			else
			{
				Close();
			}
		}

		private IEnumerator ReEnablingCloseOnOutsideClick()
		{
			yield return (object)new WaitForEndOfFrame();
			_canCloseScreen = true;
		}

		private void HandleOnClanInvitesRetrieved(ClanInvite[] invites)
		{
			_clanInvitesSucceeded = true;
			_hasPendingInvites = (invites.GetLength(0) != 0);
		}

		private void HandleOnClanInfoRetrieved(ClanInfo data)
		{
			_getMyClanSucceeded = true;
			if (data == null)
			{
				_isInClan = false;
			}
			else
			{
				_isInClan = true;
			}
			if (!_clanAnalyticEventSent)
			{
				_clanAnalyticEventSent = true;
			}
		}

		private void ConfigureFirstTabAsCreateOrYourClan()
		{
			if (!_isInClan)
			{
				SwitchTabType(0, ClanSectionType.CreateClan);
			}
			else
			{
				DispatchAnyClanMessage(new SocialMessage(SocialMessageType.ConfigureYourClanData, string.Empty));
				SwitchTabType(0, ClanSectionType.YourClan);
			}
			UpdateTabLabels();
		}

		private void HandleLoadRequestFailed(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("Clans");
			string okText = "strRetry";
			ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, okText, behaviour.alternativeText, delegate
			{
				loadingIconPresenter.NotifyLoading("Clans");
				behaviour.MainBehaviour();
			}, behaviour.Alternative));
		}

		private void Close()
		{
			if (_mainView.IsActive())
			{
				SocialMessage message = new SocialMessage(SocialMessageType.ClanScreenClosed, string.Empty);
				_mainView.SendClanMessage(message);
				_mainView.BubbleUpSocialMessage(message);
				guiInputController.RemoveFloatingWidget(this);
				_mainView.HideMainPanel();
				avatarSelectionPresenter.Hide();
			}
		}

		private void ShowSingleTabOnTop(ClanSectionType typeToChangeTo, bool tabHeaderBarVisible)
		{
			_singleTabShownOnTop = true;
			_singleTabShownType = typeToChangeTo;
			_mainView.SetSingleTabHeaderVisible(tabHeaderBarVisible);
			_mainView.SetTabsVisibile(status: false);
		}

		private void SwitchTabType(int tabIndex, ClanSectionType clanTabType)
		{
			_controllerTabType[tabIndex] = clanTabType;
			UpdateTabLabels();
			if (_currentTab == tabIndex)
			{
				DispatchTabActivationMessage(_controllerTabType[tabIndex]);
			}
		}

		private void SelectTab(int tabIndex)
		{
			_mainView.SetSingleTabActive(tabIndex);
			_currentTab = tabIndex;
			DispatchTabActivationMessage(_controllerTabType[tabIndex]);
		}

		private void UpdateTabLabels()
		{
			for (int i = 0; i < 2; i++)
			{
				switch (_controllerTabType[i])
				{
				case ClanSectionType.CreateClan:
					_mainView.SetTabLabel(i, StringTableBase<StringTable>.Instance.GetString("strClanControllerTabCreate"));
					break;
				case ClanSectionType.SearchClan:
					_mainView.SetTabLabel(i, StringTableBase<StringTable>.Instance.GetString("strClanControllerTabSearch"));
					break;
				case ClanSectionType.YourClan:
					_mainView.SetTabLabel(i, StringTableBase<StringTable>.Instance.GetString("strClanControllerTabYourClan"));
					break;
				case ClanSectionType.ClanInvites:
					_mainView.SetTabLabel(i, StringTableBase<StringTable>.Instance.GetString("strClanControllerTabInvitations"));
					break;
				}
			}
		}

		public void OnScreenSizeChange(int newHeight, int oldHeight)
		{
			if (newHeight > _mainView.screenSizeSwitchHeight && oldHeight <= _mainView.screenSizeSwitchHeight)
			{
				_mainView.Minimize();
			}
			else if (newHeight <= _mainView.screenSizeSwitchHeight && oldHeight > _mainView.screenSizeSwitchHeight)
			{
				_mainView.Maximize();
			}
		}

		public void HandleQuitPressed()
		{
			Close();
		}
	}
}
