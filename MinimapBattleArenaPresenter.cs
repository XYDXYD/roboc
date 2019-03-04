internal class MinimapBattleArenaPresenter : MinimapPresenter
{
	protected override void OnPlayerRespawnedIn(SpawnInParametersPlayer spawnInParameters)
	{
		if (spawnInParameters.isMe)
		{
			_view.RegisterMainPlayer();
		}
		else
		{
			_activePlayers.Add(spawnInParameters.playerId);
		}
	}

	protected override void OnMachineDestroyed(int playerId, int machineId, bool isMe)
	{
		if (isMe)
		{
			_view.UnregisterMainPlayer();
		}
		else if (_machinesInfo.ContainsKey(machineId))
		{
			MinimapPlayerInfo playerInfo = _machinesInfo[machineId];
			SetVisible(playerInfo, visible: false);
			_activePlayers.Remove(playerId);
		}
	}
}
