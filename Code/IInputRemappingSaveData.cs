using Rewired;

internal interface IInputRemappingSaveData
{
	void Save();

	void Load();

	bool HasSavedSettings();

	void Delete();

	void Revert();

	void ReplaceElementMap(int controllerId, ControllerType controllerType, ControllerMap currentControllerMap, ActionElementMap loadedElementMap);
}
