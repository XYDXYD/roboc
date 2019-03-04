using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;

namespace Simulation
{
	internal sealed class SurrenderControllerClient : IInitialize, IWaitForFrameworkDestruction
	{
		private SurrenderCooldownTimer _cooldownTimer = new SurrenderCooldownTimer();

		private SurrenderVoteTimer _voteTimer = new SurrenderVoteTimer();

		public bool _voteCast = true;

		private bool _surrenderInitiated;

		private int _currentVotesCount;

		[Inject]
		internal MultiplayerGameTimerClient gameTimer
		{
			private get;
			set;
		}

		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		public int playerCooldownSeconds
		{
			get;
			private set;
		}

		public int teamCooldownSeconds
		{
			get;
			private set;
		}

		public int surrenderTimeoutSeconds
		{
			get;
			private set;
		}

		public int initialSurrenderTimeoutSeconds
		{
			get;
			private set;
		}

		public event Action<int> ShowSurrenderGUI = delegate
		{
		};

		public event Action HideSurrenderGUI = delegate
		{
		};

		public event Action ShowPlayerVotedGUI = delegate
		{
		};

		public event Action ShowPlayerInitiatedVoteGUI = delegate
		{
		};

		public event Action<FasterList<bool>, int> OnNumVotesChanged = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(OnGameStart);
			playerCooldownSeconds = 300;
			teamCooldownSeconds = 30;
			surrenderTimeoutSeconds = 20;
			initialSurrenderTimeoutSeconds = 300;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
		}

		public void RegisterOnCooldownTimerStarted(Action onModeChanged)
		{
			_cooldownTimer.onCooldownTimerStarted += onModeChanged;
		}

		public void RegisterOnCooldownTimerEnded(Action onModeChanged)
		{
			_cooldownTimer.onCooldownTimerEnded += onModeChanged;
		}

		public void RegisterOnCooldownSecondsChanged(Action<int> onCooldownSecondsChanged)
		{
			_cooldownTimer.onCooldownSecondsChanged += onCooldownSecondsChanged;
		}

		public void RegisterOnVoteTimerFinished(Action onVoteTimerFinished)
		{
			_voteTimer.onVoteTimerFinished += onVoteTimerFinished;
		}

		public void RegisterOnVoteTimerChanged(Action<double> onVoteTimeChanged)
		{
			_voteTimer.onVoteTimeChanged += onVoteTimeChanged;
		}

		public void UnregisterOnCooldownTimerStarted(Action onModeChanged)
		{
			_cooldownTimer.onCooldownTimerStarted -= onModeChanged;
		}

		public void UnregisterOnCooldownTimerEnded(Action onModeChanged)
		{
			_cooldownTimer.onCooldownTimerEnded -= onModeChanged;
		}

		public void UnregisterOnCooldownSecondsChanged(Action<int> onSecondsChanged)
		{
			_cooldownTimer.onCooldownSecondsChanged -= onSecondsChanged;
		}

		public void UnregisterOnVoteTimerFinished(Action onVoteTimerFinished)
		{
			_voteTimer.onVoteTimerFinished -= onVoteTimerFinished;
		}

		public void UnregisterOnVoteTimerChanged(Action<double> onVoteTimeChanged)
		{
			_voteTimer.onVoteTimeChanged -= onVoteTimeChanged;
		}

		public void StartSurrenderCooldownTimer(float cooldownTime)
		{
			_cooldownTimer.StartCounting(cooldownTime);
		}

		public void StartSurrenderVoteTimer()
		{
			_voteTimer.StartVotingTimer(surrenderTimeoutSeconds);
		}

		public int GetSurrenderCooldownTimeRemaining()
		{
			return _cooldownTimer.GetSecondsLeft();
		}

		public void SetSurrenderTimes(int playerCooldownSecondsNew, int teamCooldownSecondsNew, int surrenderTimeoutSecondsNew, int initialSurrenderTimeoutSecondsNew)
		{
			playerCooldownSeconds = playerCooldownSecondsNew;
			teamCooldownSeconds = teamCooldownSecondsNew;
			surrenderTimeoutSeconds = surrenderTimeoutSecondsNew;
			initialSurrenderTimeoutSeconds = initialSurrenderTimeoutSecondsNew;
			if (gameTimer.GetElapsedTime() > 0f)
			{
				OnGameStart();
			}
		}

		public void SurrenderInitiated()
		{
			_surrenderInitiated = true;
		}

		public void SurrenderVoteStarted(int playersOnTeam)
		{
			this.ShowSurrenderGUI(playersOnTeam);
			_cooldownTimer.StopTimer();
			if (_surrenderInitiated)
			{
				_voteCast = true;
				this.ShowPlayerInitiatedVoteGUI();
			}
			else
			{
				_voteCast = false;
				StartSurrenderVoteTimer();
			}
		}

		public void SurrenderDeclined(float gameTimeElapsedWhenDeclined, int cooldownTime)
		{
			this.HideSurrenderGUI();
			_currentVotesCount = 0;
			_voteTimer.EndVoting();
			float cooldownTime2 = (float)cooldownTime - (gameTimer.GetElapsedTime() - gameTimeElapsedWhenDeclined);
			if (cooldownTime > GetSurrenderCooldownTimeRemaining())
			{
				StartSurrenderCooldownTimer(cooldownTime2);
			}
			else
			{
				StartSurrenderCooldownTimer(GetSurrenderCooldownTimeRemaining());
			}
			_surrenderInitiated = false;
		}

		public void UpdateVotes(FasterList<bool> votes)
		{
			int arg = votes.get_Count() - _currentVotesCount;
			_currentVotesCount = votes.get_Count();
			this.OnNumVotesChanged(votes, arg);
		}

		public void PlayerCastVote()
		{
			_voteCast = true;
			_voteTimer.StopTimer();
			this.ShowPlayerVotedGUI();
		}

		public bool HasPlayerVoted()
		{
			return _voteCast;
		}

		public void UpdateCooldownTimer(float deltaTime)
		{
			_cooldownTimer.Tick(deltaTime);
		}

		public void UpdateVoteTimer(float deltaTime)
		{
			_voteTimer.Tick(deltaTime);
		}

		private void OnGameStart()
		{
			int num = initialSurrenderTimeoutSeconds - (int)Math.Ceiling(gameTimer.GetElapsedTime());
			if (num > 0)
			{
				StartSurrenderCooldownTimer(num);
			}
			else
			{
				StartSurrenderCooldownTimer(1f);
			}
		}
	}
}
