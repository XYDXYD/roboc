using Svelto.Context;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class BattleStatsPlayerLayout : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		public GameObject ListHeaderGO_TDM;

		public GameObject ListHeaderGO_Pit;

		public GameObject ListHeaderGO_BA;

		public GameObject PlayerListItemTemplateGO_TDM;

		public GameObject PlayerListItemTemplateGO_BA;

		public GameObject PlayerListItemTemplateGO_Pit;

		public GameObject PlayerListItemTemplate;

		public UITable PlayersListContainer;

		public GameObject okButton;

		public GameObject hintTextLabel;

		public UIWidget MiddleUIWidget;

		public int MiddleBackgroundPadding = 10;

		protected FasterList<BattleStatsPlayerWidget> _widgetsList = new FasterList<BattleStatsPlayerWidget>();

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal BattleStatsPresenter battleStatsPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public BattleStatsPlayerLayout()
			: this()
		{
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_widgetsList.FastClear();
		}

		void IInitialize.OnDependenciesInjected()
		{
			battleStatsPresenter.SetView(this);
			PlayerListItemTemplate = PlayerListItemTemplateGO_TDM;
		}

		public void SetBattleArenaStyle()
		{
			ListHeaderGO_BA.SetActive(true);
			ListHeaderGO_Pit.SetActive(false);
			ListHeaderGO_TDM.SetActive(false);
			PlayerListItemTemplate = PlayerListItemTemplateGO_BA;
			TaskRunner.get_Instance().Run(SetAvatarAtlases(PlayerListItemTemplate));
		}

		public void SetPitHeaderStyle()
		{
			ListHeaderGO_BA.SetActive(false);
			ListHeaderGO_Pit.SetActive(true);
			ListHeaderGO_TDM.SetActive(false);
			PlayerListItemTemplate = PlayerListItemTemplateGO_Pit;
			TaskRunner.get_Instance().Run(SetAvatarAtlases(PlayerListItemTemplate));
		}

		public void SetTDMHeaderStyle()
		{
			ListHeaderGO_BA.SetActive(false);
			ListHeaderGO_Pit.SetActive(false);
			ListHeaderGO_TDM.SetActive(true);
			PlayerListItemTemplate = PlayerListItemTemplateGO_TDM;
			TaskRunner.get_Instance().Run(SetAvatarAtlases(PlayerListItemTemplate));
		}

		public void AddWidget()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectFactory.Build(PlayerListItemTemplate);
			val.get_transform().set_parent(PlayersListContainer.get_transform());
			val.get_transform().set_localScale(Vector3.get_one());
			BattleStatsPlayerWidget component = val.GetComponent<BattleStatsPlayerWidget>();
			_widgetsList.Add(component);
			val.SetActive(true);
			component.bGSprite.UpdateAnchors();
			PlayersListContainer.set_repositionNow(true);
		}

		private IEnumerator SetAvatarAtlases(GameObject widget)
		{
			while (battleStatsPresenter.AvatarAtlasTexture == null)
			{
				yield return null;
			}
			BattleStatsPlayerWidget battleStatsWidget = widget.GetComponent<BattleStatsPlayerWidget>();
			battleStatsWidget.AvatarTexture.set_mainTexture(battleStatsPresenter.AvatarAtlasTexture);
			battleStatsWidget.ClanAvatarTexture.set_mainTexture(battleStatsPresenter.ClanAvatarAtlasTexture);
			FasterListEnumerator<BattleStatsPlayerWidget> enumerator = _widgetsList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BattleStatsPlayerWidget current = enumerator.get_Current();
					current.AvatarTexture.set_mainTexture(battleStatsPresenter.AvatarAtlasTexture);
					current.ClanAvatarTexture.set_mainTexture(battleStatsPresenter.ClanAvatarAtlasTexture);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		public void SetOKButtonEnabled(bool enabled)
		{
			okButton.SetActive(enabled);
		}

		public void UpdatePlayerWidget(int index, bool isMe, bool isMyTeam, string playerName, string displayName, bool isFriend, bool requestSent, bool friendFunctionsEnabled, string clanTag, bool isSinglePlayerMode)
		{
			_widgetsList.get_Item(index).SetBGColor(isMe, isMyTeam, index);
			_widgetsList.get_Item(index).SetPlayerNameAndClan(playerName, displayName, clanTag);
			_widgetsList.get_Item(index).SetFriendIcon(isMe, isFriend, requestSent, friendFunctionsEnabled, isSinglePlayerMode);
			TaskRunner.get_Instance().Run(SetAvatars(index, playerName));
		}

		private IEnumerator SetAvatars(int index, string playerName)
		{
			while (battleStatsPresenter.AvatarAtlasRects == null && battleStatsPresenter.ClanAvatarAtlasRects == null)
			{
				yield return null;
			}
			_widgetsList.get_Item(index).AvatarTexture.set_uvRect(battleStatsPresenter.AvatarAtlasRects[playerName]);
			if (battleStatsPresenter.ClanAvatarAtlasRects.TryGetValue(playerName, out Rect clanAvatarUvRect))
			{
				_widgetsList.get_Item(index).ClanAvatarTexture.get_gameObject().SetActive(true);
				_widgetsList.get_Item(index).ClanAvatarTexture.set_uvRect(clanAvatarUvRect);
			}
			else
			{
				_widgetsList.get_Item(index).ClanAvatarTexture.get_gameObject().SetActive(false);
			}
		}

		public void UpdateFriendStatus(int index, bool setAsFriend, bool setAsRequested, bool friendFunctionsEnabled)
		{
			_widgetsList.get_Item(index).SetFriendIcon(isMe: false, setAsFriend, setAsRequested, friendFunctionsEnabled, WorldSwitching.GetGameModeType() == GameModeType.PraticeMode);
		}

		public void SetTeamColour(int index, bool isMe, bool isMyTeam)
		{
			_widgetsList.get_Item(index).SetBGColor(isMe, isMyTeam, index);
		}

		public void SetStatsLabel(int index, InGameStatId statId, uint amount)
		{
			_widgetsList.get_Item(index).SetStatsLabel(statId, amount);
		}

		public void GameEnded()
		{
			hintTextLabel.SetActive(true);
		}

		public bool IsVisible()
		{
			return this.get_gameObject().get_activeInHierarchy();
		}

		public void Show()
		{
			hintTextLabel.SetActive(false);
			this.get_gameObject().SetActive(true);
			PlayersListContainer.set_repositionNow(true);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)WaitThenRepositionTable);
		}

		private IEnumerator WaitThenRepositionTable()
		{
			yield return null;
			PlayersListContainer.Reposition();
			int middleBottomAnchor = -(_widgetsList.get_Count() * _widgetsList.get_Item(0).uiWidget.get_height()) - MiddleBackgroundPadding;
			MiddleUIWidget.bottomAnchor.absolute = middleBottomAnchor;
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
