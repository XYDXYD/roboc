using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

internal sealed class ChatWarningDialogue
{
	private static bool _open;

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		get;
		set;
	}

	public void Execute(string heading, string body, Action okCallback)
	{
		if (!_open)
		{
			_open = true;
			GenericErrorDialogue dialogue = gameObjectFactory.Build("Suspended_Warning_Popup").GetComponent<GenericErrorDialogue>();
			GenericErrorData errorData = new GenericErrorData(heading, body, StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				_open = false;
				dialogue.Close();
				guiInputController.ToggleCurrentScreen();
				Object.Destroy(dialogue.get_gameObject());
				okCallback();
			});
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			dialogue.Open(errorData);
		}
	}
}
