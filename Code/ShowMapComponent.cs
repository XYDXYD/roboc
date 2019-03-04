using Svelto.ES.Legacy;
using System;

internal sealed class ShowMapComponent : IMapInputComponent, IHandleCharacterInput, IInputComponent, IComponent
{
	public event Action<InputCharacterData> OnInputData;

	public void HandleCharacterInput(InputCharacterData data)
	{
		this.OnInputData(data);
	}
}
