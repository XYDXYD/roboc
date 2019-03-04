using Avatars;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
	internal sealed class LobbyPlayerListPresenter : IInitialize, IWaitForFrameworkDestruction, ILobbyPlayerListPresenter
	{
		private readonly List<LobbyPlayerListView> _views = new List<LobbyPlayerListView>();

		private readonly Dictionary<string, Action<Texture2D>> _playerAvatarSetters = new Dictionary<string, Action<Texture2D>>();

		private readonly Dictionary<string, Action<Texture2D>> _clanAvatarSetters = new Dictionary<string, Action<Texture2D>>();

		private CPUSettingsDependency _cpuSettings;

		[Inject]
		internal MachineSpawnDispatcher networkMachineDispatcher
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
		internal IMultiAvatarLoader avatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver avatarAvailableObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			private get;
			set;
		}

		public int MaxRegularCPU => (int)_cpuSettings.maxRegularCpu;

		unsafe void IInitialize.OnDependenciesInjected()
		{
			networkMachineDispatcher.OnPlayerSpawnedIn += OnPlayerJoined;
			networkMachineDispatcher.OnPlayerRespawnedIn += OnPlayerJoined;
			networkMachineDispatcher.OnPlayerSpawnedOut += OnPlayerLeft;
			avatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadCPUSettings);
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
		{
			if (avatarData.avatarType == AvatarType.PlayerAvatar)
			{
				if (_playerAvatarSetters.ContainsKey(avatarData.avatarName))
				{
					_playerAvatarSetters[avatarData.avatarName](avatarData.texture);
				}
			}
			else if (_clanAvatarSetters.ContainsKey(avatarData.avatarName))
			{
				_clanAvatarSetters[avatarData.avatarName](avatarData.texture);
			}
		}

		public void AddView(LobbyPlayerListView view)
		{
			_views.Add(view);
			view.get_gameObject().SetActive(WorldSwitching.IsMultiplayer());
		}

		public void RemoveView(LobbyPlayerListView view)
		{
			_views.Remove(view);
		}

		internal void OnPlayerJoined(SpawnInParametersPlayer spawnInParameters)
		{
			for (int i = 0; i < _views.Count; i++)
			{
				_views[i].PlayerJoined(playerNamesContainer.GetPlayerName(spawnInParameters.playerId), spawnInParameters.teamId);
			}
		}

		internal void OnPlayerLeft(SpawnOutParameters paramaters)
		{
			for (int i = 0; i < _views.Count; i++)
			{
				_views[i].PlayerLeft(playerNamesContainer.GetPlayerName(paramaters.playerId));
			}
		}

		public void LoadAvatars(Dictionary<string, Action<Texture2D>> playerAvatarTextures, Dictionary<string, Action<Texture2D>> clanAvatarTextures, LobbyPlayerListView view)
		{
			foreach (KeyValuePair<string, Action<Texture2D>> playerAvatarTexture in playerAvatarTextures)
			{
				_playerAvatarSetters.Add(playerAvatarTexture.Key, playerAvatarTexture.Value);
				avatarLoader.RequestAvatar(AvatarType.PlayerAvatar, playerAvatarTexture.Key);
			}
			foreach (KeyValuePair<string, Action<Texture2D>> clanAvatarTexture in clanAvatarTextures)
			{
				_clanAvatarSetters.Add(clanAvatarTexture.Key, clanAvatarTexture.Value);
				avatarLoader.RequestAvatar(AvatarType.ClanAvatar, clanAvatarTexture.Key);
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			avatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			networkMachineDispatcher.OnPlayerSpawnedIn -= OnPlayerJoined;
			networkMachineDispatcher.OnPlayerSpawnedOut -= OnPlayerLeft;
		}

		private IEnumerator LoadCPUSettings()
		{
			TaskService<CPUSettingsDependency> loadCPUTaskService = serviceFactory.Create<ILoadCpuSettingsRequest>().AsTask();
			yield return new HandleTaskServiceWithError(loadCPUTaskService, null, null).GetEnumerator();
			_cpuSettings = loadCPUTaskService.result;
		}

		public uint MaxCPU()
		{
			if (WorldSwitching.IsCustomGame())
			{
				return _cpuSettings.maxMegabotCpu;
			}
			return _cpuSettings.maxRegularCpu;
		}
	}
}
