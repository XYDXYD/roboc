using Svelto.ES.Legacy;
using System;

internal class MapModeComponent : IMapModeComponent, IInputComponent, IComponent
{
	private MMode _currentMode;

	public event Action<MMode> OnSwitch = delegate
	{
	};

	public void SwitchMode(MMode newMode)
	{
		this.OnSwitch(newMode);
	}
}
