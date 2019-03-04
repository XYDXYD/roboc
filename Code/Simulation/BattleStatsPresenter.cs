using Authentication;
using LobbyServiceLayer;
using Simulation.Hardware.Weapons;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class BattleStatsPresenter : IGUIDisplay, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IBattleStatsPresenterComponent, IInitialize, IComponent
	{
		private enum UIStyle
		{
			Unavailable,
			Pit,
			TDM,
			BattleArena,
			AI
		}

		private delegate int CompareFunction(BattleStatsData p1, BattleStatsData p2, bool ascending, InGameStatId statId);

		private BattleStatsPlayerLayout _battleStatsView;

		private IList<BattleStatsData> _playersList = new List<BattleStatsData>();

		private InGameStatId _currentStatsId;

		private IList<BattleStatsSortButtonView> _sortButtons = new List<BattleStatsSortButtonView>();

		private bool _sortAscending;

		private HashSet<string> _friendList = new HashSet<string>();

		private HashSet<string> _sentList = new HashSet<string>();

		private HashSet<string> _pendingList = new HashSet<string>();

		private bool _friendFunctionsEnabled;

		private bool _disableGuiInputControllerHide;

		private Dictionary<int, uint> _playerTotalObjectiveScore = new Dictionary<int, uint>();

		private readonly StringBuilder _stringBuilder = new StringBuilder();

		private IServiceEventContainer _socialEventContainer;

		private ReadOnlyDictionary<string, ClanInfo> _clanInfos;

		private HudStyle _battleHudStyle;

		private UIStyle _uiStyle;

		[Inject]
		public IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		public GameStateClient gameStateClient
		{
			private get;
			set;
		}

		[Inject]
		public ICursorMode cursorMode
		{
			get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public IGameObjectFactory gameObjectFactory
		{
			get;
			set;
		}

		[Inject]
		public RegisterPlayerObserver registerPlayerObserver
		{
			get;
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
		public ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		public Texture2D AvatarAtlasTexture
		{
			get;
			private set;
		}

		public IDictionary<string, Rect> AvatarAtlasRects
		{
			get;
			private set;
		}

		public Texture2D ClanAvatarAtlasTexture
		{
			get;
			private set;
		}

		public IDictionary<string, Rect> ClanAvatarAtlasRects
		{
			get;
			private set;
		}

		public GuiScreens screenType => GuiScreens.BattleStatsScreen;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public bool isScreenBlurred => gameStateClient.hasGameEnded;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => _battleHudStyle;

		public GUIShowResult Show()
		{
			_battleStatsView.Show();
			_battleStatsView.SetOKButtonEnabled(gameStateClient.hasGameEnded);
			UpdateVisuals();
			if (gameStateClient.hasGameEnded)
			{
				_battleHudStyle = HudStyle.HideAllButChat;
				_battleStatsView.GameEnded();
			}
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			if (_disableGuiInputControllerHide)
			{
				return false;
			}
			if (!_battleStatsView.IsVisible())
			{
				return false;
			}
			_battleStatsView.Hide();
			return true;
		}

		public bool IsActive()
		{
			if (_battleStatsView == null)
			{
				return false;
			}
			return _battleStatsView.get_gameObject().get_activeSelf();
		}

		public void EnableBackground(bool enable)
		{
		}

		public void SetView(BattleStatsPlayerLayout view)
		{
			_battleStatsView = view;
			_battleStatsView.Hide();
			switch (_uiStyle)
			{
			case UIStyle.Unavailable:
				Console.Log("ERROR:battle stats view should not be present in this game-mode");
				break;
			case UIStyle.BattleArena:
				_battleStatsView.SetBattleArenaStyle();
				break;
			case UIStyle.Pit:
				_battleStatsView.SetPitHeaderStyle();
				break;
			case UIStyle.TDM:
			case UIStyle.AI:
				_battleStatsView.SetTDMHeaderStyle();
				break;
			}
			RegisterEventListeners();
			GetPlayerFriendsList();
		}

		void IInitialize.OnDependenciesInjected()
		{
			registerPlayerObserver.OnRegisterPlayer += RegisterPlayer;
		}

		public void AddSortButton(BattleStatsSortButtonView buttonView)
		{
			buttonView.ResetSorting();
			_sortButtons.Add(buttonView);
		}

		public void ResetSortButtons(BattleStatsSortButtonView fromButton)
		{
			foreach (BattleStatsSortButtonView sortButton in _sortButtons)
			{
				if (sortButton != fromButton)
				{
					sortButton.ResetSorting();
				}
			}
		}

		public void ChangeStateToGameEnded()
		{
			_disableGuiInputControllerHide = true;
		}

		public void ShowBattleStats()
		{
			guiInputController.ShowScreen(GuiScreens.BattleStatsScreen);
		}

		public void HideBattleStats()
		{
			guiInputController.CloseCurrentScreen();
		}

		public BattleStatsData GetPlayerScore(string playerName)
		{
			for (int i = 0; i < _playersList.Count; i++)
			{
				BattleStatsData battleStatsData = _playersList[i];
				if (battleStatsData.playerName == playerName)
				{
					return battleStatsData;
				}
			}
			return null;
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			BuildDependencies();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			registerPlayerObserver.OnRegisterPlayer -= RegisterPlayer;
			_playersList.Clear();
		}

		protected virtual void BuildDependencies()
		{
			gameObjectFactory.Build("Battle_Stats");
		}

		private void RegisterEventListeners()
		{
			_socialEventContainer = socialEventContainerFactory.Create();
			_socialEventContainer.ListenTo<IFriendInviteEventListener, FriendListUpdate>(OnFriendEvent);
			_socialEventContainer.ListenTo<IFriendAcceptEventListener, FriendListUpdate>(OnFriendEvent);
		}

		private void OnFriendEvent(FriendListUpdate update)
		{
			for (int i = 0; i < update.friendList.Count; i++)
			{
				if (update.friendList[i].Name == update.user)
				{
					switch (update.friendList[i].InviteStatus)
					{
					case FriendInviteStatus.Accepted:
						FriendAccepted(update.user);
						break;
					case FriendInviteStatus.InvitePending:
						FriendRequestReceived(update.user);
						break;
					}
					return;
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("sender", update.user);
			Dictionary<string, string> data = dictionary;
			RemoteLogger.Error("Friend event received by unrecognised friend", User.Username, null, data);
		}

		protected void GetPlayerFriendsList()
		{
			if (_uiStyle == UIStyle.AI)
			{
				GetPlayerList();
				return;
			}
			IServiceRequest serviceRequest = socialRequestFactory.Create<IGetFriendListRequest>().SetAnswer(new ServiceAnswer<GetFriendListResponse>(delegate(GetFriendListResponse friendListResponse)
			{
				OnGotFriendList(friendListResponse.friendsList);
			}, OnGotFriendListFailed));
			serviceRequest.Execute();
		}

		private void OnGotFriendList(IList<Friend> friendList)
		{
			foreach (Friend friend in friendList)
			{
				switch (friend.InviteStatus)
				{
				case FriendInviteStatus.Accepted:
					_friendList.Add(friend.Name);
					break;
				case FriendInviteStatus.InvitePending:
					_pendingList.Add(friend.Name);
					break;
				case FriendInviteStatus.InviteSent:
					_sentList.Add(friend.Name);
					break;
				}
			}
			_friendFunctionsEnabled = true;
			GetPlayerList();
		}

		private void OnGotFriendListFailed(ServiceBehaviour behaviour)
		{
			RemoteLogger.Error(behaviour.errorTitle, behaviour.errorBody, null);
			GetPlayerList();
		}

		protected void GetPlayerList()
		{
			lobbyRequestFactory.Create<IGetClanInfosRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, ClanInfo>>(OnClanInfosLoaded)).Execute();
			IRetrieveExpectedPlayersListRequest retrieveExpectedPlayersListRequest = lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>();
			retrieveExpectedPlayersListRequest.SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(delegate(ReadOnlyDictionary<string, PlayerDataDependency> playerDependencies)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				DictionaryEnumerator<string, PlayerDataDependency> enumerator = playerDependencies.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, PlayerDataDependency> current = enumerator.get_Current();
						AddPlayer(current.Value.PlayerName, current.Value.DisplayName, current.Value.TeamId, _friendList.Contains(current.Value.PlayerName), _sentList.Contains(current.Value.PlayerName));
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				_battleStatsView.SetOKButtonEnabled(enabled: false);
			}));
			retrieveExpectedPlayersListRequest.Execute();
		}

		private void OnClanInfosLoaded(ReadOnlyDictionary<string, ClanInfo> clanInfos)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_clanInfos = clanInfos;
		}

		private void AddPlayer(string playerName, string displayName, int playerTeam, bool isFriend, bool requestSent)
		{
			BattleStatsData battleStatsData = new BattleStatsData();
			battleStatsData.stats.Add(InGameStatId.Kill, 0u);
			battleStatsData.stats.Add(InGameStatId.RobotDestroyed, 0u);
			battleStatsData.stats.Add(InGameStatId.KillAssist, 0u);
			battleStatsData.stats.Add(InGameStatId.DestroyedCubes, 0u);
			battleStatsData.stats.Add(InGameStatId.HealCubes, 0u);
			battleStatsData.stats.Add(InGameStatId.Score, 0u);
			battleStatsData.stats.Add(InGameStatId.Points, 0u);
			battleStatsData.stats.Add(InGameStatId.CurrentKillStreak, 0u);
			battleStatsData.stats.Add(InGameStatId.BestKillStreak, 0u);
			battleStatsData.stats.Add(InGameStatId.BattleArenaObjectives, 0u);
			battleStatsData.playerName = playerName;
			battleStatsData.displayName = displayName;
			battleStatsData.teamId = playerTeam;
			battleStatsData.isFriend = isFriend;
			battleStatsData.requestSent = requestSent;
			_playersList.Add(battleStatsData);
			_battleStatsView.AddWidget();
			UpdatePlayerWidget(_playersList.Count - 1, init: true);
		}

		public void RegisterPlayer(string name, int id, bool isMe, bool isMyTeam)
		{
			string displayName = playerNamesContainer.GetDisplayName(id);
			RegisterPlayerData registerPlayerData = new RegisterPlayerData(id, name, displayName, isMe, isMyTeam);
			RegisterPlayer(ref registerPlayerData);
		}

		public void RegisterPlayer(ref RegisterPlayerData registerPlayerData)
		{
			int playerIndexByName = GetPlayerIndexByName(registerPlayerData.playerName);
			if (playerIndexByName != -1)
			{
				_playersList[playerIndexByName].playerId = registerPlayerData.playerId;
			}
			if (playerTeamsContainer.IsMe(TargetType.Player, registerPlayerData.playerId))
			{
				UpdateTeamColours(registerPlayerData.playerId);
				SortPlayerListByTeam(ascending: false);
			}
		}

		public virtual void TryUpdateStatValue(int playerId, InGameStatId gameStatId, uint amount, uint deltaScore, bool isFinal = false)
		{
			if (gameStatId == InGameStatId.DestroyedProtoniumCubes || gameStatId == InGameStatId.EqualiserDestroyedBattleArenaMode || gameStatId == InGameStatId.CapturePointBattleArenaMode)
			{
				gameStatId = InGameStatId.BattleArenaObjectives;
				if (!_playerTotalObjectiveScore.ContainsKey(playerId))
				{
					_playerTotalObjectiveScore.Add(playerId, 0u);
				}
				Dictionary<int, uint> playerTotalObjectiveScore;
				int key;
				(playerTotalObjectiveScore = _playerTotalObjectiveScore)[key = playerId] = playerTotalObjectiveScore[key] + deltaScore;
				uint num = _playerTotalObjectiveScore[playerId];
				ActuallyUpdateStatValue(playerId, gameStatId, num, num, isFinal);
			}
			else
			{
				ActuallyUpdateStatValue(playerId, gameStatId, amount, deltaScore, isFinal);
			}
		}

		private void ActuallyUpdateStatValue(int playerId, InGameStatId gameStatId, uint amount, uint deltaScore, bool isFinal = false)
		{
			BattleStatsData playerById = GetPlayerById(playerId);
			if (playerById != null)
			{
				bool value = false;
				if (!playerById.isFinal.TryGetValue(gameStatId, out value) || !value)
				{
					playerById.stats[gameStatId] = amount;
					playerById.isFinal[gameStatId] = isFinal;
				}
			}
			if (_battleStatsView.IsVisible())
			{
				UpdateVisuals();
			}
		}

		private void UpdateVisuals()
		{
			if (_currentStatsId == InGameStatId.None)
			{
				SortPlayerListByTeam(_sortAscending);
			}
			else
			{
				SortPlayerListByStat(_currentStatsId, _sortAscending);
			}
		}

		public void UpdatePlayerWidgets()
		{
			for (int i = 0; i < _playersList.Count; i++)
			{
				UpdatePlayerWidget(i);
			}
		}

		public void UpdateTeamColours(int localPlayerId)
		{
			for (int i = 0; i < _playersList.Count; i++)
			{
				_battleStatsView.SetTeamColour(i, _playersList[i].playerId == localPlayerId, playerTeamsContainer.IsMyTeam(_playersList[i].teamId));
			}
		}

		public bool CheckIsMyTeam(int teamId)
		{
			return playerTeamsContainer.IsMyTeam(teamId);
		}

		private void UpdatePlayerWidget(int index, bool init = false)
		{
			bool isMyTeam = !init && playerTeamsContainer.IsMyTeam(_playersList[index].teamId);
			string playerName = _playersList[index].playerName;
			string displayName = _playersList[index].displayName;
			string clanTag = string.Empty;
			ClanInfo clanInfo = default(ClanInfo);
			if (_clanInfos.TryGetValue(playerName, ref clanInfo))
			{
				clanTag = string.Format("{0}{1}{2}", "[", clanInfo.ClanName, "]");
			}
			_battleStatsView.UpdatePlayerWidget(index, playerTeamsContainer.IsMe(TargetType.Player, _playersList[index].playerId), isMyTeam, playerName, displayName, _playersList[index].isFriend, _playersList[index].requestSent, _friendFunctionsEnabled, clanTag, WorldSwitching.GetGameModeType() == GameModeType.PraticeMode);
			foreach (KeyValuePair<InGameStatId, uint> stat in _playersList[index].stats)
			{
				_battleStatsView.SetStatsLabel(index, stat.Key, stat.Value);
			}
		}

		public void OnFriendButtonClicked(string name)
		{
			TaskRunnerExtensions.Run(OnFriendButtonClickedTask(name));
		}

		private IEnumerator OnFriendButtonClickedTask(string name)
		{
			TaskService<ReadOnlyDictionary<string, PlayerDataDependency>> expectedPlayersTask = lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().AsTask();
			yield return expectedPlayersTask;
			if (expectedPlayersTask.succeeded && expectedPlayersTask.result.ContainsKey(name) && expectedPlayersTask.result.get_Item(name).AiPlayer)
			{
				string @string = StringTableBase<StringTable>.Instance.GetString("strRobocloudError");
				string string2 = StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_AUTO_DECLINED_FRIEND_OR_CLAN.ToString());
				_stringBuilder.Length = 0;
				_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strInviteFriendError"));
				_stringBuilder.Append("\r\n");
				_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", string2));
				OnFriendsError(new GenericErrorData(@string, _stringBuilder.ToString()));
			}
			else if (_pendingList.Contains(name))
			{
				IServiceRequest serviceRequest = socialRequestFactory.Create<IAcceptFriendRequest, string>(name).SetAnswer(new ServiceAnswer<IList<Friend>>(OnFriendAccepted, OnFriendsError));
				serviceRequest.Execute();
				FriendAccepted(name);
			}
			else
			{
				IServiceRequest serviceRequest2 = socialRequestFactory.Create<IInviteFriendRequest, string>(name).SetAnswer(new ServiceAnswer<IList<Friend>>(OnFriendInvited, OnFriendsError));
				serviceRequest2.Execute();
				FriendRequested(name);
			}
		}

		private void OnFriendAccepted(IList<Friend> friend)
		{
			Console.Log("Friend request accepted");
		}

		private void OnFriendInvited(IList<Friend> friend)
		{
			Console.Log("Friend request invited");
		}

		private void OnFriendsError(ServiceBehaviour serviceBehaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
		}

		private void OnFriendsError(GenericErrorData genericErrorData)
		{
			ErrorWindow.ShowErrorWindow(genericErrorData);
		}

		public void FriendRequested(string playerName)
		{
			int playerIndexByName = GetPlayerIndexByName(playerName);
			if (playerIndexByName != -1)
			{
				_sentList.Add(playerName);
				_playersList[playerIndexByName].requestSent = true;
				_battleStatsView.UpdateFriendStatus(playerIndexByName, setAsFriend: false, setAsRequested: true, _friendFunctionsEnabled);
			}
		}

		public void FriendRequestReceived(string playerName)
		{
			int playerIndexByName = GetPlayerIndexByName(playerName);
			if (playerIndexByName != -1)
			{
				_pendingList.Add(playerName);
			}
		}

		public void FriendAccepted(string playerName)
		{
			int playerIndexByName = GetPlayerIndexByName(playerName);
			if (playerIndexByName != -1)
			{
				_sentList.Remove(playerName);
				_pendingList.Remove(playerName);
				_playersList[playerIndexByName].requestSent = false;
				_playersList[playerIndexByName].isFriend = true;
				_battleStatsView.UpdateFriendStatus(playerIndexByName, setAsFriend: true, setAsRequested: false, _friendFunctionsEnabled);
			}
		}

		public void SortPlayerListByStat(InGameStatId statId, bool ascending)
		{
			_sortAscending = ascending;
			_currentStatsId = statId;
			BubbleSort(CompareInGameStats, _currentStatsId);
			UpdatePlayerWidgets();
		}

		public void SortPlayerListByTeam(bool ascending)
		{
			_sortAscending = ascending;
			_currentStatsId = InGameStatId.None;
			BubbleSort(CompareNames);
			BubbleSort(CompareTeams);
			UpdatePlayerWidgets();
		}

		private int CompareInGameStats(BattleStatsData p1, BattleStatsData p2, bool ascending, InGameStatId statId)
		{
			if (p1.stats[statId] > p2.stats[statId])
			{
				return ascending ? 1 : (-1);
			}
			if (p1.stats[statId] < p2.stats[statId])
			{
				return (!ascending) ? 1 : (-1);
			}
			return 0;
		}

		private int CompareTeams(BattleStatsData p1, BattleStatsData p2, bool ascending, InGameStatId statId)
		{
			bool flag = playerTeamsContainer.IsMyTeam(p1.teamId);
			bool flag2 = playerTeamsContainer.IsMyTeam(p2.teamId);
			if (flag && !flag2)
			{
				return ascending ? 1 : (-1);
			}
			if (!flag && flag2)
			{
				return (!ascending) ? 1 : (-1);
			}
			return 0;
		}

		private int CompareNames(BattleStatsData p1, BattleStatsData p2, bool ascending, InGameStatId statId)
		{
			return string.Compare(p1.playerName, p2.playerName, ignoreCase: true);
		}

		private void BubbleSort(CompareFunction compFunction, InGameStatId statId = InGameStatId.None)
		{
			int num = _playersList.Count;
			do
			{
				int num2 = 0;
				for (int i = 1; i <= num - 1; i++)
				{
					if (compFunction(_playersList[i - 1], _playersList[i], _sortAscending, statId) > 0)
					{
						BattleStatsData value = _playersList[i - 1];
						_playersList[i - 1] = _playersList[i];
						_playersList[i] = value;
						num2 = i;
					}
				}
				num = num2;
			}
			while (num != 0);
		}

		public void GoBackToMothership()
		{
			_disableGuiInputControllerHide = false;
			HideBattleStats();
			if (WorldSwitching.IsMultiplayer() && !WorldSwitching.IsCustomGame())
			{
				guiInputController.ShowScreen(GuiScreens.VotingAfterBattleScreen);
				return;
			}
			SwitchToMothershipCommand switchToMothershipCommand = commandFactory.Build<SwitchToMothershipCommand>();
			switchToMothershipCommand.Execute();
		}

		internal void InjectMultiplayerAvatars(Texture2D avatarAtlas, IDictionary<string, Rect> avatarAtlasRects, Texture2D clanAvatarAtlas, IDictionary<string, Rect> clanAvatarAtlasRects)
		{
			AvatarAtlasTexture = avatarAtlas;
			AvatarAtlasRects = avatarAtlasRects;
			ClanAvatarAtlasTexture = clanAvatarAtlas;
			ClanAvatarAtlasRects = clanAvatarAtlasRects;
		}

		private BattleStatsData GetPlayerById(int playerId)
		{
			for (int i = 0; i < _playersList.Count; i++)
			{
				BattleStatsData battleStatsData = _playersList[i];
				if (battleStatsData.playerId == playerId)
				{
					return battleStatsData;
				}
			}
			return null;
		}

		private int GetPlayerIndexByName(string name)
		{
			for (int i = 0; i < _playersList.Count; i++)
			{
				BattleStatsData battleStatsData = _playersList[i];
				if (battleStatsData.playerName == name)
				{
					return i;
				}
			}
			return -1;
		}

		public void ConfigureForBattleArenaStyle()
		{
			_uiStyle = UIStyle.BattleArena;
		}

		public void ConfigureForPitModeStyle()
		{
			_uiStyle = UIStyle.Pit;
		}

		public void ConfigureForTDMModeStyle()
		{
			_uiStyle = UIStyle.TDM;
		}

		public void ConfigureForAIStyle()
		{
			_uiStyle = UIStyle.AI;
		}
	}
}
