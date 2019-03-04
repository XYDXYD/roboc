using Svelto.ES.Legacy;
using System;

internal sealed class BattleStatsInputComponent : IBattleStatsInputComponent, IHandleCharacterInput, IInputComponent, IComponent
{
	public event Action<InputCharacterData> OnInputData = delegate
	{
	};

	public void HandleCharacterInput(InputCharacterData data)
	{
		this.OnInputData(data);
	}
}
