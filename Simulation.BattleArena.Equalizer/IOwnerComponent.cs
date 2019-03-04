namespace Simulation.BattleArena.Equalizer
{
	internal interface IOwnerComponent
	{
		int machineId
		{
			get;
			set;
		}

		int playerId
		{
			get;
			set;
		}
	}
}
