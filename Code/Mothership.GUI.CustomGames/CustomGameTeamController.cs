using Authentication;
using CustomGames;
using Mothership.GUI.Party;
using Services.Web;
using Services.Web.Photon;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership.GUI.CustomGames
{
	internal sealed class CustomGameTeamController : IInitialize
	{
		private int _maxTeamSize;

		private CustomGameTeamView _view;

		private CustomGamePartyTeamsDataSource _dataSource;

		private HidePopupMenuMessage _hidePopupMenuMessage;

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
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
		internal GameModeChoiceFilterPresenter gameModeChoiceFilterPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameGameModeObserver customGameModeObserver
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		public int TeamSize => _dataSource.NumberOfDataItemsAvailable(0);

		public string GetMemberNameForSlotIndex(int index)
		{
			return _dataSource.QueryData<string>(index, 0);
		}

		public unsafe void OnDependenciesInjected()
		{
			_hidePopupMenuMessage = new HidePopupMenuMessage();
			_dataSource = new CustomGamePartyTeamsDataSource(serviceFactory);
			customGameModeObserver.AddAction(new ObserverAction<GameModeType>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void RebuildIconList()
		{
			PartyIconView[] existingIconList = _view.GetExistingIconList();
			PartyIconView[] array = existingIconList;
			foreach (PartyIconView partyIconView in array)
			{
				gameObjectPool.Recycle(partyIconView.get_gameObject(), "CustomGamePartyIconPool");
				partyIconView.get_gameObject().SetActive(false);
			}
			existingIconList = BuildList(_maxTeamSize);
			_view.AnchorIconList(existingIconList);
			_view.BuildSignalAndBubble();
			int num = 0;
			PartyIconView[] array2 = existingIconList;
			foreach (PartyIconView partyIconView2 in array2)
			{
				partyIconView2.Activate(num);
				CustomGameReceiveDropBehaviour component = partyIconView2.GetComponent<CustomGameReceiveDropBehaviour>();
				component.RebuildBubbleSignal();
				CustomGameProvideDragObjectBehaviour component2 = partyIconView2.GetComponent<CustomGameProvideDragObjectBehaviour>();
				component2.Configure(num, _view.TeamRepresentation == CustomGameTeamChoice.TeamA, partyIconView2, _view.DragIconToDisplay);
				num++;
			}
		}

		private PartyIconView[] BuildList(int size)
		{
			PartyIconView[] array = new PartyIconView[size];
			for (int i = 0; i < size; i++)
			{
				GameObject val = gameObjectPool.Use("CustomGamePartyIconPool", (Func<GameObject>)CreateCustomGamePartyIcon);
				val.SetActive(true);
				val.set_name("PartyIconView_" + i);
				array[i] = val.GetComponent<PartyIconView>();
			}
			return array;
		}

		private GameObject CreateCustomGamePartyIcon()
		{
			return gameObjectFactory.Build("CustomGamePartyButton");
		}

		public void SetView(CustomGameTeamView view)
		{
			_view = view;
			_dataSource.SetTeam(view.TeamRepresentation);
		}

		private void OnCustomGameTeamsChanged()
		{
			Console.Log("Team refresh needed");
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshTeamsChanged);
		}

		private void OnCustomGameGameModeChanged(ref GameModeType gameMode)
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshTeamsChanged);
		}

		private IEnumerator RefreshTeamsChanged()
		{
			int previousTeamSize = _maxTeamSize;
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return refreshTask;
			if (refreshTask.succeeded && refreshTask.result.Data != null)
			{
				GameModeType gameMode = (GameModeType)Enum.Parse(value: refreshTask.result.Data.Config["GameMode"], enumType: typeof(GameModeType));
				if (gameMode == GameModeType.Pit && _view.TeamRepresentation == CustomGameTeamChoice.TeamB)
				{
					_maxTeamSize = 0;
				}
				else
				{
					loadingIconPresenter.NotifyLoading("CustomGameTeamController");
					ICustomGameGetTeamSetupRequest request = serviceFactory.Create<ICustomGameGetTeamSetupRequest>();
					request.Inject(gameMode);
					TaskService<int> taskService = new TaskService<int>(request);
					yield return new HandleTaskServiceWithError(taskService, delegate
					{
						loadingIconPresenter.NotifyLoading("CustomGameTeamController");
					}, delegate
					{
						loadingIconPresenter.NotifyLoadingDone("CustomGameTeamController");
					}).GetEnumerator();
					loadingIconPresenter.NotifyLoadingDone("CustomGameTeamController");
					if (taskService.succeeded && taskService.result != _maxTeamSize)
					{
						_maxTeamSize = taskService.result;
					}
				}
				if (_maxTeamSize != previousTeamSize)
				{
					RebuildIconList();
				}
				TaskRunner.get_Instance().Run(HandleTeamLeaderChangedOrNeedsRefreshing());
				loadingIconPresenter.NotifyLoading("CustomGameTeamController");
				_dataSource.InvalidateCache();
				yield return _dataSource.RefreshDataWithEmumerator(OnRefreshSucceed, OnRefreshFailed);
			}
			else
			{
				Console.LogWarning("could not refresh Teams as the retrieve request failed");
			}
		}

		private void OnRefreshFailed(ServiceBehaviour failBehaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("CustomGameTeamController");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCustomGameRefreshFailed"), StringTableBase<StringTable>.Instance.GetString("strCustomGameRefreshFailedForTeam"), Localization.Get("strRetry", true), Localization.Get("strCancel", true), delegate
			{
				TaskRunner.get_Instance().Run(RefreshTeamsChanged());
			}, delegate
			{
			}));
		}

		private void OnRefreshSucceed()
		{
			loadingIconPresenter.NotifyLoadingDone("CustomGameTeamController");
			int num = _dataSource.NumberOfDataItemsAvailable(0);
			_view.BuildSignalAndBubble();
			string sessionLeader = _dataSource.GetSessionLeader();
			for (int i = 0; i < num; i++)
			{
				string name = _dataSource.QueryData<string>(i, 0);
				string memberDisplayName = _dataSource.GetMemberDisplayName(i);
				bool leader = _dataSource.QueryData<bool>(i, 0);
				AvatarInfo avatar = _dataSource.QueryData<AvatarInfo>(i, 0);
				PlatoonMember.MemberStatus status = _dataSource.QueryData<PlatoonMember.MemberStatus>(i, 0);
				PartyMemberDataChangedMessage message = new PartyMemberDataChangedMessage(i, name, memberDisplayName, status, leader, avatar, sessionLeader);
				_view.DeepBroadcast(message);
			}
			for (int j = num; j < _maxTeamSize; j++)
			{
				PartyMemberDataChangedMessage message2 = new PartyMemberDataChangedMessage(j, string.Empty, string.Empty, PlatoonMember.MemberStatus.Ready, leader: false, null, sessionLeader);
				_view.DeepBroadcast(message2);
			}
			bool largePlayerAvatar = false;
			if (num > 0)
			{
				string text = _dataSource.QueryData<string>(0, 0);
				if (text.CompareTo(User.Username) == 0)
				{
					largePlayerAvatar = true;
				}
			}
			if (_maxTeamSize > 0)
			{
				_view.SetLargePlayerAvatar(largePlayerAvatar);
			}
		}

		public void HandleMessage(object message)
		{
			if (message is CustomGameTeamAssignmentsChangedMessage)
			{
				OnCustomGameTeamsChanged();
			}
			else if (message is CustomGameTierChangedMessage)
			{
				CustomGameTierChangedMessage customGameTierChangedMessage = message as CustomGameTierChangedMessage;
				_view.Broadcast(new PartyIconTierChangeMessage(customGameTierChangedMessage.TargetUser, customGameTierChangedMessage.Tier, customGameTierChangedMessage.TierString));
			}
			else if (message is TeamLeaderChangedMessage)
			{
				TaskRunner.get_Instance().Run(HandleTeamLeaderChangedOrNeedsRefreshing());
			}
			if (!(message is PartyMemberButtonClickMessage))
			{
				return;
			}
			PartyMemberButtonClickMessage partyMemberButtonClickMessage = (PartyMemberButtonClickMessage)message;
			if (partyMemberButtonClickMessage.IconStateWhenClicked == PartyIconState.AddMemberState)
			{
				_view.Broadcast(_hidePopupMenuMessage);
				_view.ShowPartyInviteForCustomGame(partyMemberButtonClickMessage.UIElement);
			}
			else if (partyMemberButtonClickMessage.IconStateWhenClicked != 0)
			{
				if (partyMemberButtonClickMessage.PlayerAssignedToSlot.CompareTo(User.Username) != 0)
				{
					_view.Broadcast(_hidePopupMenuMessage);
					ShowContextMenuForOther(partyMemberButtonClickMessage.UIElement, partyMemberButtonClickMessage.PlayerAssignedToSlot, partyMemberButtonClickMessage.IconStateWhenClicked);
				}
				else
				{
					_view.Broadcast(_hidePopupMenuMessage);
					_view.ShowContextMenuForCustomGame(partyMemberButtonClickMessage.UIElement, partyMemberButtonClickMessage.PlayerAssignedToSlot, PartyPopupMenuItems.LeaveParty);
				}
			}
		}

		private void ShowContextMenuForOther(UIWidget uiElement, string partyMemberName, PartyIconState iconState)
		{
			if (iconState == PartyIconState.MemberPendingAccept)
			{
				_view.ShowContextMenuForCustomGame(uiElement, partyMemberName, PartyPopupMenuItems.CancelPendingInvitation);
			}
			else
			{
				_view.ShowContextMenuForCustomGame(uiElement, partyMemberName, PartyPopupMenuItems.RemoveFromParty);
			}
		}

		private IEnumerator HandleTeamLeaderChangedOrNeedsRefreshing()
		{
			bool isLocalPlayerLeader = false;
			IRetrieveCustomGameSessionRequest refreshSessionRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> refreshTask = new TaskService<RetrieveCustomGameSessionRequestData>(refreshSessionRequest);
			yield return refreshTask;
			if (refreshTask.succeeded && refreshTask.result.Data != null)
			{
				string sessionLeader = refreshTask.result.Data.SessionLeader;
				if (sessionLeader == User.Username)
				{
					isLocalPlayerLeader = true;
				}
			}
			_view.BuildSignalAndBubble();
			_view.Broadcast(new PartyIconCannotInteractMessage(!isLocalPlayerLeader));
		}
	}
}
