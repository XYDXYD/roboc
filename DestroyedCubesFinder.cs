using Svelto.DataStructures;
using System.Collections.Generic;
using Utility;

internal sealed class DestroyedCubesFinder
{
	private sealed class CheckStep
	{
		public CubeNodeInstance node;

		public CheckStep fromStep;

		public CheckStep(CubeNodeInstance n, CheckStep f)
		{
			InitStep(n, f);
		}

		public void InitStep(CubeNodeInstance n, CheckStep f)
		{
			node = n;
			fromStep = f;
		}
	}

	private FasterList<CubeNodeInstance> _allProcessedNodes = new FasterList<CubeNodeInstance>();

	private FasterList<InstantiatedCube> _allSeperatedCubes = new FasterList<InstantiatedCube>();

	private Queue<CheckStep> stepsToProcess = new Queue<CheckStep>();

	private FasterList<CubeNodeInstance> linksToChairToProcess = new FasterList<CubeNodeInstance>();

	private Stack<CheckStep> unusedSteps = new Stack<CheckStep>();

	private Stack<CheckStep> usedSteps = new Stack<CheckStep>();

	private int _maxNumLinksToChair;

	private bool _foundLinkToChair;

	private Queue<CubeNodeInstance> _cubesToProcess = new Queue<CubeNodeInstance>();

	private FasterList<CubeNodeInstance> neighbours = new FasterList<CubeNodeInstance>();

	private FasterList<CubeNodeInstance> updatedNodes = new FasterList<CubeNodeInstance>();

	private FasterList<CubeNodeInstance> _destroyedNodes = new FasterList<CubeNodeInstance>();

	private static int numToRecycle;

	public FasterList<InstantiatedCube> FindSeperatedCubes(FasterList<InstantiatedCube> destroyedCubes, FasterList<uint> indices, ref bool isRootDestroyed)
	{
		_allSeperatedCubes.FastClear();
		FasterList<CubeNodeInstance> val = SetupDestroyedCubes(destroyedCubes, ref isRootDestroyed);
		if (isRootDestroyed)
		{
			for (int num = val.get_Count() - 1; num >= 0; num--)
			{
				BreadthFirstUpdateRoot(val.get_Item(num), indices);
			}
		}
		else
		{
			for (int num2 = val.get_Count() - 1; num2 >= 0; num2--)
			{
				BreadthFirstCheckNeighboursForLinkToChair(val.get_Item(num2), indices);
			}
		}
		return _allSeperatedCubes;
	}

	private void BreadthFirstUpdateRoot(CubeNodeInstance destroyedNode, FasterList<uint> indices)
	{
		FasterList<CubeNodeInstance> val = destroyedNode.GetNeighbours();
		for (int num = val.get_Count() - 1; num >= 0; num--)
		{
			if (!val.get_Item(num).isDestroyed)
			{
				_allProcessedNodes.FastClear();
				UpdateGraph(val.get_Item(num));
				uint num2 = 0u;
				for (int i = 0; i < _allProcessedNodes.get_Count(); i++)
				{
					InstantiatedCube instantiatedCube = _allProcessedNodes.get_Item(i).instantiatedCube;
					_allSeperatedCubes.Add(instantiatedCube);
					_allProcessedNodes.get_Item(i).isDestroyed = true;
					num2 += instantiatedCube.persistentCubeData.cpuRating;
					_allProcessedNodes.get_Item(i).processed = false;
				}
				if (_allProcessedNodes.get_Count() > 0)
				{
					uint num3 = (indices.get_Count() != 0) ? (indices.get_Item(indices.get_Count() - 3) + indices.get_Item(indices.get_Count() - 2)) : 0u;
					indices.Add(num3);
					indices.Add((uint)_allProcessedNodes.get_Count());
					indices.Add(num2);
				}
			}
		}
	}

	private void UpdateGraph(CubeNodeInstance root)
	{
		root.linkToChair = null;
		root.processed = true;
		_cubesToProcess.Enqueue(root);
		_allProcessedNodes.Add(root);
		while (_cubesToProcess.Count > 0)
		{
			CubeNodeInstance cubeNodeInstance = _cubesToProcess.Dequeue();
			FasterList<CubeNodeInstance> val = cubeNodeInstance.GetNeighbours();
			for (int num = val.get_Count() - 1; num >= 0; num--)
			{
				CubeNodeInstance cubeNodeInstance2 = val.get_Item(num);
				if (!cubeNodeInstance2.processed && !cubeNodeInstance2.isDestroyed)
				{
					cubeNodeInstance2.processed = true;
					cubeNodeInstance2.linkToChair = cubeNodeInstance;
					_cubesToProcess.Enqueue(cubeNodeInstance2);
					_allProcessedNodes.Add(cubeNodeInstance2);
				}
			}
		}
	}

