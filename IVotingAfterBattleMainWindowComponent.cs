using Svelto.ECS;

public interface IVotingAfterBattleMainWindowComponent
{
	DispatchOnChange<bool> confirmButtonPressed
	{
		get;
	}

	DispatchOnChange<bool> active
	{
		get;
	}

	DispatchOnSet<bool> victory
	{
		get;
	}

	DispatchOnSet<int> numWidgetShowAnimationsEnded
	{
		get;
	}

	int numPlayersOnPedestal
	{
		set;
	}
}
