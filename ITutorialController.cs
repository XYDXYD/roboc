using System.Collections;

internal interface ITutorialController
{
	void SetDisplay(TutorialScreenBase display);

	void ShowTutorialScreenAndActivateFSM();

	void HideTutorialScreen();

	bool IsActive();

	IEnumerator RequestTutorialStatusData();

	bool TutorialInProgress();
}
