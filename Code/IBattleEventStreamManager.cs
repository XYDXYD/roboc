using System;

internal interface IBattleEventStreamManager
{
	event Action<int> OnPlayerSpawnedIn;

	event Action<int> OnPlayerSpawnedOut;

	event Action<int, int> OnPlayerWasKilledBy;

	void PlayerWasKilledBy(int player, int shooter);
}
