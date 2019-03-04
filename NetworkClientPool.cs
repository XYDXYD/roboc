using Network;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility;

internal sealed class NetworkClientPool
{
	private const int maxPlayers = 1024;

	private Dictionary<object, int> playerToId = new Dictionary<object, int>();

	private Dictionary<int, object> idToPlayer = new Dictionary<int, object>();

	private int nextTemporaryId = 100;

	private List<int> currentPlayers = new List<int>();

	public void RegisterPlayer<T>(T player)
	{
		if (playerToId.ContainsKey(player))
		{
			RemoteLogger.Error("Registered Player more than once", string.Empty, null);
		}
		else
		{
			playerToId.Add(player, nextTemporaryId);
			idToPlayer.Add(nextTemporaryId, player);
			currentPlayers.Add(nextTemporaryId);
			nextTemporaryId++;
		}
		NetworkConnection networkConnection = player as NetworkConnection;
		Console.Log("NetworkConnection: " + player.GetHashCode() + " senderID (temporary): " + (nextTemporaryId - 1) + " connID: " + ((networkConnection == null) ? "NA" : networkConnection.connectionId.ToString()));
	}

	public void UnregisterPlayer<T>(T player)
	{
		if (!playerToId.ContainsKey(player))
		{
			RemoteLogger.Error("Unregistering Player more than once", string.Empty, null);
			return;
		}
		int num = playerToId[player];
		playerToId.Remove(player);
		idToPlayer.Remove(num);
		currentPlayers.Remove(num);
	}

	public bool TryGetPlayerId<T>(T player, out int playerId)
	{
		return playerToId.TryGetValue(player, out playerId);
	}

	public void UpdatePlayerId(int oldId, int newId)
	{
		object obj = idToPlayer[oldId];
		idToPlayer.Remove(oldId);
		idToPlayer[newId] = obj;
		playerToId[obj] = newId;
		int index = currentPlayers.IndexOf(oldId);
		currentPlayers[index] = newId;
	}

	public ReadOnlyCollection<int> GetPlayerIds()
	{
		return currentPlayers.AsReadOnly();
	}

	public bool TryGetPlayer<T>(int playerId, out T player)
	{
		if (idToPlayer.TryGetValue(playerId, out object value))
		{
			player = (T)value;
			return true;
		}
		player = default(T);
		return false;
	}
}
