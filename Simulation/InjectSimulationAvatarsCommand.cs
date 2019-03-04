using Battle;
using Svelto.Command;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class InjectSimulationAvatarsCommand : ICommand
	{
		protected Texture2D AvatarAtlas;

		protected IDictionary<string, Rect> AvatarAtlasRects;

		protected Texture2D ClanAvatarAtlas;

		protected IDictionary<string, Rect> ClanAvatarAtlasRects;

		[Inject]
		internal PlayerListHudPresenter PlayerListHudPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal HUDPlayerIDManager HudPlayerIdManager
		{
			private get;
			set;
		}

		[Inject]
		internal ILobbyPlayerListPresenter LobbyPlayerListPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal BattleStatsPresenter BattleStatsPresenter
		{
			private get;
			set;
		}

		public ICommand Inject(Texture2D avatarAtlas, IDictionary<string, Rect> avatarAtlasRects, Texture2D clanAvatarAtlas, IDictionary<string, Rect> clanAvatarAtlasRects)
		{
			AvatarAtlas = avatarAtlas;
			AvatarAtlasRects = avatarAtlasRects;
			ClanAvatarAtlas = clanAvatarAtlas;
			ClanAvatarAtlasRects = clanAvatarAtlasRects;
			return this;
		}

		public virtual void Execute()
		{
			PlayerListHudPresenter.InjectMultiplayerAvatars(AvatarAtlas, AvatarAtlasRects, ClanAvatarAtlas, ClanAvatarAtlasRects);
			HudPlayerIdManager.InjectMultiplayerAvatars(AvatarAtlas, AvatarAtlasRects, ClanAvatarAtlas, ClanAvatarAtlasRects);
			BattleStatsPresenter.InjectMultiplayerAvatars(AvatarAtlas, AvatarAtlasRects, ClanAvatarAtlas, ClanAvatarAtlasRects);
		}
	}
}
