using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class PressStartToJoinExample_Assigner : MonoBehaviour
	{
		private class PlayerMap
		{
			public int rewiredPlayerId;

			public int gamePlayerId;

			public PlayerMap(int rewiredPlayerId, int gamePlayerId)
			{
				this.rewiredPlayerId = rewiredPlayerId;
				this.gamePlayerId = gamePlayerId;
			}
		}

		private static PressStartToJoinExample_Assigner instance;

		public int maxPlayers = 4;

		private List<PlayerMap> playerMap;

		private int gamePlayerIdCounter;

		public PressStartToJoinExample_Assigner()
			: this()
		{
		}

		public static Player GetRewiredPlayer(int gamePlayerId)
		{
			if (!ReInput.get_isReady())
			{
				return null;
			}
			if (instance == null)
			{
				Debug.LogError((object)"Not initialized. Do you have a PressStartToJoinPlayerSelector in your scehe?");
				return null;
			}
			for (int i = 0; i < instance.playerMap.Count; i++)
			{
				if (instance.playerMap[i].gamePlayerId == gamePlayerId)
				{
					return ReInput.get_players().GetPlayer(instance.playerMap[i].rewiredPlayerId);
				}
			}
			return null;
		}

		private void Awake()
		{
			playerMap = new List<PlayerMap>();
			instance = this;
		}

		private void Update()
		{
			for (int i = 0; i < ReInput.get_players().get_playerCount(); i++)
			{
				if (ReInput.get_players().GetPlayer(i).GetButtonDown("JoinGame"))
				{
					AssignNextPlayer(i);
				}
			}
		}

		private void AssignNextPlayer(int rewiredPlayerId)
		{
			if (playerMap.Count >= maxPlayers)
			{
				Debug.LogError((object)"Max player limit already reached!");
				return;
			}
			int nextGamePlayerId = GetNextGamePlayerId();
			playerMap.Add(new PlayerMap(rewiredPlayerId, nextGamePlayerId));
			Player player = ReInput.get_players().GetPlayer(rewiredPlayerId);
			player.controllers.maps.SetMapsEnabled(false, "Assignment");
			player.controllers.maps.SetMapsEnabled(true, "Default");
			Debug.Log((object)("Added Rewired Player id " + rewiredPlayerId + " to game player " + nextGamePlayerId));
		}

		private int GetNextGamePlayerId()
		{
			return gamePlayerIdCounter++;
		}
	}
}
