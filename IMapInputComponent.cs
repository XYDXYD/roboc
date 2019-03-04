using Svelto.ES.Legacy;
using System;

internal interface IMapInputComponent : IHandleCharacterInput, IInputComponent, IComponent
{
	event Action<InputCharacterData> OnInputData;
}
