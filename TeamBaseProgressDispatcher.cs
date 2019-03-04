using System;
using System.Collections.Generic;

internal sealed class TeamBaseProgressDispatcher
{
	public delegate void BaseProgressChangedEventHandler(float i);

	public delegate void CaptureStartedEventHandler();

	public delegate void CaptureStoppedEventHandler();

	public delegate void CaptureResetEventHandler();

	public delegate void SectionCompleteEventHandler();

	public delegate void FinalSectionCompleteEventHandler();

	private Dictionary<int, BaseProgressChangedEventHandler> onBaseProgressChanged = new Dictionary<int, BaseProgressChangedEventHandler>();

	private Dictionary<int, CaptureStartedEventHandler> onCaptureStarted = new Dictionary<int, CaptureStartedEventHandler>();

	private Dictionary<int, CaptureStoppedEventHandler> onCaptureStopped = new Dictionary<int, CaptureStoppedEventHandler>();

	private Dictionary<int, CaptureResetEventHandler> onCaptureReset = new Dictionary<int, CaptureResetEventHandler>();

	private Dictionary<int, SectionCompleteEventHandler> onSectionComplete = new Dictionary<int, SectionCompleteEventHandler>();

	private Dictionary<int, FinalSectionCompleteEventHandler> onFinalSectionComplete = new Dictionary<int, FinalSectionCompleteEventHandler>();

	public void OnBaseProgressChanged(int teamId, float progress)
	{
		if (onBaseProgressChanged.ContainsKey(teamId))
		{
			onBaseProgressChanged[teamId](progress);
		}
	}

	public void OnCaptureStarted(int teamId)
	{
		if (onCaptureStarted.ContainsKey(teamId))
		{
			onCaptureStarted[teamId]();
		}
	}

	public void OnCaptureReset(int teamId)
	{
		if (onCaptureReset.ContainsKey(teamId))
		{
			onCaptureReset[teamId]();
		}
	}

	public void OnCaptureStopped(int teamId)
	{
		if (onCaptureStopped.ContainsKey(teamId))
		{
			onCaptureStopped[teamId]();
		}
	}

	public void OnSectionComplete(int teamId)
	{
		if (onSectionComplete.ContainsKey(teamId))
		{
			onSectionComplete[teamId]();
		}
	}

	public void OnFinalSectionComplete(int teamId)
	{
		if (onFinalSectionComplete.ContainsKey(teamId))
		{
			onFinalSectionComplete[teamId]();
		}
	}

	public void RegisterBaseChange(int team, BaseProgressChangedEventHandler onBaseChangedEvent)
	{
		if (!onBaseProgressChanged.ContainsKey(team))
		{
			onBaseProgressChanged.Add(team, onBaseChangedEvent);
		}
		else
		{
			Dictionary<int, BaseProgressChangedEventHandler> dictionary;
			int key;
			(dictionary = onBaseProgressChanged)[key = team] = (BaseProgressChangedEventHandler)Delegate.Combine(dictionary[key], onBaseChangedEvent);
		}
	}

	public void UnRegisterBaseChange(int team, BaseProgressChangedEventHandler onBaseChangedEvent)
	{
		Dictionary<int, BaseProgressChangedEventHandler> dictionary;
		int key;
		(dictionary = onBaseProgressChanged)[key = team] = (BaseProgressChangedEventHandler)Delegate.Remove(dictionary[key], onBaseChangedEvent);
	}

	public void RegisterCaptureStarted(int team, CaptureStartedEventHandler onCaptureStartedEvent)
	{
		if (!onCaptureStarted.ContainsKey(team))
		{
			onCaptureStarted.Add(team, onCaptureStartedEvent);
		}
		else
		{
			Dictionary<int, CaptureStartedEventHandler> dictionary;
			int key;
			(dictionary = onCaptureStarted)[key = team] = (CaptureStartedEventHandler)Delegate.Combine(dictionary[key], onCaptureStartedEvent);
		}
	}

