using Simulation.SinglePlayer;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Simulation
{
	internal sealed class BattleStatsEngine : SingleEntityViewEngine<AIAgentDataComponentsNode>, IEngine, IInitialize, IWaitForFrameworkDestruction
	{
		private bool _showing;

		private IBattleStatsInputComponent _bsInputComponent;

		private IBattleStatsPresenterComponent _battleStatsPresenterComponent;

		private FasterList<AIAgentDataComponentsNode> _aiBotAgents = new FasterList<AIAgentDataComponentsNode>();

		[Inject]
		public GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		private event Action ShowBattleStats = delegate
		{
		};

		private event Action HideBattleStats = delegate
		{
		};

		private event Action<BSMode> SwitchMode = delegate
		{
		};

		private event Action OnChangeStateToGameEnded = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			gameEndedObserver.OnGameEnded += OnGameEnded;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameEndedObserver.OnGameEnded -= OnGameEnded;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[3]
			{
				typeof(IBattleStatsModeComponent),
				typeof(IBattleStatsPresenterComponent),
				typeof(IBattleStatsInputComponent)
			};
		}

		public void Add(IComponent component)
		{
			if (component is IBattleStatsModeComponent)
			{
				IBattleStatsModeComponent obj = component as IBattleStatsModeComponent;
				SwitchMode += obj.SwitchMode;
			}
			else if (component is IBattleStatsPresenterComponent)
			{
				IBattleStatsPresenterComponent obj2 = component as IBattleStatsPresenterComponent;
				ShowBattleStats += obj2.ShowBattleStats;
				IBattleStatsPresenterComponent obj3 = component as IBattleStatsPresenterComponent;
				HideBattleStats += obj3.HideBattleStats;
				IBattleStatsPresenterComponent obj4 = component as IBattleStatsPresenterComponent;
				OnChangeStateToGameEnded += obj4.ChangeStateToGameEnded;
				_battleStatsPresenterComponent = (component as IBattleStatsPresenterComponent);
				for (int i = 0; i < _aiBotAgents.get_Count(); i++)
				{
					AIAgentDataComponentsNode aIAgentDataComponentsNode = _aiBotAgents.get_Item(i);
					bool isMyTeam = _battleStatsPresenterComponent.CheckIsMyTeam(aIAgentDataComponentsNode.aiBotIdData.teamId);
					RegisterPlayerData registerPlayerData = new RegisterPlayerData(aIAgentDataComponentsNode.aiBotIdData.playerId, aIAgentDataComponentsNode.aiMovementData.playerName, aIAgentDataComponentsNode.aiMovementData.playerName, isMe: false, isMyTeam);
					_battleStatsPresenterComponent.RegisterPlayer(ref registerPlayerData);
				}
			}
			else if (component is IBattleStatsInputComponent)
			{
				_bsInputComponent = (component as IBattleStatsInputComponent);
				(component as IBattleStatsInputComponent).OnInputData += HandleInputData;
			}
		}

		public void Remove(IComponent component)
		{
			if (component is IBattleStatsModeComponent)
			{
				IBattleStatsModeComponent obj = component as IBattleStatsModeComponent;
				SwitchMode -= obj.SwitchMode;
			}
			else if (component is IBattleStatsPresenterComponent)
			{
				IBattleStatsPresenterComponent obj2 = component as IBattleStatsPresenterComponent;
				ShowBattleStats -= obj2.ShowBattleStats;
				IBattleStatsPresenterComponent obj3 = component as IBattleStatsPresenterComponent;
				HideBattleStats -= obj3.HideBattleStats;
				IBattleStatsPresenterComponent obj4 = component as IBattleStatsPresenterComponent;
				OnChangeStateToGameEnded -= obj4.ChangeStateToGameEnded;
			}
			else if (component is IBattleStatsInputComponent)
			{
				_bsInputComponent = null;
				(component as IBattleStatsInputComponent).OnInputData -= HandleInputData;
			}
		}

		private void OnGameEnded(bool won)
		{
			_bsInputComponent.OnInputData -= HandleInputData;
			this.HideBattleStats();
			this.OnChangeStateToGameEnded();
		}

		private void HandleInputData(InputCharacterData input)
		{
			float num = input.data[14];
			if (num > 0f)
			{
				if (!_showing)
				{
					this.SwitchMode(BSMode.ShowBattleStats);
					this.ShowBattleStats();
					_showing = true;
				}
			}
			else if (_showing)
			{
				this.SwitchMode(BSMode.HideBattleStats);
				this.HideBattleStats();
				_showing = false;
			}
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			if (obj != null)
			{
				if (_battleStatsPresenterComponent != null)
				{
					bool isMyTeam = _battleStatsPresenterComponent.CheckIsMyTeam(obj.aiBotIdData.teamId);
					RegisterPlayerData registerPlayerData = new RegisterPlayerData(obj.aiMovementData.playeId, obj.aiMovementData.playerName, obj.aiMovementData.playerName, isMe: false, isMyTeam);
					_battleStatsPresenterComponent.RegisterPlayer(ref registerPlayerData);
				}
				else
				{
					_aiBotAgents.Add(obj);
				}
			}
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			if (obj != null)
			{
				_aiBotAgents.UnorderedRemove(obj);
			}
		}
	}
}
