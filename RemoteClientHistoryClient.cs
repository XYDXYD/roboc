using System.Collections.Generic;
using UnityEngine;

internal sealed class RemoteClientHistoryClient
{
	public Dictionary<int, PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame>> machineHistory = new Dictionary<int, PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame>>();

	public Dictionary<int, PlayerHistoryBuffer<PlayerInputHistoryFrame>> inputHistory = new Dictionary<int, PlayerHistoryBuffer<PlayerInputHistoryFrame>>();

	public void AddMachine(int playerId, int machineId, Rigidbody rbData)
	{
		if (!machineHistory.ContainsKey(machineId))
		{
			PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> playerHistoryBuffer = new PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame>(20);
			playerHistoryBuffer.Flush(new PlayerMachineMotionHistoryFrame(rbData, null));
			machineHistory.Add(machineId, playerHistoryBuffer);
		}
		else
		{
			machineHistory[machineId].Flush(new PlayerMachineMotionHistoryFrame(rbData, null));
		}
		if (!inputHistory.ContainsKey(playerId))
		{
			inputHistory.Add(playerId, new PlayerHistoryBuffer<PlayerInputHistoryFrame>(20));
		}
	}

	public void RemoveMachine(int machineId)
	{
		if (machineHistory.ContainsKey(machineId))
		{
			machineHistory.Remove(machineId);
		}
	}

	public void RemovePlayer(int playerId)
	{
		if (inputHistory.ContainsKey(playerId))
		{
			inputHistory.Remove(playerId);
		}
	}

	public PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> GetMachineHistory(int machineId)
	{
		if (machineHistory.ContainsKey(machineId))
		{
			return machineHistory[machineId];
		}
		return null;
	}

	public PlayerHistoryBuffer<PlayerInputHistoryFrame> GetInputHistory(int playerId)
	{
		if (inputHistory.ContainsKey(playerId))
		{
			return inputHistory[playerId];
		}
		return null;
	}
}
