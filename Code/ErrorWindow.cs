using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal static class ErrorWindow
{
	private static WeakReference<GenericErrorDialogue> _dialogue;

	private static Dictionary<string, string> _details = new Dictionary<string, string>();

	private static Queue<Action> _windowsToShow = new Queue<Action>();

	public static void Init(IGameObjectFactory factory)
	{
		Init(factory.Build("ErrorDialog"));
	}

	public static void Init(GameObject errorWindow)
	{
		GenericErrorDialogue component = errorWindow.GetComponent<GenericErrorDialogue>();
		if (_dialogue == null)
		{
			_dialogue = new WeakReference<GenericErrorDialogue>(component);
		}
		else
		{
			_dialogue.set_Target(component);
		}
	}

	public static void ShowServiceErrorWindow(ServiceBehaviour behaviour, Action onRetryExtraAction)
	{
		ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
		{
			if (onRetryExtraAction != null)
			{
				onRetryExtraAction();
			}
			if (behaviour.MainBehaviour != null)
			{
				behaviour.MainBehaviour();
			}
		}, behaviour.Alternative), behaviour.exceptionThrown.StackTrace);
	}

	public static void ShowServiceErrorWindow(ServiceBehaviour behaviour)
	{
		ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, behaviour.mainText, behaviour.alternativeText, delegate
		{
			if (behaviour.MainBehaviour != null)
			{
				behaviour.MainBehaviour();
			}
		}, behaviour.Alternative), behaviour.exceptionThrown.StackTrace);
	}

	public static void ShowErrorWindow(GenericErrorData error, string stackTrace = null)
	{
		RemoteLogger.Error(error.body, null, stackTrace, _details);
		ShowErrorWindowInternal(error);
	}

	private static void ShowErrorWindowInternal(GenericErrorData error)
	{
		if (_dialogue == null || IsErrorWindowOpen())
		{
			_windowsToShow.Enqueue(delegate
			{
				ShowErrorWindowInternal(error);
			});
			return;
		}
		DeactivateLoadingScreenIfActive();
		Action clicked = error.onOkClicked;
		Action okClicked = delegate
		{
			if (_dialogue.get_IsValid())
			{
				_dialogue.get_Target().Close();
			}
			if (_windowsToShow.Count > 0)
			{
				_windowsToShow.Dequeue()();
			}
			if (clicked != null)
			{
				clicked();
			}
		};
		Action cancelClicked = error.onCancelClicked;
		Action cancelClicked2 = delegate
		{
			if (_dialogue.get_IsValid())
			{
				_dialogue.get_Target().Close();
			}
			if (_windowsToShow.Count > 0)
			{
				_windowsToShow.Dequeue()();
			}
			if (cancelClicked != null)
			{
				cancelClicked();
			}
		};
		if (_dialogue.get_IsValid())
		{
			_dialogue.get_Target().Open(new GenericErrorData(error.header, error.body, error.okButtonText, error.cancelButtonText, okClicked, cancelClicked2));
		}
	}

	public static bool IsErrorWindowOpen()
	{
		if (_dialogue == null)
		{
			return false;
		}
		if (_dialogue.get_IsValid() && _dialogue.get_Target().isOpen)
		{
			return true;
		}
		return false;
	}

	private static void DeactivateLoadingScreenIfActive()
	{
		GenericLoadingScreen[] array = Object.FindObjectsOfType<GenericLoadingScreen>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Console.LogWarning(FastConcatUtility.FastConcat<string>("Loading Icon Still Opened on Error, your service user must close the loading icon on error!", FastConcatUtility.FastConcat<string>("Loading Screen: ", array[i].get_gameObject().get_name())));
				array[i].get_gameObject().SetActive(false);
			}
		}
	}

	internal static void TearDown()
	{
		_windowsToShow = new Queue<Action>();
	}
}
