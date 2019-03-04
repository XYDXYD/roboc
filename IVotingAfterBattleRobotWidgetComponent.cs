using Svelto.ECS;
using UnityEngine;

public interface IVotingAfterBattleRobotWidgetComponent
{
	DispatchOnChange<Texture> RobotTexture
	{
		get;
	}

	DispatchOnChange<string> PlayerName
	{
		get;
	}

	DispatchOnChange<string> DisplayName
	{
		get;
	}

	DispatchOnChange<int> NumPlayersOnPedestal
	{
		get;
	}

	DispatchOnSet<bool> IsMe
	{
		get;
	}

	DispatchOnSet<bool> IsMyTeam
	{
		get;
	}

	DispatchOnSet<bool> Active
	{
		get;
	}

	DispatchOnChange<bool> IsHover
	{
		get;
	}

	DispatchOnSet<string> ThresholdUpdated
	{
		get;
	}

	DispatchOnChange<bool> ShowAnimationEnded
	{
		get;
	}

	int PedestalPosition
	{
		get;
	}
}
