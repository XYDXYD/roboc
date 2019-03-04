using Svelto.ES.Legacy;
using System;

internal interface IBattleStatsModeComponent : IInputComponent, IComponent
{
	event Action<BSMode> OnSwitch;

	void SwitchMode(BSMode newMode);
}
