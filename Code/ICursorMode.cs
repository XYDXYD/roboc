using System;

internal interface ICursorMode
{
	Mode currentMode
	{
		get;
	}

	event Action<Mode> OnSwitch;

	void ForceFree();

	void ReleaseForceFree();

	void PushFreeMode();

	void PushNoKeyInputMode();

	void PopFreeMode();

	void Reset();

	void Refresh();
}
