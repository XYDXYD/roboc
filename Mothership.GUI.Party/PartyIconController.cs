using Authentication;
using Avatars;
using Mothership.GUI.CustomGames;
using Services.Web.Photon;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class PartyIconController : IPartyIconController, IInitialize, IWaitForFrameworkDestruction
	{
		private bool _blockedFromInteraction;

		private string _playerAssignedToSlot = string.Empty;

		private string _partyLeader = string.Empty;

		private PartyIconInfo _iconInfo;

		private ISharedPartyIcon _guiView;

		private PresetAvatarMap _avatarMap;

		private TiersData _tiersData;

		[Inject]
		internal ICPUPower cpuPower
		{
			get;
			set;
		}

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController guiInputController
		{
			get;
			set;
		}

		[Inject]
		internal IMultiAvatarLoader AvatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver AvatarObserver
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
		internal LoadingIconPresenter loadingPresenter
		{
			get;
			set;
		}

		public string PlayerAssignedToSlot => _playerAssignedToSlot;

		public bool IsBlockedFromInteraction
		{
			get
			{
				GuiScreens activeScreen = guiInputController.GetActiveScreen();
				if (activeScreen == GuiScreens.BattleCountdown)
				{
					return true;
				}
				if (User.Username == _playerAssignedToSlot)
				{
					return false;
				}
				return _blockedFromInteraction;
			}
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			_iconInfo = new PartyIconInfo(emptySlot_: false, isLeader_: false, robotTierMatchesLeaderTier_: false, isready_: false);
			_avatarMap = PresetAvatarMapProvider.GetAvatarMap();
			guiInputController.OnScreenStateChange += RefreshInteractivityState;
			AvatarObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			TaskRunner.get_Instance().Run(LoadDependantDataForTiers());
		}

		private IEnumerator LoadDependantDataForTiers()
		{
			loadingPresenter.NotifyLoading("LoadingTiersData");
			ILoadTiersBandingRequest loadTiersBandingReq = serviceFactory.Create<ILoadTiersBandingRequest>();
			TaskService<TiersData> loadTiersBandingTS = new TaskService<TiersData>(loadTiersBandingReq);
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadTiersBandingTS, delegate
			{
				loadingPresenter.NotifyLoading("LoadingTiersData");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("LoadingTiersData");
			});
			yield return handleTSWithError.GetEnumerator();
			if (loadTiersBandingTS.succeeded)
			{
				_tiersData = loadTiersBandingTS.result;
			}
			loadingPresenter.NotifyLoadingDone("LoadingTiersData");
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
		{
			if (_playerAssignedToSlot == avatarData.avatarName && avatarData.avatarType == AvatarType.PlayerAvatar)
			{
				_guiView.AssignAvatarTexture(avatarData.texture);
			}
		}

		private void RefreshInteractivityState()
		{
			if (_guiView != null)
			{
				if (IsBlockedFromInteraction)
				{
					_guiView.Disable();
				}
				else
				{
					_guiView.Enable();
				}
			}
		}

		public void ReceiveMessage(object message)
		{
			if (message is PartyIconTierChangeMessage)
			{
				PartyIconTierChangeMessage partyIconTierChangeMessage = (PartyIconTierChangeMessage)message;
				if (partyIconTierChangeMessage.UserName == _playerAssignedToSlot)
				{
					if (partyIconTierChangeMessage.TierTextString != string.Empty)
					{
						_guiView.SetPlayerRobotTier(partyIconTierChangeMessage.TierTextString);
					}
					else
					{
						TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshSlotRobotTier);
					}
				}
			}
			else if (message is PartyIconCannotInteractMessage)
			{
				PartyIconCannotInteractMessage partyIconCannotInteractMessage = (PartyIconCannotInteractMessage)message;
				_blockedFromInteraction = partyIconCannotInteractMessage.preventInteraction;
				RefreshInteractivityState();
			}
			else
			{
				if (message.GetType() != typeof(PartyMemberDataChangedMessage))
				{
					return;
				}
				PartyMemberDataChangedMessage partyMemberDataChangedMessage = (PartyMemberDataChangedMessage)message;
				if (partyMemberDataChangedMessage.positionIndex == _guiView.GetPositionIndex())
				{
					if (partyMemberDataChangedMessage.playerName == string.Empty)
					{
						_playerAssignedToSlot = string.Empty;
						_guiView.ChangeIconStatus(PartyIconState.AddMemberState);
						_iconInfo.isLeader = false;
						_iconInfo.emptySlot = true;
						_guiView.UpdateBackgroundColour(_iconInfo);
						_guiView.HideTierStatus();
					}
					else
					{
						_iconInfo.isLeader = partyMemberDataChangedMessage.isLeader;
						if (partyMemberDataChangedMessage.playerName != _playerAssignedToSlot)
						{
							_playerAssignedToSlot = partyMemberDataChangedMessage.playerName;
							_partyLeader = partyMemberDataChangedMessage.partyLeaderName;
							TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshSlotRobotTier);
						}
						AssignPartyMemberAvatar(partyMemberDataChangedMessage.playerName, partyMemberDataChangedMessage.avatarInfo);
						SetIconForPartyMemberStatus(partyMemberDataChangedMessage.status, partyMemberDataChangedMessage.isLeader);
					}
					_guiView.SetPlayerName(partyMemberDataChangedMessage.playerDisplayName);
				}
				RefreshInteractivityState();
			}
		}

		private IEnumerator RefreshSlotRobotTier()
		{
			uint maxCosmeticCPUPool = cpuPower.MaxCosmeticCpuPool;
			ILoadPlayerRobotRankingRequest request = serviceFactory.Create<ILoadPlayerRobotRankingRequest>();
			request.Inject(_playerAssignedToSlot);
			request.ClearCache();
			TaskService<RankingAndCPU> task2 = new TaskService<RankingAndCPU>(request);
			yield return task2;
			if (task2.succeeded)
			{
				yield return cpuPower.IsLoadedEnumerator();
				RankingAndCPU robotRankingAndCPU2 = task2.result;
				uint totalCPU2 = (uint)robotRankingAndCPU2.TotalCPU;
				uint totalCosmeticCPU2 = (uint)robotRankingAndCPU2.TotalCosmeticCPU;
				uint robotRanking2 = (uint)robotRankingAndCPU2.Ranking;
				uint actualCPU2 = RobotCPUCalculator.CalculateRobotActualCPU(totalCPU2, totalCosmeticCPU2, maxCosmeticCPUPool);
				bool isMegabot2 = actualCPU2 > cpuPower.MaxCpuPower;
				string tier = RRAndTiers.ConvertRobotRankingToTierString(robotRanking2, isMegabot2, _tiersData);
				_guiView.SetPlayerRobotTier(tier);
				if (_partyLeader == _playerAssignedToSlot)
				{
					_iconInfo.isLeader = true;
					yield return null;
				}
				request = serviceFactory.Create<ILoadPlayerRobotRankingRequest>();
				request.Inject(_partyLeader);
				request.ClearCache();
				task2 = new TaskService<RankingAndCPU>(request);
				yield return task2;
				if (task2.succeeded)
				{
					robotRankingAndCPU2 = task2.result;
					totalCPU2 = (uint)robotRankingAndCPU2.TotalCPU;
					totalCosmeticCPU2 = (uint)robotRankingAndCPU2.TotalCosmeticCPU;
					robotRanking2 = (uint)robotRankingAndCPU2.Ranking;
					actualCPU2 = RobotCPUCalculator.CalculateRobotActualCPU(totalCPU2, totalCosmeticCPU2, maxCosmeticCPUPool);
					isMegabot2 = (actualCPU2 > cpuPower.MaxCpuPower);
					string leaderTier = RRAndTiers.ConvertRobotRankingToTierString(robotRanking2, isMegabot2, _tiersData);
					_iconInfo.robotTierMatchesLeaderTier = ((leaderTier == tier) ? true : false);
					_guiView.UpdateBackgroundColour(_iconInfo);
				}
			}
		}

		private void AssignPartyMemberAvatar(string player, AvatarInfo avatarInfo)
		{
			if (avatarInfo.UseCustomAvatar)
			{
				AvatarLoader.RequestAvatar(AvatarType.PlayerAvatar, player);
				return;
			}
			Texture2D presetAvatar = _avatarMap.GetPresetAvatar(avatarInfo.AvatarId);
			_guiView.AssignAvatarTexture(presetAvatar);
		}

		private void SetIconForPartyMemberStatus(PlatoonMember.MemberStatus status, bool isLeader)
		{
			switch (status)
			{
			case PlatoonMember.MemberStatus.Invited:
				_iconInfo.isReadyState = false;
				_iconInfo.emptySlot = true;
				_guiView.ChangeIconStatus(PartyIconState.MemberPendingAccept);
				break;
			case PlatoonMember.MemberStatus.InQueue:
				_guiView.ChangeIconStatus(PartyIconState.MemberWaitingInQueue);
				_iconInfo.emptySlot = false;
				_iconInfo.isReadyState = true;
				break;
			case PlatoonMember.MemberStatus.InBattle:
				_iconInfo.isReadyState = true;
				_iconInfo.emptySlot = false;
				_guiView.ChangeIconStatus(PartyIconState.MemberInGame);
				break;
			case PlatoonMember.MemberStatus.Ready:
				_iconInfo.isReadyState = false;
				_iconInfo.emptySlot = false;
				_guiView.ChangeIconStatus(PartyIconState.MemberShowAvatar);
				break;
			}
			_iconInfo.isLeader = isLeader;
			_guiView.UpdateBackgroundColour(_iconInfo);
		}

		public void RegisterView(ISharedPartyIcon guiView)
		{
			_guiView = guiView;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			AvatarObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public bool IsLeader()
		{
			return _iconInfo.isLeader;
		}
	}
}
