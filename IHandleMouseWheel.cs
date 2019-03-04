using Svelto.ES.Legacy;

internal interface IHandleMouseWheel : IInputComponent, IComponent
{
	void HandleWheel(float val);
}
