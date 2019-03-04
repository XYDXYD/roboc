using Authentication;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
	internal class LobbyPlayerListView : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		[SerializeField]
		protected PlayerListSimulation myTeam;

		[SerializeField]
		protected PlayerListSimulation enemyTeam;

		[SerializeField]
		protected UILabel battleType;

		public Color mePresentColour;

		public Color teamPresentColour;

		public Color teamAbsentColour;

		public Color enemyPresentColour;

		public Color enemyAbsentColour;

		public Color platoonPresentColour;

		public Color platoonAbsentColour;

		protected string battleTypeText = "strBattleType";

		protected string normalModeBattleString = "strNormalModeBattle";

		protected string normalRankedModeBattleString = "strNormalRankedModeBattle";

		protected string brawlEliminationString = "strBrawlEliminationModeBattle";

		protected string brawlPitString = "strBrawlPitBattle";

		protected string basicModeString = "strBasicModeBattle";

		protected string brawlBasicModeString = "strBrawlTeamDeathMatchBattle";

		protected string aiModeString = "strAIModeBattle";

		protected string campaignModeString = "strCampaignModeBattle";

		private uint _myTeamId;

		private bool _isOpen;

		private PresetAvatarMap _presetAvatarMap;

		[Inject]
		internal ProfanityFilter profanityFilter
		{
			get;
			set;
		}

		[Inject]
		internal ILobbyPlayerListPresenter presenter
		{
			get;
			set;
		}

		[Inject]
		internal BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal BattleLoadProgress battleLoadProgress
		{
			private get;
			set;
		}

		protected int maxCPU
		{
			get;
			private set;
		}

		protected int maxRegularCPU
		{
			get;
			private set;
		}

		public LobbyPlayerListView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			battleLoadProgress.PlayerLoadProgressUpdateEvent += OnPlayerLoadUpdate;
		}

		public void OnFrameworkDestroyed()
		{
			battleLoadProgress.PlayerLoadProgressUpdateEvent -= OnPlayerLoadUpdate;
		}

		public bool IsOpen()
		{
			return _isOpen;
		}

		private void Open()
		{
			this.get_gameObject().SetActive(true);
			_isOpen = true;
			UIScrollView componentInChildren = this.GetComponentInChildren<UIScrollView>();
			if (componentInChildren != null)
			{
				UICamera.set_selectedObject(componentInChildren.get_gameObject());
			}
		}

		public void Close()
		{
			if (this.get_gameObject().get_activeSelf())
			{
				this.get_gameObject().SetActive(false);
			}
		}

		public void PlayerJoined(string username, int teamId)
		{
			TaskRunner.get_Instance().Run(MarkAsJoinedOncePopulated(username, teamId));
		}

		public void PlayerLeft(string username)
		{
			if (IsMyTeam(GetTeam(username)))
			{
				myTeam.SetPlayerColour(username, IsMe(username), isAlly: true, IsInMyPlatoon(username), isPresent: true);
			}
			else
			{
				enemyTeam.SetPlayerColour(username, isMe: false, isAlly: false, isPlatoonMate: false, isPresent: true);
			}
		}

		protected bool IsMe(string username)
		{
			return User.Username.Equals(username, StringComparison.OrdinalIgnoreCase);
		}

		protected bool IsMyTeam(int teamId)
		{
			return _myTeamId == teamId;
		}

		protected virtual void InitialisePlayers(List<PlayerDataDependency> players)
		{
			Dictionary<string, Action<Texture2D>> dictionary = new Dictionary<string, Action<Texture2D>>();
			Dictionary<string, Action<Texture2D>> dictionary2 = new Dictionary<string, Action<Texture2D>>();
			int count = players.Count;
			for (int i = 0; i < count; i++)
			{
				PlayerDataDependency playerDataDependency = players[i];
				PlayerListElementSimulation newElement = AddPlayer(playerDataDependency);
				if (playerDataDependency.AvatarInfo.UseCustomAvatar)
				{
					dictionary.Add(playerDataDependency.PlayerName, delegate(Texture2D texture)
					{
						newElement.avatarTexture.set_mainTexture(texture);
					});
				}
				else
				{
					newElement.avatarTexture.set_mainTexture(_presetAvatarMap.GetPresetAvatar(playerDataDependency.AvatarInfo.AvatarId));
				}
				string clanName = playerDataDependency.ClanName;
				if (clanName != null && playerDataDependency.ClanAvatarInfo.UseCustomAvatar)
				{
					if (!dictionary2.ContainsKey(clanName))
					{
						dictionary2.Add(clanName, delegate(Texture2D texture)
						{
							newElement.clanAvatarTexture.set_mainTexture(texture);
						});
					}
					else
					{
						Dictionary<string, Action<Texture2D>> dictionary3;
						string key;
						(dictionary3 = dictionary2)[key = clanName] = (Action<Texture2D>)Delegate.Combine(dictionary3[key], (Action<Texture2D>)delegate(Texture2D texture)
						{
							newElement.clanAvatarTexture.set_mainTexture(texture);
						});
					}
					newElement.clanAvatarTexture.set_enabled(true);
				}
				else
				{
					newElement.clanAvatarTexture.set_enabled(false);
				}
			}
			presenter.LoadAvatars(dictionary, dictionary2, this);
		}

		protected virtual PlayerListElementSimulation AddPlayer(PlayerDataDependency player)
		{
			bool flag = IsMe(player.PlayerName);
			bool flag2 = IsMyTeam(player.TeamId);
			string filteredRobotName = FilterProfanity(flag, player.RobotName);
			bool isMegabot = player.Cpu > maxRegularCPU;
			PlayerListElementSimulation result;
			if (flag)
			{
				result = myTeam.AddPlayer(player, filteredRobotName, maxCPU, isMegabot);
				myTeam.SetPlayerColour(player.PlayerName, isMe: true, isAlly: true, isPlatoonMate: true, isPresent: true);
			}
			else if (flag2)
			{
				result = myTeam.AddPlayer(player, filteredRobotName, maxCPU, isMegabot);
				myTeam.SetPlayerColour(player.PlayerName, isMe: false, isAlly: true, IsInMyPlatoon(player), isPresent: true);
			}
			else
			{
				result = enemyTeam.AddPlayer(player, filteredRobotName, maxCPU, isMegabot);
				enemyTeam.SetPlayerColour(player.PlayerName, isMe: false, isAlly: false, isPlatoonMate: false, isPresent: true);
			}
			return result;
		}

		protected string FilterProfanity(bool isMe, string robotName)
		{
			return (!isMe) ? profanityFilter.FilterString(robotName) : robotName;
		}

		private bool IsInMyPlatoon(string userName)
		{
			return battlePlayers.MyPlatoonId != 255 && battlePlayers.GetPlatoonId(userName) == battlePlayers.MyPlatoonId;
		}

		protected bool IsInMyPlatoon(PlayerDataDependency player)
		{
			return battlePlayers.MyPlatoonId != 255 && player.PlatoonId == battlePlayers.MyPlatoonId;
		}

		private IEnumerator Start()
		{
			UIScrollView scrollView = this.GetComponentInChildren<UIScrollView>();
			if (scrollView != null)
			{
				scrollView.ResetPosition();
			}
			_presetAvatarMap = (PresetAvatarMap)Resources.Load("PresetAvatarMap");
			myTeam.RegisterView(this);
			enemyTeam.RegisterView(this);
			SetBattleType(WorldSwitching.IsRanked(), WorldSwitching.IsBrawl(), WorldSwitching.IsCustomGame(), WorldSwitching.GetGameModeType());
			_myTeamId = GetMyTeamId();
			while (!profanityFilter.IsReady())
			{
				yield return null;
			}
			while (presenter == null)
			{
				yield return null;
			}
			List<PlayerDataDependency> expectedPlayersList = battlePlayers.GetExpectedPlayersList();
			maxCPU = (int)presenter.MaxCPU();
			maxRegularCPU = presenter.MaxRegularCPU;
			InitialisePlayers(expectedPlayersList);
			presenter.AddView(this);
			Open();
		}

		private void OnDestroy()
		{
			if (presenter != null)
			{
				presenter.RemoveView(this);
			}
		}

		private uint GetMyTeamId()
		{
			return battlePlayers.MyTeam;
		}

		private IEnumerator MarkAsJoinedOncePopulated(string username, int teamId)
		{
			while (!_isOpen)
			{
				yield return null;
			}
			if (IsMyTeam(teamId))
			{
				myTeam.SetPlayerColour(username, IsMe(username), isAlly: true, IsInMyPlatoon(username), isPresent: true);
			}
			else
			{
				enemyTeam.SetPlayerColour(username, isMe: false, isAlly: false, isPlatoonMate: false, isPresent: true);
			}
		}

		private int GetTeam(string username)
		{
			return (int)battlePlayers.GetTeamId(username);
		}

		private void SetBattleType(bool isRanked, bool isBrawl, bool isCustomGame, GameModeType gameModeType)
		{
			if (!(battleType == null))
			{
				string str = string.Empty;
				switch (gameModeType)
				{
				case GameModeType.Normal:
					str = ((!isRanked) ? StringTableBase<StringTable>.Instance.GetString(normalModeBattleString) : StringTableBase<StringTable>.Instance.GetString(normalRankedModeBattleString));
					break;
				case GameModeType.Campaign:
					str = StringTableBase<StringTable>.Instance.GetString(campaignModeString);
					break;
				case GameModeType.PraticeMode:
					str = StringTableBase<StringTable>.Instance.GetString(aiModeString);
					break;
				case GameModeType.TeamDeathmatch:
					str = ((!isBrawl) ? StringTableBase<StringTable>.Instance.GetString(basicModeString) : StringTableBase<StringTable>.Instance.GetString(brawlBasicModeString));
					break;
				case GameModeType.SuddenDeath:
					str = ((!isBrawl) ? StringTableBase<StringTable>.Instance.GetString("strElimination") : StringTableBase<StringTable>.Instance.GetString(brawlEliminationString));
					break;
				case GameModeType.Pit:
					str = ((!isBrawl) ? StringTableBase<StringTable>.Instance.GetString("strPitBattle") : StringTableBase<StringTable>.Instance.GetString(brawlPitString));
					break;
				}
				battleType.set_text(StringTableBase<StringTable>.Instance.GetString(battleTypeText) + " : " + str);
			}
		}

		private void OnPlayerLoadUpdate(string playerName, float progress)
		{
			if (IsMyTeam(GetTeam(playerName)))
			{
				myTeam.SetLoadProgress(playerName, progress);
			}
			else
			{
				enemyTeam.SetLoadProgress(playerName, progress);
			}
		}
	}
}
