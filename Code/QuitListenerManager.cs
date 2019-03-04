using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

internal sealed class QuitListenerManager
{
	private event Func<bool> _onQuitRequested;

	private event Action _onQuitIsHappening = delegate
	{
	};

	public QuitListenerManager()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		GameObject val = new GameObject("QuitListener");
		QuitListener quitListener = val.AddComponent<QuitListener>();
		quitListener.quitListenerManager = this;
	}

	public void IsQuitting()
	{
		if (this._onQuitRequested != null)
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)QuitTriggered);
			return;
		}
		Console.Log("Quitting");
		this._onQuitIsHappening();
	}

	private IEnumerator QuitTriggered()
	{
		Console.Log("QuitTriggered");
		Application.CancelQuit();
		this._onQuitIsHappening();
		DateTime startTime = DateTime.UtcNow;
		bool canQuit = false;
		Delegate[] invocationList = this._onQuitRequested.GetInvocationList();
		while (!canQuit)
		{
			canQuit = true;
			for (int i = 0; i < invocationList.Length; i++)
			{
				Func<bool> func = invocationList[i] as Func<bool>;
				canQuit &= func();
			}
			if ((DateTime.UtcNow - startTime).TotalMilliseconds >= 500.0)
			{
				Console.Log("Taking too long.. Force quit!");
				canQuit = true;
			}
			yield return (object)new WaitForSeconds(0.1f);
		}
		this._onQuitRequested = null;
		Application.Quit();
	}

	public void AddOnQuitTriggered(Func<bool> cb)
	{
		_onQuitRequested += cb;
	}

	public void RemoveOnQuitTriggered(Func<bool> cb)
	{
		_onQuitRequested -= cb;
	}

	public void AddOnQuitIsHappening(Action cb)
	{
		_onQuitIsHappening += cb;
	}

	public void RemoveOnIsHappening(Action cb)
	{
		_onQuitIsHappening -= cb;
	}
}
