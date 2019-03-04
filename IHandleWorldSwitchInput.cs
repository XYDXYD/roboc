using Svelto.ES.Legacy;

internal interface IHandleWorldSwitchInput : IInputComponent, IComponent
{
	void HandleWorldSwitchInput(bool buttonDown);
}
