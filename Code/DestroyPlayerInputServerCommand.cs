using Svelto.Command;
using Svelto.Command.Dispatcher;
using UnityEngine;

internal sealed class DestroyPlayerInputServerCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private PlayerIdDependency _playerIdDependency;

	public IDispatchableCommand Inject(object dependency)
	{
		_playerIdDependency = (dependency as PlayerIdDependency);
		return this;
	}

	public void Execute()
	{
		GameObject val = GameObject.Find("PlayerInputListener_" + _playerIdDependency.owner);
		if (val != null)
		{
			Object.Destroy(val.get_gameObject());
		}
	}
}
