using Svelto.ES.Legacy;

internal interface IHandleMouseButtons : IInputComponent, IComponent
{
	void HandleButton(InputMouseData data);
}
