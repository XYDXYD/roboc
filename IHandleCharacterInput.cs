using Svelto.ES.Legacy;

internal interface IHandleCharacterInput : IInputComponent, IComponent
{
	void HandleCharacterInput(InputCharacterData data);
}
