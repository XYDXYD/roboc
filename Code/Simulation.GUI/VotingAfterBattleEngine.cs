using RCNetwork.Events;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation.GUI
{
	internal class VotingAfterBattleEngine : MultiEntityViewsEngine<VotingAfterBattleRobotVoteNode, VotingAfterBattleMainWindowNode, VotingAfterBattleRobotWidgetNode>, IGUIDisplay, IQueryingEntityViewEngine, IComponent, IEngine
	{
		private IGameObjectFactory _gameObjectFactory;

		private bool _initialized;

		private VotingAfterBattleRobotWidgetNode[] _sortedRobotWidgetImplementors;

		private Dictionary<int, uint> _scoreboard = new Dictionary<int, uint>();

		private Dictionary<int, RegisterPlayerData> _playerData = new Dictionary<int, RegisterPlayerData>();

		private string[] _sortedTopPlayerNames;

		private string[] _sortedTopDisplayNames;

		private HudStyle _battleHudStyle;

		private VotingAfterBattleMainWindowNode _mainNode;

		private FasterList<UpdateVotingAfterBattleDependency> _savedVotes = new FasterList<UpdateVotingAfterBattleDependency>();

		private UpdateVotingAfterBattleDependency _updateVotingAfterBattleDependency = new UpdateVotingAfterBattleDependency(string.Empty, 0, VoteType.BestLooking);

		[Inject]
		internal INetworkEventManagerClient networkEventManagerClient
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
		internal IMusicManager musicManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer _playerNamesContainer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public HudStyle battleHudStyle => _battleHudStyle;

		public bool doesntHideOnSwitch => false;

		public bool hasBackground => false;

		public bool isScreenBlurred => true;

		public GuiScreens screenType => GuiScreens.VotingAfterBattleScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public unsafe VotingAfterBattleEngine(InGamePlayerStatsUpdatedObserver battleStatsUpdatedObserver, RegisterPlayerObserver registerPlayerObserver, UpdateVotingAfterBattleClientCommandObserver updateVotingAfterBattleClientCommandObserver, GameEndedObserver gameEndedObserver, IGameObjectFactory gameObjectFactory)
		{
			registerPlayerObserver.OnRegisterPlayer += RegisterPlayer;
			battleStatsUpdatedObserver.AddAction(new ObserverAction<InGamePlayerStats>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			updateVotingAfterBattleClientCommandObserver.AddAction(new ObserverAction<UpdateVotingAfterBattleDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			gameEndedObserver.OnGameEnded += HandleGameEnded;
			_gameObjectFactory = gameObjectFactory;
		}

		void IQueryingEntityViewEngine.Ready()
		{
		}

		private void RegisterPlayer(string name, int id, bool isMe, bool isMyTeam)
		{
			string displayName = _playerNamesContainer.GetDisplayName(id);
			RegisterPlayerData registerPlayerData = new RegisterPlayerData(id, name, displayName, isMe, isMyTeam);
			_playerData[registerPlayerData.playerId] = registerPlayerData;
			_scoreboard[registerPlayerData.playerId] = 0u;
		}

		private void HandleVoteReceived(ref UpdateVotingAfterBattleDependency dependency)
		{
			if (_initialized)
			{
				UpdateVoteCounter(ref dependency);
			}
			else
			{
				_savedVotes.Add(dependency);
			}
		}

		private void HandleThresholdReached(int nodeID, string thresholdName)
		{
			VotingAfterBattleRobotVoteNode votingAfterBattleRobotVoteNode = entityViewsDB.QueryEntityView<VotingAfterBattleRobotVoteNode>(nodeID);
			int robotWidgetID = votingAfterBattleRobotVoteNode.votingAfterBattleRobotVoteComponent.RobotWidgetID;
			VotingAfterBattleRobotWidgetNode votingAfterBattleRobotWidgetNode = entityViewsDB.QueryEntityView<VotingAfterBattleRobotWidgetNode>(robotWidgetID);
			votingAfterBattleRobotWidgetNode.votingAfterBattleRobotWidgetComponent.ThresholdUpdated.set_value(thresholdName);
		}

		private void UpdateVoteCounter(ref UpdateVotingAfterBattleDependency dependency)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<VotingAfterBattleRobotVoteNode> val = entityViewsDB.QueryEntityViews<VotingAfterBattleRobotVoteNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				if (val.get_Item(i).votingAfterBattleRobotVoteComponent.Type == dependency.voteType)
				{
					int robotWidgetID = val.get_Item(i).votingAfterBattleRobotVoteComponent.RobotWidgetID;
					VotingAfterBattleRobotWidgetNode votingAfterBattleRobotWidgetNode = entityViewsDB.QueryEntityView<VotingAfterBattleRobotWidgetNode>(robotWidgetID);
					if (votingAfterBattleRobotWidgetNode.votingAfterBattleRobotWidgetComponent.PlayerName.get_value() == dependency.playerName)
					{
						PlayVoteReceivedParticle(val.get_Item(i).votingAfterBattleRobotVoteComponent.ReceiveVoteParticlePrefab);
						val.get_Item(i).votingAfterBattleRobotVoteComponent.CountUpdated.set_value(dependency.amount);
					}
				}
			}
		}

		private void PlayVoteReceivedParticle(GameObject particlePrefab)
		{
			GameObject val = _gameObjectFactory.Build(particlePrefab);
			val.get_transform().SetParent(particlePrefab.get_transform().get_parent(), false);
			val.get_gameObject().SetActive(true);
		}

		private void UpdateScore(ref InGamePlayerStats inGamePlayerStatsData)
		{
			List<InGameStat> inGameStats = inGamePlayerStatsData.inGameStats;
			for (int i = 0; i < inGameStats.Count; i++)
			{
				if (inGameStats[i].ID == InGameStatId.Score || inGameStats[i].ID == InGameStatId.Points)
				{
					_scoreboard[inGamePlayerStatsData.playerId] = inGameStats[i].Score;
				}
			}
		}

		protected override void Add(VotingAfterBattleMainWindowNode node)
		{
			_mainNode = node;
			_mainNode.votingAfterBattleMainWindowComponent.confirmButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleConfirmButtonPressed);
		}

		protected override void Add(VotingAfterBattleRobotWidgetNode node)
		{
			node.votingAfterBattleRobotWidgetComponent.ShowAnimationEnded.NotifyOnValueSet((Action<int, bool>)HandleWidgetShowAnimationEnded);
		}

		protected override void Add(VotingAfterBattleRobotVoteNode node)
		{
			node.votingAfterBattleRobotVoteComponent.ButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleVoteButtonPressed);
			node.votingAfterBattleRobotVoteComponent.ThresholdReached.NotifyOnValueSet((Action<int, string>)HandleThresholdReached);
		}

		protected override void Remove(VotingAfterBattleMainWindowNode node)
		{
			node.votingAfterBattleMainWindowComponent.confirmButtonPressed.StopNotify((Action<int, bool>)HandleConfirmButtonPressed);
		}

		protected override void Remove(VotingAfterBattleRobotVoteNode node)
		{
			node.votingAfterBattleRobotVoteComponent.ButtonPressed.StopNotify((Action<int, bool>)HandleVoteButtonPressed);
			node.votingAfterBattleRobotVoteComponent.ThresholdReached.StopNotify((Action<int, string>)HandleThresholdReached);
		}

		protected override void Remove(VotingAfterBattleRobotWidgetNode node)
		{
			node.votingAfterBattleRobotWidgetComponent.ShowAnimationEnded.StopNotify((Action<int, bool>)HandleWidgetShowAnimationEnded);
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool Hide()
		{
			if (!_mainNode.votingAfterBattleMainWindowComponent.active.get_value())
			{
				return false;
			}
			_mainNode.votingAfterBattleMainWindowComponent.active.set_value(false);
			return true;
		}

		public bool IsActive()
		{
			if (_mainNode == null)
			{
				return false;
			}
			return _mainNode.votingAfterBattleMainWindowComponent.active.get_value();
		}

		public GUIShowResult Show()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			musicManager.SwitchToVotingScreenLoop();
			_mainNode.votingAfterBattleMainWindowComponent.active.set_value(true);
			int num = 0;
			FasterReadOnlyList<VotingAfterBattleRobotWidgetNode> val = entityViewsDB.QueryEntityViews<VotingAfterBattleRobotWidgetNode>();
			int num2 = (_scoreboard.Count <= val.get_Count()) ? _scoreboard.Count : val.get_Count();
			_sortedTopPlayerNames = new string[num2];
			_sortedTopDisplayNames = new string[num2];
			_sortedRobotWidgetImplementors = new VotingAfterBattleRobotWidgetNode[num2];
			_mainNode.votingAfterBattleMainWindowComponent.numPlayersOnPedestal = num2;
			for (int i = 0; i < val.get_Count(); i++)
			{
				bool value = val.get_Item(i).votingAfterBattleRobotWidgetComponent.PedestalPosition < num2;
				val.get_Item(i).votingAfterBattleRobotWidgetComponent.Active.set_value(value);
			}
			for (int j = 0; j < val.get_Count(); j++)
			{
				if (val.get_Item(j).votingAfterBattleRobotWidgetComponent.Active.get_value())
				{
					_sortedRobotWidgetImplementors[val.get_Item(j).votingAfterBattleRobotWidgetComponent.PedestalPosition] = val.get_Item(j);
				}
			}
			foreach (KeyValuePair<int, uint> item in (from key in _scoreboard
			orderby key.Value descending, key.Key
			select key).Take(num2))
			{
				_sortedTopPlayerNames[num] = _playerData[item.Key].playerName;
				_sortedTopDisplayNames[num] = _playerData[item.Key].displayName;
				_sortedRobotWidgetImplementors[num].votingAfterBattleRobotWidgetComponent.PlayerName.set_value(_sortedTopPlayerNames[num]);
				_sortedRobotWidgetImplementors[num].votingAfterBattleRobotWidgetComponent.DisplayName.set_value(_sortedTopDisplayNames[num]);
				_sortedRobotWidgetImplementors[num].votingAfterBattleRobotWidgetComponent.NumPlayersOnPedestal.set_value(num2);
				bool isMe = _playerData[item.Key].isMe;
				if (isMe)
				{
					_sortedRobotWidgetImplementors[num].votingAfterBattleRobotWidgetComponent.IsMe.set_value(isMe);
				}
				else
				{
					_sortedRobotWidgetImplementors[num].votingAfterBattleRobotWidgetComponent.IsMyTeam.set_value(_playerData[item.Key].isMyTeam);
				}
				num++;
			}
			_mainNode.panelSizeCompent.Refresh();
			return GUIShowResult.Showed;
		}

		private void HandleGameEnded(bool won)
		{
			_mainNode.votingAfterBattleMainWindowComponent.victory.set_value(won);
		}

		private void HandleConfirmButtonPressed(int nodeID, bool pressed)
		{
			SwitchToMothershipCommand switchToMothershipCommand = commandFactory.Build<SwitchToMothershipCommand>();
			switchToMothershipCommand.Execute();
		}

		private void HandleVoteButtonPressed(int nodeID, bool pressed)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<VotingAfterBattleRobotVoteNode> val = entityViewsDB.QueryEntityViews<VotingAfterBattleRobotVoteNode>();
			VotingAfterBattleRobotVoteNode votingAfterBattleRobotVoteNode = entityViewsDB.QueryEntityView<VotingAfterBattleRobotVoteNode>(nodeID);
			VoteType type = votingAfterBattleRobotVoteNode.votingAfterBattleRobotVoteComponent.Type;
			int robotWidgetID = votingAfterBattleRobotVoteNode.votingAfterBattleRobotVoteComponent.RobotWidgetID;
			VotingAfterBattleRobotWidgetNode votingAfterBattleRobotWidgetNode = entityViewsDB.QueryEntityView<VotingAfterBattleRobotWidgetNode>(robotWidgetID);
			string value = votingAfterBattleRobotWidgetNode.votingAfterBattleRobotWidgetComponent.PlayerName.get_value();
			_updateVotingAfterBattleDependency.SetFields(value, pressed ? 1 : (-1), type);
			networkEventManagerClient.SendEventToServer(NetworkEvent.UpdateVotingAfterBattle, _updateVotingAfterBattleDependency);
			for (int i = 0; i < val.get_Count(); i++)
			{
				if (val.get_Item(i).get_ID() != nodeID && val.get_Item(i).votingAfterBattleRobotVoteComponent.Type == type)
				{
					val.get_Item(i).votingAfterBattleRobotVoteComponent.ButtonEnabled.set_value(!pressed);
				}
			}
		}

		private void HandleWidgetShowAnimationEnded(int nodeID, bool ended)
		{
			DispatchOnSet<int> numWidgetShowAnimationsEnded = _mainNode.votingAfterBattleMainWindowComponent.numWidgetShowAnimationsEnded;
			numWidgetShowAnimationsEnded.set_value(numWidgetShowAnimationsEnded.get_value() + 1);
			if (_mainNode.votingAfterBattleMainWindowComponent.numWidgetShowAnimationsEnded.get_value() == _sortedTopPlayerNames.Length)
			{
				for (int i = 0; i < _savedVotes.get_Count(); i++)
				{
					UpdateVotingAfterBattleDependency dependency = _savedVotes.get_Item(i);
					UpdateVoteCounter(ref dependency);
				}
				_initialized = true;
			}
		}
	}
}
