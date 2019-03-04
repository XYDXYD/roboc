using Svelto.ES.Legacy;
using System;

internal interface IMapModeComponent : IInputComponent, IComponent
{
	event Action<MMode> OnSwitch;

	void SwitchMode(MMode newMode);
}
