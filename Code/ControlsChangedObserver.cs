using System;

internal sealed class ControlsChangedObserver
{
	public event Action onControlsChanged = delegate
	{
	};

	public void ControlsChanged()
	{
		this.onControlsChanged();
	}
}
