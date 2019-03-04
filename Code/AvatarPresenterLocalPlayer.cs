using Authentication;
using Avatars;
using Mothership;
using Mothership.GUI.CustomGames;
using Mothership.GUI.Party;
using Services.Web.Photon;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

internal class AvatarPresenterLocalPlayer : IInitialize, IWaitForFrameworkDestruction
{
	private PartyIconInfo _iconInfo;

	private AvatarViewLocalPlayer _avatarView;

	private string _platoonLeader;

	private bool _localAvatarIsCustom;

	private TiersData _tiersData;

	[Inject]
	internal ICPUPower cpuPower
	{
		get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory ServiceRequestFactory
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
	internal ICommandFactory CommandFactory
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

	[Inject]
	internal LoadingIconPresenter loadingPresenter
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
	internal IMultiAvatarLoader MultiAvatarLoader
	{
		private get;
		set;
	}

	[Inject]
	internal GarageChangedObserver garageChangedObserver
	{
		get;
		set;
	}

	unsafe void IInitialize.OnDependenciesInjected()
	{
		_localAvatarIsCustom = false;
		_iconInfo = new PartyIconInfo(emptySlot_: false, isLeader_: false, robotTierMatchesLeaderTier_: false, isready_: false);
		AvatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		garageChangedObserver.AddAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		TaskRunner.get_Instance().Run(LoadDependantDataForTiers());
	}

	unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		AvatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		garageChangedObserver.RemoveAction(new ObserverAction<GarageSlotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private void OnGarageSlotChanged(ref GarageSlotDependency dependancy)
	{
		socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
		{
			if (platoon.isInPlatoon)
			{
				_platoonLeader = platoon.leader;
				TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshSlotRobotTierWhenInPlatoon);
			}
		}, delegate
		{
		})).Execute();
	}

	private IEnumerator LoadDependantDataForTiers()
	{
		loadingPresenter.NotifyLoading("LoadingTiersData");
		ILoadTiersBandingRequest loadTiersBandingReq = ServiceRequestFactory.Create<ILoadTiersBandingRequest>();
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

	public void SetCustomAvatarExpected()
	{
		_avatarView.Texture.set_mainTexture(AvatarUtils.StillLoadingTexture);
		_localAvatarIsCustom = true;
	}

	public void ReceiveMessage(object message)
	{
		if (message.GetType() == typeof(ButtonType))
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.ShowAPanel)
			{
				ShowAvatarSelectionScreen();
			}
		}
		else if (message is PartyIconTierChangeMessage)
		{
			PartyIconTierChangeMessage partyIconTierChangeMessage = (PartyIconTierChangeMessage)message;
			int newTier = partyIconTierChangeMessage.NewTier;
			string tierTextString = partyIconTierChangeMessage.TierTextString;
			string theirName = partyIconTierChangeMessage.UserName;
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
			{
				_platoonLeader = platoon.leader;
				if (platoon.isInPlatoon && platoon.leader == theirName)
				{
					TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshSlotRobotTierWhenInPlatoon);
				}
			}, delegate
			{
			})).Execute();
		}
		else if (message.GetType() == typeof(SetOwnPartyStatusMessage))
		{
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
			{
				SetOwnPartyStatusMessage setOwnPartyStatusMessage = (SetOwnPartyStatusMessage)message;
				_iconInfo.isReadyState = setOwnPartyStatusMessage.WeAreInQueue;
				_iconInfo.isLeader = setOwnPartyStatusMessage.WeAreLeader;
				if (!setOwnPartyStatusMessage.WeAreLeader && platoon.isInPlatoon)
				{
					_platoonLeader = platoon.leader;
					TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshSlotRobotTierWhenInPlatoon);
				}
				else
				{
					_iconInfo.robotTierMatchesLeaderTier = true;
					_avatarView.UpdateBackgroundColour(_iconInfo, platoon.isInPlatoon);
				}
			}, delegate
			{
			})).Execute();
		}
		else if (message.GetType() == typeof(SetOwnPartyRobotTierMessage))
		{
			socialRequestFactory.Create<IGetPlatoonDataRequest>().SetAnswer(new ServiceAnswer<Platoon>(delegate(Platoon platoon)
			{
				SetOwnPartyRobotTierMessage setOwnPartyRobotTierMessage = message as SetOwnPartyRobotTierMessage;
				_avatarView.SetPlayerRobotTier(setOwnPartyRobotTierMessage.RobotTierDisplayString);
				_avatarView.UpdateBackgroundColour(_iconInfo, platoon.isInPlatoon);
			}, delegate
			{
			})).Execute();
		}
	}

	private IEnumerator RefreshSlotRobotTierWhenInPlatoon()
	{
		uint maxCosmeticCPUPool = cpuPower.MaxCosmeticCpuPool;
		ILoadPlayerRobotRankingRequest request = ServiceRequestFactory.Create<ILoadPlayerRobotRankingRequest>();
		request.Inject(User.Username);
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
			_avatarView.SetPlayerRobotTier(tier);
			request = ServiceRequestFactory.Create<ILoadPlayerRobotRankingRequest>();
			request.Inject(_platoonLeader);
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
				_avatarView.UpdateBackgroundColour(_iconInfo, isInParty: true);
			}
		}
	}

	public void RegisterView(AvatarViewLocalPlayer avatarView)
	{
		_avatarView = avatarView;
		Hide();
		ServiceRequestFactory.Create<IGetAvatarInfoRequest>().SetAnswer(new ServiceAnswer<AvatarInfo>(ApplyAvatarAndShow, GetAvatarInfoFailed)).Execute();
	}

	public void ApplyAvatarAndShow(AvatarInfo avatarInfo)
	{
		_localAvatarIsCustom = avatarInfo.UseCustomAvatar;
		if (avatarInfo.UseCustomAvatar)
		{
			MultiAvatarLoader.RequestAvatar(AvatarType.PlayerAvatar, AvatarUtils.LocalPlayerAvatarName);
		}
		else
		{
			_avatarView.Texture.set_mainTexture(PresetAvatarMapProvider.GetPresetAvatar(avatarInfo.AvatarId));
		}
		Show();
	}

	private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
	{
		if (avatarData.avatarName == AvatarUtils.LocalPlayerAvatarName && avatarData.avatarType == AvatarType.PlayerAvatar && _localAvatarIsCustom)
		{
			_avatarView.Texture.set_mainTexture(avatarData.texture);
		}
	}

	private void GetAvatarInfoFailed(ServiceBehaviour serviceBehaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
	}

	private void ShowAvatarSelectionScreen()
	{
		string @string = StringTableBase<StringTable>.Instance.GetString("strAvatarTitle");
		ShowAvatarSelectionScreenCommandDependancy dependency = new ShowAvatarSelectionScreenCommandDependancy(@string, LoadLocalPlayerAvatarInfo_: true, OnSelectionCallback, CustomAvatarCannotBeSelected_: false);
		CommandFactory.Build<ShowAvatarSelectionScreenCommand>().Inject(dependency).Execute();
	}

	private void OnSelectionCallback(ShowAvatarSelectionScreenCommandCallbackParameters callbackParams)
	{
		TaskRunner.get_Instance().Run(SaveAndUpdate(callbackParams.AvatarInfo));
	}

	private IEnumerator SaveAndUpdate(AvatarInfo newAvatarInfo)
	{
		loadingPresenter.NotifyLoading("AvatarSelection");
		ISetAvatarInfoRequest request = ServiceRequestFactory.Create<ISetAvatarInfoRequest>();
		request.Inject(newAvatarInfo);
		TaskService task2 = new TaskService(request);
		yield return new HandleTaskServiceWithError(task2, delegate
		{
			loadingPresenter.NotifyLoading("AvatarSelection");
		}, delegate
		{
			loadingPresenter.NotifyLoadingDone("AvatarSelection");
		}).GetEnumerator();
		loadingPresenter.NotifyLoadingDone("AvatarSelection");
		CommandFactory.Build<SetLocalPlayerAvatarCommand>().Inject(newAvatarInfo).Execute();
		loadingPresenter.NotifyLoading("AvatarSelection");
		IAvatarUpdatedRequest avatarUpdatedRequest = socialRequestFactory.Create<IAvatarUpdatedRequest>();
		task2 = new TaskService(avatarUpdatedRequest);
		yield return new HandleTaskServiceWithError(task2, delegate
		{
			loadingPresenter.NotifyLoading("AvatarSelection");
		}, delegate
		{
			loadingPresenter.NotifyLoadingDone("AvatarSelection");
		}).GetEnumerator();
		loadingPresenter.NotifyLoadingDone("AvatarSelection");
	}

	private void Show()
	{
		_avatarView.get_gameObject().SetActive(true);
	}

	private void Hide()
	{
		_avatarView.get_gameObject().SetActive(false);
	}
}
