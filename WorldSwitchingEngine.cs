using Svelto.ES.Legacy;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class WorldSwitchingEngine : IEngine
{
	private IDispatchWorldSwitching _worldSwitcher;

	private List<IRunOnWorldSwitching> _runOnSwitching;

	private List<IRunOnWorldSwitching> _runOnSwitched;

	public WorldSwitchingEngine(IDispatchWorldSwitching worldSwitcher)
	{
		_runOnSwitching = new List<IRunOnWorldSwitching>();
		_runOnSwitched = new List<IRunOnWorldSwitching>();
		_worldSwitcher = worldSwitcher;
		worldSwitcher.OnWorldJustSwitched += ExecuteOnSwitched;
	}

	public Type[] AcceptedComponents()
	{
		return new Type[1]
		{
			typeof(IRunOnWorldSwitching)
		};
	}

	public void Add(IComponent component)
	{
		if (!(component as IRunOnWorldSwitching).FadeIn)
		{
			_runOnSwitching.Add(component as IRunOnWorldSwitching);
		}
		else
		{
			_runOnSwitched.Add(component as IRunOnWorldSwitching);
		}
	}

	public void Remove(IComponent component)
	{
		if (!(component as IRunOnWorldSwitching).FadeIn)
		{
			_runOnSwitching.Remove(component as IRunOnWorldSwitching);
		}
		else
		{
			_runOnSwitched.Remove(component as IRunOnWorldSwitching);
		}
	}

	private void ExecuteOnSwitched(WorldSwitchMode currentMode)
	{
		TaskRunner.get_Instance().Run(ExecuteEvent(_runOnSwitched, currentMode));
		_worldSwitcher.OnWorldIsSwitching.Add(ExecuteEvent(_runOnSwitching, currentMode));
	}

	private IEnumerator ExecuteEvent(List<IRunOnWorldSwitching> run, WorldSwitchMode currentMode)
	{
		int minPriority = 0;
		int maxPriority = 0;
		if (run.Count > 0)
		{
			int priority;
			maxPriority = (priority = run[0].Priority);
			minPriority = priority;
		}
		foreach (IRunOnWorldSwitching item in run)
		{
			minPriority = Mathf.Min(item.Priority, minPriority);
			maxPriority = Mathf.Max(item.Priority, maxPriority);
		}
		for (int currentPriority = minPriority; currentPriority <= maxPriority; currentPriority++)
		{
			float duration = 0f;
			foreach (IRunOnWorldSwitching item2 in run)
			{
				if (item2.Priority == currentPriority)
				{
					duration = Mathf.Max(item2.Duration, duration);
					item2.Execute(currentMode);
				}
			}
			if (duration > 0f)
			{
				yield return (object)new WaitForSecondsEnumerator(duration);
			}
		}
	}
}
