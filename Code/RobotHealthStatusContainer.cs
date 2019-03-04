using Simulation.Hardware.Weapons;
using System.Collections.Generic;

internal class RobotHealthStatusContainer
{
	private Dictionary<TargetType, Dictionary<int, RobotHealthStatus.RobotStatus>> _roboStatus = new Dictionary<TargetType, Dictionary<int, RobotHealthStatus.RobotStatus>>();

	public void SetRoboStatus(TargetType type, int machineId, RobotHealthStatus.RobotStatus status)
	{
		if (!_roboStatus.ContainsKey(type))
		{
			_roboStatus[type] = new Dictionary<int, RobotHealthStatus.RobotStatus>();
		}
		_roboStatus[type][machineId] = status;
	}

	public RobotHealthStatus.RobotStatus GetRoboStatus(TargetType type, int machineId)
	{
		return _roboStatus[type][machineId];
	}
}
