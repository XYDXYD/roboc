using Authentication;
using Fabric;
using SocialServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class PartyAlertBarController : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private const string strSelectGameModeToReadyUp = "strSelectGameModeToReadyUp";

		private const string strPartyWaitingReadyUp = "strPartyWaitingReadyUp";

		private const string strPartyLeaderSelectGameMode = "strPartyLeaderSelectGameMode";

		private const string strPartyLeaderLaunchTheGame = "strPartyLeaderLaunchTheGame";

		private const string strPartyLeaderSelectAGameModeToPlay = "strPartyLeaderSelectAGameModeToPlay";

		private const string strWaitingForYourParty = "strWaitingForYourParty";

		private const string strEnteringQueue = "strEnteringQueue";

		private bool _partyInvitationPending;

		private bool _waitingSoundEnabled;

		private DateTime _playSoundTimerStart;

		private double _soundPeriod = 1.0;

		private PlatoonQueueState _platoonQueueState;

		private IServiceEventContainer _eventContainer;

		private PartyAlertBarView _view;

		[Inject]
		internal ISocialEventContainerFactory socialEventContainerFactory
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

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)WaitingSoundMonitor);
			_eventContainer = socialEventContainerFactory.Create();
			_eventContainer.ListenTo<IPlatoonChangedEventListener, Platoon>(HandlePlatoonChanged);
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(OnLanguageChanged));
		}

		public void OnFrameworkDestroyed()
		{
			if (_eventContainer != null)
			{
				_eventContainer.Dispose();
				_eventContainer = null;
			}
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(OnLanguageChanged));
		}

		public void RegisterView(PartyAlertBarView view)
		{
			_view = view;
		}

		public void UnregisterView()
		{
			_view = null;
		}

		public void SetSoundPeriod(float period)
		{
			_soundPeriod = period;
		}

		public void Listen(object message)
		{
			if (message is PartyInvitationReceivedMessage)
			{
				_partyInvitationPending = true;
			}
			else if (message is HidePartyInvitationDialogMessage)
			{
				_partyInvitationPending = false;
			}
		}

		private void HandlePlatoonChanged(Platoon partyData)
		{
			PlatoonQueueState platoonQueueState = PlatoonQueueState.NoParty;
			if (partyData.isInPlatoon)
			{
				int num = 0;
				int num2 = 0;
				bool flag = false;
				for (int i = 0; i < partyData.members.Length; i++)
				{
					if (partyData.members[i].Status == PlatoonMember.MemberStatus.InQueue)
					{
						if (partyData.members[i].Name.Equals(User.Username, StringComparison.CurrentCultureIgnoreCase))
						{
							flag = true;
						}
						num2++;
						num++;
					}
					else if (partyData.members[i].Status == PlatoonMember.MemberStatus.Ready)
					{
						num2++;
					}
				}
				platoonQueueState = (flag ? ((num != partyData.members.Length) ? PlatoonQueueState.QueuingWaitingOnParty : PlatoonQueueState.AllPlayersQueued) : (partyData.GetIsPlatoonLeader() ? ((num == partyData.members.Length - 1) ? PlatoonQueueState.LeaderPartyWaitingForYou : ((num2 > 1) ? PlatoonQueueState.LeaderReadyUp : PlatoonQueueState.NoParty)) : ((num > 0) ? PlatoonQueueState.MemberPartyWaitingForYou : ((num2 > 1) ? PlatoonQueueState.MemberReadyUp : PlatoonQueueState.NoParty))));
			}
			bool waitingSoundEnabled = _waitingSoundEnabled;
			_waitingSoundEnabled = (platoonQueueState == PlatoonQueueState.LeaderPartyWaitingForYou || platoonQueueState == PlatoonQueueState.MemberPartyWaitingForYou);
			if (_waitingSoundEnabled != waitingSoundEnabled)
			{
				_playSoundTimerStart = DateTime.UtcNow;
			}
			if (_platoonQueueState != platoonQueueState)
			{
				UpdatePlatoonState(platoonQueueState);
				_platoonQueueState = platoonQueueState;
			}
		}

		private void UpdatePlatoonState(PlatoonQueueState state)
		{
			switch (state)
			{
			case PlatoonQueueState.NoParty:
				_view.Hide();
				break;
			case PlatoonQueueState.MemberReadyUp:
				_view.SetStateGreen(StringTableBase<StringTable>.Instance.GetString("strSelectGameModeToReadyUp").ToUpper());
				break;
			case PlatoonQueueState.MemberPartyWaitingForYou:
				_view.SetStateRed(StringTableBase<StringTable>.Instance.GetString("strPartyWaitingReadyUp").ToUpper());
				break;
			case PlatoonQueueState.LeaderReadyUp:
				_view.SetStateGreen(StringTableBase<StringTable>.Instance.GetString("strPartyLeaderSelectGameMode").ToUpper());
				break;
			case PlatoonQueueState.LeaderPartyWaitingForYou:
				_view.SetStateRed(string.Format("{0}{1}{2}", StringTableBase<StringTable>.Instance.GetString("strPartyLeaderLaunchTheGame").ToUpper(), "\n", StringTableBase<StringTable>.Instance.GetString("strPartyLeaderSelectAGameModeToPlay").ToUpper()));
				break;
			case PlatoonQueueState.QueuingWaitingOnParty:
				_view.SetStateGreen(StringTableBase<StringTable>.Instance.GetString("strWaitingForYourParty").ToUpper());
				break;
			case PlatoonQueueState.AllPlayersQueued:
				_view.SetStateGreen(StringTableBase<StringTable>.Instance.GetString("strEnteringQueue").ToUpper());
				break;
			}
		}

		private IEnumerator WaitingSoundMonitor()
		{
			while (true)
			{
				if (_waitingSoundEnabled || _partyInvitationPending)
				{
					DateTime utcNow = DateTime.UtcNow;
					double elapsedTime = (utcNow - _playSoundTimerStart).TotalSeconds;
					if (elapsedTime > _soundPeriod)
					{
						if (!_partyInvitationPending)
						{
							EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.LobbyCountdownLoud));
						}
						_playSoundTimerStart = utcNow;
					}
				}
				yield return null;
			}
		}

		private void OnLanguageChanged()
		{
			UpdatePlatoonState(_platoonQueueState);
		}
	}
}
