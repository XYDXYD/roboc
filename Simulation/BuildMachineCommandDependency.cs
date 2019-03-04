namespace Simulation
{
	internal class BuildMachineCommandDependency
	{
		public readonly int playerId;

		public readonly string playerName;

		public readonly int teamId;

		public readonly bool isAI;

		public readonly string spawnEffect;

		public readonly string deathEffect;

		public BuildMachineCommandDependency(int pPlayerId, string pPlayerName, int pTeamId, bool pIsAI, string pSpawnEffect, string pDeathEffect)
		{
			playerId = pPlayerId;
			playerName = pPlayerName;
			teamId = pTeamId;
			isAI = pIsAI;
			spawnEffect = pSpawnEffect;
			deathEffect = pDeathEffect;
		}
	}
}
