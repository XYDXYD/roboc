using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

public class GenericBroadCasterCommand : IDispatchableCommand, ICommand
{
	private SignalChain _signal;

	private INotification _notification;

	private bool _deepBroadcast;

	public GenericBroadCasterCommand(Transform transform, INotification notification, bool deepBroadcast = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		_signal = new SignalChain(transform);
		_notification = notification;
		_deepBroadcast = deepBroadcast;
	}

	public void Execute()
	{
		if (_deepBroadcast)
		{
			_signal.DeepBroadcast<INotification>(_notification);
		}
		else
		{
			_signal.Broadcast<INotification>(_notification);
		}
	}
}
