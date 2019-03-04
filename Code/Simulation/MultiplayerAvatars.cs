using Avatars;
using Battle;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class MultiplayerAvatars : IInitialize
	{
		[Inject]
		internal ISocialRequestFactory SocialRequestFactory
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
		internal WorldSwitching WorldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayers BattlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal IMultiAvatarLoader AvatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMap
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			WorldSwitching.OnWorldIsSwitching.Add(ClearAvatarCache());
		}

		public IEnumerator LoadAndInjectAvatars()
		{
			Console.Log("Loading the avatar atlas and injecting");
			List<PlayerDataDependency> playerList = BattlePlayers.GetExpectedPlayersList();
			Dictionary<string, AvatarInfo> avatarInfos = new Dictionary<string, AvatarInfo>(playerList.Count);
			Dictionary<string, AvatarInfo> clanAvatarInfos = new Dictionary<string, AvatarInfo>(playerList.Count);
			List<string> customAvatarNamesToFetch = new List<string>();
			List<AvatarType> avatarTypesToFetch = new List<AvatarType>();
			Dictionary<string, string> clanByPlayer = new Dictionary<string, string>();
			int playerCount = playerList.Count;
			for (int i = 0; i < playerCount; i++)
			{
				PlayerDataDependency playerDataDependency = playerList[i];
				avatarInfos.Add(playerDataDependency.PlayerName, playerDataDependency.AvatarInfo);
				if (playerDataDependency.ClanName != null)
				{
					if (playerDataDependency.ClanAvatarInfo.UseCustomAvatar)
					{
						customAvatarNamesToFetch.Add(playerDataDependency.ClanName);
						avatarTypesToFetch.Add(AvatarType.ClanAvatar);
					}
					clanByPlayer[playerDataDependency.PlayerName] = playerDataDependency.ClanName;
					clanAvatarInfos[playerDataDependency.PlayerName] = playerDataDependency.ClanAvatarInfo;
				}
				if (playerDataDependency.AvatarInfo.UseCustomAvatar)
				{
					customAvatarNamesToFetch.Add(playerDataDependency.PlayerName);
					avatarTypesToFetch.Add(AvatarType.PlayerAvatar);
				}
			}
			Console.Log("waiting for a total of " + customAvatarNamesToFetch.Count + " custom avatars");
			SerialTaskCollection serialTaskCollection = new SerialTaskCollection();
			serialTaskCollection.Add(AvatarLoader.WaitForAllCustomAvatars(customAvatarNamesToFetch, avatarTypesToFetch));
			Dictionary<string, TaskService<Texture2D>> loadPlayerAvatarTasks = new Dictionary<string, TaskService<Texture2D>>();
			Dictionary<string, TaskService<Texture2D>> loadClanAvatarTasks = new Dictionary<string, TaskService<Texture2D>>();
			for (int j = 0; j < customAvatarNamesToFetch.Count; j++)
			{
				IMultiAvatarLoadRequest multiAvatarLoadRequest = SocialRequestFactory.Create<IMultiAvatarLoadRequest>();
				multiAvatarLoadRequest.Inject(new MultiAvatarRequestDependency(customAvatarNamesToFetch[j], avatarTypesToFetch[j]));
				TaskService<Texture2D> taskService = new TaskService<Texture2D>(multiAvatarLoadRequest);
				if (avatarTypesToFetch[j] == AvatarType.PlayerAvatar)
				{
					loadPlayerAvatarTasks[customAvatarNamesToFetch[j]] = taskService;
				}
				else
				{
					loadClanAvatarTasks[customAvatarNamesToFetch[j]] = taskService;
				}
				serialTaskCollection.Add(taskService);
			}
			serialTaskCollection.Add(BuildAtlasesAndInject(loadPlayerAvatarTasks, loadClanAvatarTasks, avatarInfos, clanByPlayer, clanAvatarInfos));
			yield return serialTaskCollection;
		}

		private IEnumerator BuildAtlasesAndInject(Dictionary<string, TaskService<Texture2D>> loadedPlayerAvatarTasks, Dictionary<string, TaskService<Texture2D>> loadedClanAvatarTasks, Dictionary<string, AvatarInfo> avatarInfos, Dictionary<string, string> clanByPlayer, Dictionary<string, AvatarInfo> clanAvatarInfos)
		{
			IGetAvatarAtlasForBattleRequest playerAvatarAtlasRequest = SocialRequestFactory.Create<IGetAvatarAtlasForBattleRequest>();
			IGetClanAvatarAtlasForBattleRequest clanAvatarAtlasRequest = SocialRequestFactory.Create<IGetClanAvatarAtlasForBattleRequest>();
			Dictionary<string, Texture2D> avatarTextures = new Dictionary<string, Texture2D>();
			foreach (KeyValuePair<string, TaskService<Texture2D>> loadedPlayerAvatarTask in loadedPlayerAvatarTasks)
			{
				avatarTextures[loadedPlayerAvatarTask.Key] = loadedPlayerAvatarTask.Value.result;
			}
			playerAvatarAtlasRequest.Inject(new GetAvatarAtlasRequestDependancy(avatarInfos, avatarTextures));
			TaskService<AvatarAtlasForBattle> playerAvatarAtlasTask = new TaskService<AvatarAtlasForBattle>(playerAvatarAtlasRequest);
			yield return (object)new TaskWrapper(playerAvatarAtlasTask);
			avatarTextures.Clear();
			foreach (KeyValuePair<string, TaskService<Texture2D>> loadedClanAvatarTask in loadedClanAvatarTasks)
			{
				avatarTextures[loadedClanAvatarTask.Key] = loadedClanAvatarTask.Value.result;
			}
			foreach (KeyValuePair<string, AvatarInfo> clanAvatarInfo in clanAvatarInfos)
			{
				if (!clanAvatarInfo.Value.UseCustomAvatar)
				{
					int avatarId = clanAvatarInfo.Value.AvatarId;
					string key = clanByPlayer[clanAvatarInfo.Key];
					avatarTextures[key] = PresetAvatarMap.GetAvatarMap().GetPresetAvatar(avatarId);
				}
			}
			clanAvatarAtlasRequest.Inject(new GetClanAvatarAtlasRequestDependancy(avatarTextures, clanByPlayer));
			TaskService<AvatarAtlasForBattle> clanAvatarAtlasTask = new TaskService<AvatarAtlasForBattle>(clanAvatarAtlasRequest);
			yield return (object)new TaskWrapper(clanAvatarAtlasTask);
			AvatarAtlasForBattle playerAvatarAtlas = playerAvatarAtlasTask.result;
			AvatarAtlasForBattle clanAvatarAtlas = clanAvatarAtlasTask.result;
			CommandFactory.Build<InjectSimulationAvatarsCommand>().Inject(playerAvatarAtlas.AtlasTexture, playerAvatarAtlas.AtlasRects, clanAvatarAtlas.AtlasTexture, clanAvatarAtlas.AtlasRects).Execute();
		}

		private IEnumerator ClearAvatarCache()
		{
			IClearAvatarCacheRequest clearRequest = SocialRequestFactory.Create<IClearAvatarCacheRequest>();
			clearRequest.Inject(null, null);
			yield return clearRequest;
		}
	}
}
