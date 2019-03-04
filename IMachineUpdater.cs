internal interface IMachineUpdater
{
	float GetCurrentTime();

	void SetPing(float ping);

	void Tick(float deltaTime);

	void OnEnable();
}