	public FasterList<InstantiatedCube> FindSeperatedCubes(InstantiatedCube removedCube, FasterList<uint> indices)
	{
		_allSeperatedCubes.FastClear();
		CubeNodeInstance cubeNodeInstance = removedCube.cubeNodeInstance;
		cubeNodeInstance.isDestroyed = true;
		BreadthFirstCheckNeighboursForLinkToChair(cubeNodeInstance, indices);
		return _allSeperatedCubes;
	}

	private void BreadthFirstCheckNeighboursForLinkToChair(CubeNodeInstance destroyedNode, FasterList<uint> indices)
	{
		_maxNumLinksToChair = destroyedNode.numLinksToChair;
		FasterList<CubeNodeInstance> val = destroyedNode.GetNeighbours();
		for (int num = val.get_Count() - 1; num >= 0; num--)
		{
			_allProcessedNodes.FastClear();
			_foundLinkToChair = false;
			stepsToProcess.Enqueue(GetStep(val.get_Item(num), null));
			while (!_foundLinkToChair && stepsToProcess.Count > 0)
			{
				CheckStep step = stepsToProcess.Dequeue();
				_foundLinkToChair |= CheckCubeNeighbours(step);
			}
			if (_foundLinkToChair)
			{
				stepsToProcess.Clear();
			}
			uint num2 = 0u;
			for (int i = 0; i < _allProcessedNodes.get_Count(); i++)
			{
				if (!_foundLinkToChair)
				{
					InstantiatedCube instantiatedCube = _allProcessedNodes.get_Item(i).instantiatedCube;
					instantiatedCube.isConnected = false;
					_allSeperatedCubes.Add(instantiatedCube);
					_allProcessedNodes.get_Item(i).isDestroyed = true;
					num2 += instantiatedCube.persistentCubeData.cpuRating;
				}
				_allProcessedNodes.get_Item(i).processed = false;
			}
			if (!_foundLinkToChair && _allProcessedNodes.get_Count() > 0)
			{
				uint num3 = (indices.get_Count() != 0) ? (indices.get_Item(indices.get_Count() - 3) + indices.get_Item(indices.get_Count() - 2)) : 0u;
				indices.Add(num3);
				indices.Add((uint)_allProcessedNodes.get_Count());
				indices.Add(num2);
			}
			RecycleSteps();
		}
	}

	private bool CheckCubeNeighbours(CheckStep step)
	{
		if (step.node.processed || step.node.isDestroyed)
		{
			return false;
		}
		if (step.node.numLinksToChair == 0)
		{
			return true;
		}
		step.node.processed = true;
		_allProcessedNodes.Add(step.node);
		neighbours.FastClear();
		neighbours.AddRange(step.node.GetNeighbours());
		CubeNodeInstance cubeNodeInstance = null;
		int num = int.MaxValue;
		for (int num2 = neighbours.get_Count() - 1; num2 >= 0; num2--)
		{
			CubeNodeInstance cubeNodeInstance2 = neighbours.get_Item(num2);
			if (IsBetterValidLinkToChair(cubeNodeInstance2) && cubeNodeInstance2.numLinksToChair < num)
			{
				num = cubeNodeInstance2.numLinksToChair;
				cubeNodeInstance = cubeNodeInstance2;
			}
		}
		if (cubeNodeInstance != null)
		{
			SetLinkToNode(cubeNodeInstance, step);
			return true;
		}
		EnqueueNeighbours(neighbours, step);
		return false;
	}

	private void SetLinkToNode(CubeNodeInstance node, CheckStep step)
	{
		updatedNodes.FastClear();
		if (step != null)
		{
			step.node.linkToChair = node;
			while (step.fromStep != null && step.fromStep != step)
			{
				updatedNodes.Add(step.node);
				step.fromStep.node.linkToChair = step.node;
				step = step.fromStep;
			}
			updatedNodes.Add(step.node);
		}
		for (int i = 0; i < updatedNodes.get_Count(); i++)
		{
			UpdateNumberOfLinksToChair(updatedNodes.get_Item(i));
		}
	}

