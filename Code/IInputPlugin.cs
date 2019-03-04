using Svelto.ES.Legacy;

internal interface IInputPlugin : IComponent
{
	void Execute();

	void RegisterComponent(IInputComponent component);

	void UnregisterComponent(IInputComponent component);
}
