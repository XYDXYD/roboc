using Simulation;
using Simulation.Hardware.Modules.Emp.Observers;
using Svelto.ES.Legacy;
using Svelto.Observer;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class AntiGravCubeManager : ITickable, IAlignmentRectifierPausable, ITickableBase, IComponent
{
	private int _numAntiGravCubes;

	private List<CubeAntiGravity> _antiGravCubes = new List<CubeAntiGravity>();

	private bool _functionalsEnabled = true;

	public void AlignmentRectifierStarted()
	{
		_functionalsEnabled = false;
		UpdateFunctionalStates();
	}

	public void AlignmentRectifierEnded()
	{
		_functionalsEnabled = true;
		UpdateFunctionalStates();
	}

	public unsafe void InitialiseMachineAntiGravCubes(GameObject machineRoot, ITicker ticker, MachineStunnedObserver observer)
	{
		CubeAntiGravity[] componentsInChildren = machineRoot.GetComponentsInChildren<CubeAntiGravity>();
		if (componentsInChildren.Length != 0)
		{
			_antiGravCubes.AddRange(componentsInChildren);
			ticker.Add(this);
			observer.AddAction(new ObserverAction<MachineStunnedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
	}

	private void HandleMachineStunned(ref MachineStunnedData data)
	{
		_functionalsEnabled = !data.isStunned;
		UpdateFunctionalStates();
	}

	public void Tick(float deltaTime)
	{
		_antiGravCubes.RemoveAll((CubeAntiGravity h) => h == null);
		int num = 0;
		for (int i = 0; i < _antiGravCubes.Count; i++)
		{
			CubeAntiGravity cubeAntiGravity = _antiGravCubes[i];
			if (cubeAntiGravity.get_gameObject().get_activeSelf())
			{
				num++;
			}
		}
		if (_numAntiGravCubes != num)
		{
			UpdateAntiGravCubeCount(num);
			UpdateFunctionalStates();
		}
	}

	private void UpdateAntiGravCubeCount(int numAntiGravCubes)
	{
		_numAntiGravCubes = numAntiGravCubes;
		for (int i = 0; i < _antiGravCubes.Count; i++)
		{
			CubeAntiGravity cubeAntiGravity = _antiGravCubes[i];
			if (cubeAntiGravity.get_gameObject().get_activeSelf())
			{
				cubeAntiGravity.numberHeliumCubes = numAntiGravCubes;
			}
		}
	}

	private void UpdateFunctionalStates()
	{
		for (int i = 0; i < _antiGravCubes.Count; i++)
		{
			CubeAntiGravity cubeAntiGravity = _antiGravCubes[i];
			if (null != cubeAntiGravity)
			{
				cubeAntiGravity.SetFunctionalsEnabled(_functionalsEnabled);
			}
		}
	}
}