	public void UnRegisterCaptureStarted(int team, CaptureStartedEventHandler onCaptureStartedEvent)
	{
		Dictionary<int, CaptureStartedEventHandler> dictionary;
		int key;
		(dictionary = onCaptureStarted)[key = team] = (CaptureStartedEventHandler)Delegate.Remove(dictionary[key], onCaptureStartedEvent);
	}

	public void RegisterCaptureReset(int team, CaptureResetEventHandler onCaptureResetEvent)
	{
		if (!onCaptureReset.ContainsKey(team))
		{
			onCaptureReset.Add(team, onCaptureResetEvent);
		}
		else
		{
			Dictionary<int, CaptureResetEventHandler> dictionary;
			int key;
			(dictionary = onCaptureReset)[key = team] = (CaptureResetEventHandler)Delegate.Combine(dictionary[key], onCaptureResetEvent);
		}
	}

	public void UnRegisterCaptureReset(int team, CaptureResetEventHandler onCaptureResetEvent)
	{
		Dictionary<int, CaptureResetEventHandler> dictionary;
		int key;
		(dictionary = onCaptureReset)[key = team] = (CaptureResetEventHandler)Delegate.Remove(dictionary[key], onCaptureResetEvent);
	}

	public void RegisterCaptureStopped(int team, CaptureStoppedEventHandler onCaptureStoppedEvent)
	{
		if (!onCaptureStopped.ContainsKey(team))
		{
			onCaptureStopped.Add(team, onCaptureStoppedEvent);
		}
		else
		{
			Dictionary<int, CaptureStoppedEventHandler> dictionary;
			int key;
			(dictionary = onCaptureStopped)[key = team] = (CaptureStoppedEventHandler)Delegate.Combine(dictionary[key], onCaptureStoppedEvent);
		}
	}

	public void UnRegisterCaptureStopped(int team, CaptureStoppedEventHandler onCaptureStoppedEvent)
	{
		Dictionary<int, CaptureStoppedEventHandler> dictionary;
		int key;
		(dictionary = onCaptureStopped)[key = team] = (CaptureStoppedEventHandler)Delegate.Remove(dictionary[key], onCaptureStoppedEvent);
	}

	public void RegisterSectionComplete(int team, SectionCompleteEventHandler onSectionCompleteEvent)
	{
		if (!onSectionComplete.ContainsKey(team))
		{
			onSectionComplete.Add(team, onSectionCompleteEvent);
		}
		else
		{
			Dictionary<int, SectionCompleteEventHandler> dictionary;
			int key;
			(dictionary = onSectionComplete)[key = team] = (SectionCompleteEventHandler)Delegate.Combine(dictionary[key], onSectionCompleteEvent);
		}
	}

	public void UnRegisterSectionComplete(int team, SectionCompleteEventHandler onSectionCompleteEvent)
	{
		Dictionary<int, SectionCompleteEventHandler> dictionary;
		int key;
		(dictionary = onSectionComplete)[key = team] = (SectionCompleteEventHandler)Delegate.Remove(dictionary[key], onSectionCompleteEvent);
	}

	public void RegisterFinalSectionComplete(int team, FinalSectionCompleteEventHandler onFinalSectionCompleteEvent)
	{
		if (!onFinalSectionComplete.ContainsKey(team))
		{
			onFinalSectionComplete.Add(team, onFinalSectionCompleteEvent);
		}
		else
		{
			Dictionary<int, FinalSectionCompleteEventHandler> dictionary;
			int key;
			(dictionary = onFinalSectionComplete)[key = team] = (FinalSectionCompleteEventHandler)Delegate.Combine(dictionary[key], onFinalSectionCompleteEvent);
		}
	}

	public void UnRegisterFinalSectionComplete(int team, FinalSectionCompleteEventHandler onFinalSectionCompleteEvent)
	{
		Dictionary<int, FinalSectionCompleteEventHandler> dictionary;
		int key;
		(dictionary = onFinalSectionComplete)[key = team] = (FinalSectionCompleteEventHandler)Delegate.Remove(dictionary[key], onFinalSectionCompleteEvent);
	}
}
