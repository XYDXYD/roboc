using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;
using Utility;

internal sealed class MaintenanceDialog : MonoBehaviour, IChainListener
{
	public UILabel bodyText;

	private Action _callback;

	public MaintenanceDialog()
		: this()
	{
	}

	private void Awake()
	{
		if (bodyText == null)
		{
			Console.LogException(new Exception("MaintenanceDialog does not have bodyText set"));
		}
		Cursor.set_lockState(0);
		Cursor.set_visible(true);
	}

	internal void SetBodyText(string body)
	{
		bodyText.set_text(body);
	}

	public void Listen(object message)
	{
		ParseMessage(message);
	}

	private void ParseMessage(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.QuitGame)
			{
				_callback();
			}
		}
	}

	internal void SetButtonCallback(Action cb)
	{
		_callback = cb;
	}
}
