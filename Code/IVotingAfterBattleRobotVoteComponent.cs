using Simulation.GUI;
using Svelto.ECS;
using UnityEngine;

public interface IVotingAfterBattleRobotVoteComponent
{
	int RobotWidgetID
	{
		get;
	}

	DispatchOnChange<bool> ButtonPressed
	{
		get;
	}

	DispatchOnSet<bool> ButtonEnabled
	{
		get;
	}

	DispatchOnSet<int> CountUpdated
	{
		get;
	}

	DispatchOnChange<string> ThresholdReached
	{
		get;
	}

	GameObject ReceiveVoteParticlePrefab
	{
		get;
	}

	VoteType Type
	{
		get;
	}
}
