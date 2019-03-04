using Authentication;
using Battle;
using LobbyServiceLayer;
using Services.Analytics;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class SwitchToSoloTdmPlanetCommand : IInjectableCommand<SwitchWorldDependency>, ICommand
	{
		private SwitchWorldDependency _dependency;

		[Inject]
		public ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public BattleFoundObserver battleFoundObserver
		{
			private get;
			set;
		}

		[Inject]
		public WorldSwitching worldSwitching
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
		public BattleParameters battleParameters
		{
			private get;
			set;
		}

		[Inject]
		public BattlePlayersMothership battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		public BattleTimer battleTimer
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public ICommand Inject(SwitchWorldDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			battleParameters.Clear();
			battleTimer.GameInitialised();
			lobbyRequestFactory.Create<IGetClanInfosRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, ClanInfo>>(HandleOnclanInfosLoaded)).Execute();
		}

		private void HandleOnclanInfosLoaded(ReadOnlyDictionary<string, ClanInfo> clanInfos)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			serviceFactory.Create<IGetAvatarInfoRequest>().SetAnswer(new ServiceAnswer<AvatarInfo>(delegate(AvatarInfo avatarInfo)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				HandleOnAvatarInfoLoaded(avatarInfo, clanInfos);
			})).Execute();
		}

		private void HandleOnAvatarInfoLoaded(AvatarInfo avatarInfo, ReadOnlyDictionary<string, ClanInfo> clanInfos)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(delegate(ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				SwitchToPlanet(expectedPlayers, avatarInfo, clanInfos);
			})).Execute();
		}

		private void SwitchToPlanet(ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers, AvatarInfo avatarInfo, ReadOnlyDictionary<string, ClanInfo> clanInfos)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			battlePlayers.SetExpectedPlayersForSoloMatches(new Dictionary<string, PlayerDataDependency>((IDictionary<string, PlayerDataDependency>)(object)expectedPlayers));
			Dictionary<string, AvatarInfo> dictionary = new Dictionary<string, AvatarInfo>();
			dictionary[User.Username] = avatarInfo;
			List<int> list = new List<int>();
			for (int i = 0; i < PresetAvatarMap.MAX_PRESET_AVATAR_COUNT; i++)
			{
				if (avatarInfo.UseCustomAvatar)
				{
					list.Add(i);
				}
				else if (avatarInfo.AvatarId != i)
				{
					list.Add(i);
				}
			}
			int num = list.Count;
			Random random = new Random(DateTime.UtcNow.Millisecond);
			IEnumerator<KeyValuePair<string, PlayerDataDependency>> enumerator = (IEnumerator<KeyValuePair<string, PlayerDataDependency>>)(object)expectedPlayers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string key = enumerator.Current.Key;
				if (key != User.Username)
				{
					int index = random.Next(0, num);
					AvatarInfo value = new AvatarInfo(useCustomAvatar: false, list[index]);
					dictionary.Add(key, value);
					int value2 = list[num - 1];
					list[num - 1] = list[index];
					list[index] = value2;
					num--;
					if (num == 0)
					{
						num = list.Count;
					}
				}
			}
			ReadOnlyDictionary<string, AvatarInfo> val = default(ReadOnlyDictionary<string, AvatarInfo>);
			val._002Ector(dictionary);
			ISetMultilayerAvatarInfoRequest setMultilayerAvatarInfoRequest = lobbyRequestFactory.Create<ISetMultilayerAvatarInfoRequest>();
			setMultilayerAvatarInfoRequest.Inject(val);
			setMultilayerAvatarInfoRequest.Execute();
			ILogPlayerEnteredPracticeGameRequest logPlayerEnteredPracticeGameRequest = analyticsRequestFactory.Create<ILogPlayerEnteredPracticeGameRequest>();
			logPlayerEnteredPracticeGameRequest.Execute();
			BattleParametersData battleParameters = new BattleParametersData("localhost", 0, string.Empty, new GameModeKey(GameModeType.PraticeMode), DateTime.UtcNow, null, null, "<singleplayer_guid>");
			EnterBattleDependency dependency = new EnterBattleDependency(expectedPlayers, battleParameters, val, clanInfos);
			battleFoundObserver.EnterBattle(dependency);
			worldSwitching.SwitchToPlanetSinglePlayer(GameModeType.PraticeMode, _dependency.planetToLoad);
		}
	}
}
