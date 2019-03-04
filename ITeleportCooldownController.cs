using System;

internal interface ITeleportCooldownController
{
	event Action OnCooldownRestart;

	event Action OnTeleportAttemptStart;

	event Action OnTeleportAttemptEnd;

	event Action OnCooldownDeactivated;

	float GetCooldownTime();

	bool TeleportIsAllowed();

	void TeleportAttemptStarted();

	void TeleportAttemptEnded();
}
