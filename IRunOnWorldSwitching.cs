using Svelto.ES.Legacy;

internal interface IRunOnWorldSwitching : IComponent
{
	bool FadeIn
	{
		get;
	}

	int Priority
	{
		get;
	}

	float Duration
	{
		get;
	}

	void Execute(WorldSwitchMode currentMode);
}
