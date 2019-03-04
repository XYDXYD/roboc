using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class PaintFillController
	{
		public PaintFillProgressView paintFillProgressView;

		private const float MAX_FLOOD_FILL_STEP_TIME = 0.1f;

		private const float MAX_FILL_TIME = 1.5f;

		private const float MAX_REFILL_TIME = 0.25f;

		private byte _newPaintAllColorIndex;

		private PaletteColor _newPaintAllColor;

		private HashSet<InstantiatedCube> _remainingCubesToFill = new HashSet<InstantiatedCube>();

		private FasterList<InstantiatedCube> _startingCubesToFill = new FasterList<InstantiatedCube>();

		private FasterList<InstantiatedCube> _currentNeighbourCubes = new FasterList<InstantiatedCube>();

		private FasterList<InstantiatedCube> _unattachedCubes = new FasterList<InstantiatedCube>();

		private FasterList<InstantiatedCube> _paintableNeighbourCubes = new FasterList<InstantiatedCube>();

		private FasterList<FasterList<InstantiatedCube>> _cubesAlreadyFilled = new FasterList<FasterList<InstantiatedCube>>();

		private HashSet<InstantiatedCube> _processedCubes = new HashSet<InstantiatedCube>();

		private float _stepTime;

		private float _currentPaintTime;

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
		{
			get;
			private set;
		}

		[Inject]
		internal CurrentToolMode toolMode
		{
			get;
			private set;
		}

		public bool painting
		{
			get;
			private set;
		}

		public bool rePainting
		{
			get;
			private set;
		}

		public event Action OnMachinePainted = delegate
		{
		};

		internal void StartPaintAll(Byte3 gridPos, byte newIndex, PaletteColor newColor)
		{
			painting = true;
			toolMode.ObtainSwitchingLock(CurrentToolMode.SwitchingLockTypes.Painting);
			_currentPaintTime = 0f;
			_newPaintAllColorIndex = newIndex;
			_newPaintAllColor = newColor;
			_remainingCubesToFill.Clear();
			_remainingCubesToFill.UnionWith(machineMap.GetAllInstantiatedCubes());
			InstantiatedCube info = machineMap.GetCellAt(gridPos).info;
			int num = FindNumPaintSteps(info);
			_startingCubesToFill.FastClear();
			info.previousPaletteColor = info.paletteColor;
			info.previousPaletteIndex = info.paletteIndex;
			info.paletteColor = newColor;
			info.paletteIndex = newIndex;
			_startingCubesToFill.Add(info);
			colorUpdater.PaintGroupOfCubes(_startingCubesToFill);
			info.cubeNodeInstance.processed = true;
			_processedCubes.Add(info);
			_stepTime = 1.5f / (float)num;
			paintFillProgressView.ShowProgressBar(num);
		}

		internal void StepPaintAll()
		{
			_currentPaintTime += Time.get_deltaTime();
			if (_currentPaintTime > _stepTime)
			{
				_currentPaintTime = 0f;
				PaintAllCubes(_startingCubesToFill);
				paintFillProgressView.StepForward();
			}
		}

		internal void CancelPaintAll()
		{
			if (painting)
			{
				painting = false;
				rePainting = true;
				_cubesAlreadyFilled.Add(_startingCubesToFill);
				float refillStepTime = 0.25f / (float)_cubesAlreadyFilled.get_Count();
				TaskRunner.get_Instance().Run(RefillAllCubes(refillStepTime));
			}
		}

		private IEnumerator RefillAllCubes(float refillStepTime)
		{
			for (int i = _cubesAlreadyFilled.get_Count() - 1; i >= 0; i--)
			{
				for (int j = 0; j < _cubesAlreadyFilled.get_Item(i).get_Count(); j++)
				{
					_cubesAlreadyFilled.get_Item(i).get_Item(j).paletteIndex = _cubesAlreadyFilled.get_Item(i).get_Item(j).previousPaletteIndex;
					_cubesAlreadyFilled.get_Item(i).get_Item(j).paletteColor = _cubesAlreadyFilled.get_Item(i).get_Item(j).previousPaletteColor;
				}
				colorUpdater.PaintGroupOfCubes(_cubesAlreadyFilled.get_Item(i));
				paintFillProgressView.StepBackward();
				yield return (object)new WaitForSecondsEnumerator(refillStepTime);
			}
			HashSet<InstantiatedCube>.Enumerator enumerator = _processedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.cubeNodeInstance.processed = false;
			}
			_cubesAlreadyFilled.FastClear();
			paintFillProgressView.ClearProgressBar();
			rePainting = false;
			toolMode.ReleaseSwitchingLock(CurrentToolMode.SwitchingLockTypes.Painting);
		}

		private void PaintAllCubes(FasterList<InstantiatedCube> neighbours)
		{
			_cubesAlreadyFilled.Add(neighbours);
			_startingCubesToFill = PaintNextCubes(_startingCubesToFill);
			CheckUnattachedCubesToPaint(neighbours);
			if (_processedCubes.Count >= _remainingCubesToFill.Count)
			{
				this.OnMachinePainted();
				HashSet<InstantiatedCube>.Enumerator enumerator = _processedCubes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.cubeNodeInstance.processed = false;
				}
				_processedCubes.Clear();
				_startingCubesToFill.FastClear();
				_cubesAlreadyFilled.FastClear();
				paintFillProgressView.ClearProgressBar();
				this.OnMachinePainted();
				painting = false;
				toolMode.ReleaseSwitchingLock(CurrentToolMode.SwitchingLockTypes.Painting);
			}
		}

		private FasterList<InstantiatedCube> PaintNextCubes(FasterList<InstantiatedCube> startCubes)
		{
			_currentNeighbourCubes.FastClear();
			for (int i = 0; i < startCubes.get_Count(); i++)
			{
				FindNeighbourCubes(startCubes.get_Item(i), _currentNeighbourCubes);
			}
			CheckAlreadyPaintedCubes();
			colorUpdater.PaintGroupOfCubes(_paintableNeighbourCubes);
			for (int j = 0; j < _currentNeighbourCubes.get_Count(); j++)
			{
				_currentNeighbourCubes.get_Item(j).cubeNodeInstance.processed = true;
				_processedCubes.Add(_currentNeighbourCubes.get_Item(j));
			}
			return new FasterList<InstantiatedCube>(_currentNeighbourCubes);
		}

		private void CheckAlreadyPaintedCubes()
		{
			_paintableNeighbourCubes.FastClear();
			for (int i = 0; i < _currentNeighbourCubes.get_Count(); i++)
			{
				if (_currentNeighbourCubes.get_Item(i).paletteIndex != _newPaintAllColorIndex)
				{
					_paintableNeighbourCubes.Add(_currentNeighbourCubes.get_Item(i));
				}
				_currentNeighbourCubes.get_Item(i).previousPaletteColor = _currentNeighbourCubes.get_Item(i).paletteColor;
				_currentNeighbourCubes.get_Item(i).previousPaletteIndex = _currentNeighbourCubes.get_Item(i).paletteIndex;
				_currentNeighbourCubes.get_Item(i).paletteColor = _newPaintAllColor;
				_currentNeighbourCubes.get_Item(i).paletteIndex = _newPaintAllColorIndex;
			}
		}

		private void CheckUnattachedCubesToPaint(FasterList<InstantiatedCube> startCubes)
		{
			if (startCubes.get_Count() == 0 && _processedCubes.Count < _remainingCubesToFill.Count)
			{
				_remainingCubesToFill.ExceptWith(_processedCubes);
				_unattachedCubes.FastClear();
				HashSet<InstantiatedCube>.Enumerator enumerator = _remainingCubesToFill.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.Current;
					current.cubeNodeInstance.processed = true;
					_unattachedCubes.Add(current);
					current.previousPaletteColor = current.paletteColor;
					current.previousPaletteIndex = current.paletteIndex;
					current.paletteColor = _newPaintAllColor;
					current.paletteIndex = _newPaintAllColorIndex;
				}
				colorUpdater.PaintGroupOfCubes(_unattachedCubes);
				_processedCubes.UnionWith(_remainingCubesToFill);
				_remainingCubesToFill.UnionWith(_processedCubes);
			}
		}

		private void FindNeighbourCubes(InstantiatedCube cube, FasterList<InstantiatedCube> currentNeighbours)
		{
			FasterList<CubeNodeInstance> neighbours = cube.cubeNodeInstance.GetNeighbours();
			for (int i = 0; i < neighbours.get_Count(); i++)
			{
				CubeNodeInstance cubeNodeInstance = neighbours.get_Item(i);
				if (!currentNeighbours.Contains(cubeNodeInstance.instantiatedCube) && !cubeNodeInstance.processed)
				{
					currentNeighbours.Add(cubeNodeInstance.instantiatedCube);
				}
			}
		}

		private int FindNumPaintSteps(InstantiatedCube startCube)
		{
			_currentNeighbourCubes.FastClear();
			_startingCubesToFill.FastClear();
			_startingCubesToFill.Add(startCube);
			startCube.cubeNodeInstance.processed = true;
			_processedCubes.Add(startCube);
			int num = 1;
			while (_processedCubes.Count < _remainingCubesToFill.Count)
			{
				for (int i = 0; i < _startingCubesToFill.get_Count(); i++)
				{
					FindNeighbourCubes(_startingCubesToFill.get_Item(i), _currentNeighbourCubes);
				}
				for (int j = 0; j < _currentNeighbourCubes.get_Count(); j++)
				{
					_currentNeighbourCubes.get_Item(j).cubeNodeInstance.processed = true;
					_processedCubes.Add(_currentNeighbourCubes.get_Item(j));
				}
				_startingCubesToFill.FastClear();
				_startingCubesToFill.AddRange(_currentNeighbourCubes);
				_currentNeighbourCubes.FastClear();
				if (_startingCubesToFill.get_Count() == 0 && _processedCubes.Count < _remainingCubesToFill.Count)
				{
					_processedCubes.UnionWith(_remainingCubesToFill);
				}
				num++;
			}
			HashSet<InstantiatedCube>.Enumerator enumerator = _processedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.cubeNodeInstance.processed = false;
			}
			_processedCubes.Clear();
			_startingCubesToFill.FastClear();
			_currentNeighbourCubes.FastClear();
			return num;
		}
	}
}
