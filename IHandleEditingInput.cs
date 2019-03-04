using Svelto.ES.Legacy;

internal interface IHandleEditingInput : IInputComponent, IComponent
{
	void HandleEditingInput(InputEditingData data);
}
