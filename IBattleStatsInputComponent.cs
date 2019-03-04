using Svelto.ES.Legacy;
using System;

internal interface IBattleStatsInputComponent : IHandleCharacterInput, IInputComponent, IComponent
{
	event Action<InputCharacterData> OnInputData;
}