	private void UpdateNumberOfLinksToChair(CubeNodeInstance updateNode)
	{
		linksToChairToProcess.FastClear();
		linksToChairToProcess.Add(updateNode);
		while (linksToChairToProcess.get_Count() > 0)
		{
			CubeNodeInstance cubeNodeInstance = linksToChairToProcess.get_Item(0);
			linksToChairToProcess.UnorderedRemoveAt(0);
			cubeNodeInstance.UpdateNumberOfLinksToChair();
			FasterList<CubeNodeInstance> neighboursThatLinkToMe = cubeNodeInstance.GetNeighboursThatLinkToMe();
			for (int i = 0; i < neighboursThatLinkToMe.get_Count(); i++)
			{
				if (neighboursThatLinkToMe.get_Item(i).numLinksToChair != cubeNodeInstance.numLinksToChair + 1)
				{
					linksToChairToProcess.Add(neighboursThatLinkToMe.get_Item(i));
				}
			}
		}
	}

	private void EnqueueNeighbours(FasterList<CubeNodeInstance> neighbours, CheckStep step)
	{
		for (int num = neighbours.get_Count() - 1; num >= 0; num--)
		{
			if (!neighbours.get_Item(num).processed && !neighbours.get_Item(num).isDestroyed)
			{
				stepsToProcess.Enqueue(GetStep(neighbours.get_Item(num), step));
			}
		}
	}

	private FasterList<CubeNodeInstance> SetupDestroyedCubes(FasterList<InstantiatedCube> destroyedCubes, ref bool isRootDestroyed)
	{
		_destroyedNodes.FastClear();
		for (int num = destroyedCubes.get_Count() - 1; num >= 0; num--)
		{
			CubeNodeInstance cubeNodeInstance = destroyedCubes.get_Item(num).cubeNodeInstance;
			if (cubeNodeInstance.linkToChair == null)
			{
				isRootDestroyed = true;
			}
			cubeNodeInstance.isDestroyed = true;
			_destroyedNodes.Add(cubeNodeInstance);
		}
		return _destroyedNodes;
	}

	private bool IsBetterValidLinkToChair(CubeNodeInstance node)
	{
		if (node.numLinksToChair > _maxNumLinksToChair)
		{
			return false;
		}
		while (node != null)
		{
			if (node.processed || node.isDestroyed)
			{
				return false;
			}
			node = node.linkToChair;
		}
		return true;
	}

	private CheckStep GetStep(CubeNodeInstance n, CheckStep f)
	{
		if (unusedSteps.Count > 0)
		{
			CheckStep checkStep = unusedSteps.Pop();
			checkStep.InitStep(n, f);
			usedSteps.Push(checkStep);
			return checkStep;
		}
		CheckStep checkStep2 = new CheckStep(n, f);
		usedSteps.Push(checkStep2);
		return checkStep2;
	}

	private void RecycleSteps()
	{
		numToRecycle = usedSteps.Count;
		for (int num = numToRecycle - 1; num >= 0; num--)
		{
			unusedSteps.Push(usedSteps.Pop());
		}
	}

	private void SpecialCheckNumLinks(CubeNodeInstance node)
	{
		HashSet<CubeNodeInstance> prev = new HashSet<CubeNodeInstance>();
		SpecialRecursiveCheckNumLinks(node, prev);
	}

	private void SpecialRecursiveCheckNumLinks(CubeNodeInstance node, HashSet<CubeNodeInstance> prev)
	{
		prev.Add(node);
		FasterList<CubeNodeInstance> neighboursThatLinkToMe = node.GetNeighboursThatLinkToMe();
		for (int i = 0; i < neighboursThatLinkToMe.get_Count(); i++)
		{
			if (neighboursThatLinkToMe.get_Item(i).numLinksToChair != node.numLinksToChair + 1)
			{
				Console.LogWarning("Incorrect num links to chair for " + neighboursThatLinkToMe.get_Item(i).instantiatedCube.name);
			}
			if (prev.Contains(neighboursThatLinkToMe.get_Item(i)))
			{
				Console.LogError("Found loop to " + neighboursThatLinkToMe.get_Item(i).instantiatedCube.name);
			}
			else
			{
				SpecialRecursiveCheckNumLinks(neighboursThatLinkToMe.get_Item(i), prev);
			}
		}
	}
}
