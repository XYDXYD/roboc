using Battle;
using Svelto.Factories;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal class PlayerListSimulation : MonoBehaviour
{
	public class DefaultPlayerComparer : IComparer<PlayerDataDependency>
	{
		public int Compare(PlayerDataDependency x, PlayerDataDependency y)
		{
			int num = x.PlatoonId.CompareTo(y.PlatoonId);
			return (num != 0) ? num : string.Compare(x.PlayerName, y.PlayerName, StringComparison.OrdinalIgnoreCase);
		}
	}

	[SerializeField]
	private PlayerListElementSimulation playerListElement;

	private int _previousScreenHeight;

	private SortedList<PlayerDataDependency, PlayerListElementSimulation> _playerList = new SortedList<PlayerDataDependency, PlayerListElementSimulation>(new DefaultPlayerComparer());

	private LobbyPlayerListView _lobbyPlayerListView;

	private PartyColours _partyColours;

	private UITable _grid;

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	public PlayerListSimulation()
		: this()
	{
	}

	private void Awake()
	{
		_partyColours = (Resources.Load("PartyColours") as PartyColours);
		if (_partyColours == null)
		{
			throw new Exception("Failed to load party colours");
		}
		_grid = playerListElement.get_transform().get_parent().GetComponent<UITable>();
		_previousScreenHeight = Screen.get_height();
	}

	internal PlayerListElementSimulation AddPlayer(PlayerDataDependency player, string filteredRobotName, int maxCPU, bool isMegabot)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = gameObjectFactory.Build(playerListElement.get_gameObject());
		val.get_transform().set_parent(playerListElement.get_transform().get_parent());
		val.get_transform().set_localScale(Vector3.get_one());
		val.SetActive(true);
		PlayerListElementSimulation component = val.GetComponent<PlayerListElementSimulation>();
		UISprite partySprite = component.partySprite;
		if (partySprite != null)
		{
			if (player.PlatoonId == 255)
			{
				partySprite.set_enabled(false);
			}
			else
			{
				partySprite.set_enabled(true);
				partySprite.set_color(_partyColours.GetColour(player.PlatoonId));
			}
		}
		else if (player.PlatoonId != 255)
		{
			Console.LogError("party detected but UI has no party icon");
		}
		component.SetPlayer(player, filteredRobotName, maxCPU, isMegabot);
		_playerList.Add(player, component);
		OrderSiblings();
		if (_playerList.Count < 6)
		{
			GameObject gameObject = this.get_transform().get_parent().get_gameObject();
			UIScrollScrollView component2 = gameObject.GetComponent<UIScrollScrollView>();
			if (component2 != null)
			{
				component2.set_enabled(false);
				component2.scrollView.set_enabled(false);
				UIProgressBar verticalScrollBar = component2.scrollView.verticalScrollBar;
				UISliderMouseWheelScroller component3 = verticalScrollBar.GetComponent<UISliderMouseWheelScroller>();
				component3.set_enabled(false);
			}
		}
		else
		{
			GameObject gameObject2 = this.get_transform().get_parent().get_gameObject();
			UIScrollScrollView component4 = gameObject2.GetComponent<UIScrollScrollView>();
			if (component4 != null)
			{
				component4.set_enabled(true);
				component4.scrollView.set_enabled(true);
				UIProgressBar verticalScrollBar2 = component4.scrollView.verticalScrollBar;
				UISliderMouseWheelScroller component5 = verticalScrollBar2.GetComponent<UISliderMouseWheelScroller>();
				component5.set_enabled(true);
			}
		}
		return component;
	}

	private void OrderSiblings()
	{
		int num = 0;
		foreach (KeyValuePair<PlayerDataDependency, PlayerListElementSimulation> player in _playerList)
		{
			player.Value.get_transform().SetSiblingIndex(num);
			num++;
		}
		_grid.set_repositionNow(true);
	}

	public void Update()
	{
		if (_previousScreenHeight != Screen.get_height())
		{
			_previousScreenHeight = Screen.get_height();
			_grid.set_repositionNow(true);
		}
	}

	public void SetPlayerColour(string playerName, bool isMe, bool isAlly, bool isPlatoonMate, bool isPresent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Color playerColour = GetPlayerColour(isMe, isAlly, isPlatoonMate, isPresent);
		foreach (KeyValuePair<PlayerDataDependency, PlayerListElementSimulation> player in _playerList)
		{
			if (player.Key.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase))
			{
				player.Value.SetColour(playerColour);
			}
		}
	}

	private Color GetPlayerColour(bool isMe, bool isAlly, bool isPlatoonMate, bool isPresent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (isMe)
		{
			return _lobbyPlayerListView.mePresentColour;
		}
		if (isPresent)
		{
			if (isAlly)
			{
				return (!isPlatoonMate) ? _lobbyPlayerListView.teamPresentColour : _lobbyPlayerListView.platoonPresentColour;
			}
			return _lobbyPlayerListView.enemyPresentColour;
		}
		if (isAlly)
		{
			return (!isPlatoonMate) ? _lobbyPlayerListView.teamAbsentColour : _lobbyPlayerListView.platoonAbsentColour;
		}
		return _lobbyPlayerListView.enemyAbsentColour;
	}

	internal void RegisterView(LobbyPlayerListView lobbyPlayerListView)
	{
		_lobbyPlayerListView = lobbyPlayerListView;
	}

	internal void SetLoadProgress(string playerName, float progress)
	{
		foreach (KeyValuePair<PlayerDataDependency, PlayerListElementSimulation> player in _playerList)
		{
			if (player.Key.PlayerName == playerName)
			{
				player.Value.SetLoadProgress(progress);
				break;
			}
		}
	}
}
