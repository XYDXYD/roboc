using Battle;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections;
using UnityEngine;

public class RobotNameWriterPresenter
{
	private RobotNameWriter _robotNameView;

	[Inject]
	internal MachineRootContainer machineRootContainer
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

	[Inject]
	internal PlayerMachinesContainer playerMachinesContainer
	{
		private get;
		set;
	}

	[Inject]
	internal PlayerNamesContainer playerNamesContainer
	{
		private get;
		set;
	}

	[Inject]
	internal ProfanityFilter filter
	{
		private get;
		set;
	}

	[Inject]
	internal BattlePlayers battlePlayers
	{
		private get;
		set;
	}

	public void RegisterView(RobotNameWriter robotNameView)
	{
		_robotNameView = robotNameView;
	}

	public IEnumerator GetRobotNameFromBoard()
	{
		GameObject board = GameUtility.GetMachineBoard(_robotNameView.get_transform());
		while (!machineRootContainer.IsMachineRegistered(TargetType.Player, board))
		{
			yield return null;
		}
		while (!filter.IsReady())
		{
			yield return null;
		}
		int machineId = machineRootContainer.GetMachineIdFromRoot(TargetType.Player, board);
		int ownerId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
		string playerName = playerNamesContainer.GetPlayerName(ownerId);
		string robotName = battlePlayers.GetPlayerRobotName(playerName);
		if (!playerTeamsContainer.IsMe(TargetType.Player, ownerId))
		{
			robotName = filter.FilterString(robotName);
		}
		_robotNameView.SetLabelName(robotName);
	}
}
