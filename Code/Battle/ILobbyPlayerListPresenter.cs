using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
	internal interface ILobbyPlayerListPresenter
	{
		int MaxRegularCPU
		{
			get;
		}

		void AddView(LobbyPlayerListView lobbyPlayerListView);

		void RemoveView(LobbyPlayerListView lobbyPlayerListView);

		void LoadAvatars(Dictionary<string, Action<Texture2D>> playerAvatarTextures, Dictionary<string, Action<Texture2D>> clanAvatarTextures, LobbyPlayerListView view);

		uint MaxCPU();
	}
}
