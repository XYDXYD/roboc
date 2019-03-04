using Svelto.ES.Legacy;
using System;

internal class BattleStatsModeComponent : IBattleStatsModeComponent, IInputComponent, IComponent
{
	public event Action<BSMode> OnSwitch = delegate
	{
	};

	public void SwitchMode(BSMode newMode)
	{
		this.OnSwitch(newMode);
	}
}
