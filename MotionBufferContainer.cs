using System.Collections.Generic;

internal sealed class MotionBufferContainer
{
	private Dictionary<int, PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame>> _motionBuffers = new Dictionary<int, PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame>>();

	public void RegisterMachineHistory(int player, PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> motionBuffer)
	{
		_motionBuffers.Add(player, motionBuffer);
	}

	public void UnregisterMachineHistory(int player)
	{
		_motionBuffers.Remove(player);
	}

	public PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> GetMachineHistory(int player)
	{
		return _motionBuffers[player];
	}
}
