using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class MachineEditorGraphUpdater : IInitialize
	{
		private class ChunkCpu
		{
			public readonly HashSet<InstantiatedCube> Cubes = new HashSet<InstantiatedCube>();

			public uint TotalCpu;

			internal void Add(InstantiatedCube cube)
			{
				if (Cubes.Add(cube))
				{
					TotalCpu += cube.persistentCubeData.cpuRating;
				}
			}

			internal void Remove(InstantiatedCube cube)
			{
				if (Cubes.Remove(cube))
				{
					TotalCpu -= cube.persistentCubeData.cpuRating;
				}
			}

			internal void ExceptWith(ChunkCpu newChunkCubes)
			{
				Cubes.ExceptWith(newChunkCubes.Cubes);
				TotalCpu -= newChunkCubes.TotalCpu;
			}
		}

		private static readonly FasterList<uint> _indices = new FasterList<uint>();

		private static readonly FasterList<CubeNodeInstance> _allNodes = new FasterList<CubeNodeInstance>();

		private readonly FasterList<InstantiatedCube> _tempCubes = new FasterList<InstantiatedCube>();

		private readonly HashSet<CubeNodeInstance> visitedNodes = new HashSet<CubeNodeInstance>();

		private readonly FasterList<InstantiatedCube> _newConnectedCubes = new FasterList<InstantiatedCube>();

		private FasterList<InstantiatedCube> _newDisconnectedCubes = new FasterList<InstantiatedCube>();

		private DestroyedCubesFinder _destroyedCubesFinder;

		private readonly HashSet<InstantiatedCube> _tempHashSet = new HashSet<InstantiatedCube>();

		private readonly Dictionary<InstantiatedCube, ChunkCpu> _chunks = new Dictionary<InstantiatedCube, ChunkCpu>();

		private readonly Dictionary<InstantiatedCube, InstantiatedCube> _rootPerCube = new Dictionary<InstantiatedCube, InstantiatedCube>();

		private readonly HashSet<InstantiatedCube> _chunksToProcess = new HashSet<InstantiatedCube>();

		private readonly Queue<CubeNodeInstance> _cubesToProcess = new Queue<CubeNodeInstance>();

		private InstantiatedCube _maxChunkRoot;

		public int NewDisconnectedCubesCount => _newDisconnectedCubes.get_Count();

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		public event Action<FasterList<InstantiatedCube>> OnCubesDisconnected = delegate
		{
		};

		public event Action<FasterList<InstantiatedCube>> OnCubesConnected = delegate
		{
		};

		public event Action<FasterList<InstantiatedCube>> OnGraphInitialized = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			_destroyedCubesFinder = new DestroyedCubesFinder();
		}

		public FasterList<InstantiatedCube> GetDisconnectedCubeListCopy()
		{
			_tempCubes.FastClear();
			foreach (KeyValuePair<InstantiatedCube, ChunkCpu> chunk in _chunks)
			{
				if (chunk.Key != _maxChunkRoot)
				{
					_tempCubes.AddRange((IEnumerable<InstantiatedCube>)chunk.Value.Cubes, chunk.Value.Cubes.Count);
				}
			}
			return _tempCubes;
		}

		public bool AreCubesDisconnected()
		{
			return _chunks.Keys.Count > 1;
		}

		public void UpdateResources()
		{
			_chunks.Clear();
			_rootPerCube.Clear();
			HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			UpdateGraph(allInstantiatedCubes);
			HashSet<InstantiatedCube> cubes = _chunks[_maxChunkRoot].Cubes;
			foreach (InstantiatedCube item in cubes)
			{
				item.isConnected = true;
			}
		}

		public void Initialize()
		{
			_chunks.Clear();
			_rootPerCube.Clear();
			HashSet<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
			CreateGraph(allInstantiatedCubes);
			_newDisconnectedCubes.FastClear();
			if (allInstantiatedCubes.Count > 0)
			{
				HashSet<InstantiatedCube> cubes = _chunks[_maxChunkRoot].Cubes;
				HashSet<InstantiatedCube>.Enumerator enumerator = allInstantiatedCubes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.Current;
					if (!cubes.Contains(current))
					{
						_newDisconnectedCubes.Add(current);
						current.isConnected = false;
					}
					else
					{
						current.isConnected = true;
					}
				}
			}
			this.OnGraphInitialized(_newDisconnectedCubes);
		}

		internal void UpdateGraphOnCubeDeleted(InstantiatedCube cube)
		{
			RemoveInstantiatedCube(cube);
		}

		internal void UpdateGraphOnCubePlaced(InstantiatedCube cube)
		{
			AddInstantiatedCube(cube);
		}

		private void UpdateGraph(HashSet<InstantiatedCube> cubes)
		{
			_tempHashSet.UnionWith(cubes);
			uint num = 0u;
			InstantiatedCube instantiatedCube = _maxChunkRoot;
			while (_tempHashSet.Count > 0)
			{
				ChunkCpu chunkCpu = RecomputeChunk(instantiatedCube);
				if (chunkCpu.TotalCpu > num)
				{
					num = chunkCpu.TotalCpu;
					_maxChunkRoot = instantiatedCube;
				}
				_tempHashSet.ExceptWith(chunkCpu.Cubes);
				if (_tempHashSet.Count > 0)
				{
					instantiatedCube = FindFirst();
				}
			}
		}

		private void CreateGraph(HashSet<InstantiatedCube> cubes)
		{
			_tempHashSet.UnionWith(cubes);
			uint num = 0u;
			while (_tempHashSet.Count > 0)
			{
				InstantiatedCube instantiatedCube = FindFirst();
				ChunkCpu chunkCpu = ComputeChunk(instantiatedCube);
				if (chunkCpu.TotalCpu > num)
				{
					num = chunkCpu.TotalCpu;
					_maxChunkRoot = instantiatedCube;
				}
				_tempHashSet.ExceptWith(chunkCpu.Cubes);
			}
		}

		private InstantiatedCube FindFirst()
		{
			using (HashSet<InstantiatedCube>.Enumerator enumerator = _tempHashSet.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
				return null;
			}
		}

		private ChunkCpu ComputeChunk(InstantiatedCube root)
		{
			_cubesToProcess.Clear();
			_newConnectedCubes.FastClear();
			ChunkCpu chunkInstance = GetChunkInstance(root);
			_cubesToProcess.Enqueue(root.cubeNodeInstance);
			root.cubeNodeInstance.linkToChair = null;
			root.isConnected = true;
			LinkNodesToRoot(root, _cubesToProcess);
			return chunkInstance;
		}

		private void DebugPrint()
		{
			Debug.Log((object)("Num Chunks " + _chunks.Count));
			int num = 0;
			foreach (KeyValuePair<InstantiatedCube, ChunkCpu> chunk in _chunks)
			{
				Debug.Log((object)("Chunk " + num++ + " " + chunk.Value.Cubes.Count));
			}
			Debug.Log((object)_rootPerCube.Count);
		}

		private void RemoveInstantiatedCube(InstantiatedCube cube)
		{
			_newDisconnectedCubes.FastClear();
			_indices.FastClear();
			_newDisconnectedCubes = _destroyedCubesFinder.FindSeperatedCubes(cube, _indices);
			cube.cubeNodeInstance.BreakLinks();
			cube.cubeNodeInstance.BreakOriginalLinks();
			ComputeNewChunks(_chunksToProcess, _newDisconnectedCubes, _indices);
			InstantiatedCube a = UpdateCurrentChunkCubes(cube, _chunksToProcess);
			InstantiatedCube maxCpuChunkRoot = GetMaxCpuChunkRoot();
			if (maxCpuChunkRoot != _maxChunkRoot)
			{
				if (_chunksToProcess.Remove(maxCpuChunkRoot))
				{
					_newDisconnectedCubes.AddRange((ICollection<InstantiatedCube>)_chunks[_maxChunkRoot].Cubes);
					for (int num = _newDisconnectedCubes.get_Count() - 1; num >= 0; num--)
					{
						if (_chunks[maxCpuChunkRoot].Cubes.Contains(_newDisconnectedCubes.get_Item(num)))
						{
							_newDisconnectedCubes.get_Item(num).cubeNodeInstance.isDestroyed = false;
							_newDisconnectedCubes.UnorderedRemoveAt(num);
						}
					}
					HashSet<InstantiatedCube>.Enumerator enumerator = _chunks[maxCpuChunkRoot].Cubes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						InstantiatedCube current = enumerator.Current;
						current.isConnected = true;
					}
				}
				else
				{
					_newDisconnectedCubes.AddRange((ICollection<InstantiatedCube>)_chunks[_maxChunkRoot].Cubes);
					_newConnectedCubes.FastClear();
					_newConnectedCubes.AddRange((ICollection<InstantiatedCube>)_chunks[maxCpuChunkRoot].Cubes);
					SetCubesConnected(_newConnectedCubes, isConnected: true);
					this.OnCubesConnected(_newConnectedCubes);
				}
				_maxChunkRoot = maxCpuChunkRoot;
			}
			if (a == cube)
			{
				_chunks[cube].Cubes.Clear();
				_chunks.Remove(cube);
				if (cube == _maxChunkRoot)
				{
					_maxChunkRoot = null;
				}
			}
			_rootPerCube.Remove(cube);
			SetCubesConnected(_newDisconnectedCubes, isConnected: false);
			if (cube.isConnected)
			{
				this.OnCubesDisconnected(_newDisconnectedCubes);
			}
		}

		private InstantiatedCube UpdateCurrentChunkCubes(InstantiatedCube cube, HashSet<InstantiatedCube> chunksToProcess)
		{
			InstantiatedCube instantiatedCube = _rootPerCube[cube];
			ChunkCpu chunkCpu = _chunks[instantiatedCube];
			chunkCpu.Remove(cube);
			HashSet<InstantiatedCube>.Enumerator enumerator = chunksToProcess.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				chunkCpu.ExceptWith(_chunks[current]);
			}
			return instantiatedCube;
		}

		private void ComputeNewChunks(HashSet<InstantiatedCube> newChunkRoots, FasterList<InstantiatedCube> cubes, FasterList<uint> indices)
		{
			newChunkRoots.Clear();
			for (int i = 0; i < indices.get_Count(); i += 3)
			{
				InstantiatedCube instantiatedCube = cubes.get_Item((int)indices.get_Item(i));
				RecomputeChunk(instantiatedCube);
				newChunkRoots.Add(instantiatedCube);
			}
		}

		private ChunkCpu RecomputeChunk(InstantiatedCube root)
		{
			_newConnectedCubes.FastClear();
			ChunkCpu chunkInstance = GetChunkInstance(root);
			UpdateLinksToRoot(root.cubeNodeInstance, areCubesConnected: false);
			return chunkInstance;
		}

		private ChunkCpu GetChunkInstance(InstantiatedCube root)
		{
			ChunkCpu chunkCpu = new ChunkCpu();
			chunkCpu.Add(root);
			_chunks[root] = chunkCpu;
			_rootPerCube[root] = root;
			return chunkCpu;
		}

		private void UpdateLinksToRoot(CubeNodeInstance root, bool areCubesConnected)
		{
			_cubesToProcess.Clear();
			root.linkToChair = null;
			root.processed = true;
			InstantiatedCube instantiatedCube = root.instantiatedCube;
			instantiatedCube.isConnected = areCubesConnected;
			if (areCubesConnected)
			{
				_newConnectedCubes.Add(instantiatedCube);
			}
			_cubesToProcess.Enqueue(root);
			_allNodes.Add(root);
			while (_cubesToProcess.Count > 0)
			{
				CubeNodeInstance cubeNodeInstance = _cubesToProcess.Dequeue();
				FasterList<CubeNodeInstance> neighbours = cubeNodeInstance.GetNeighbours();
				for (int num = neighbours.get_Count() - 1; num >= 0; num--)
				{
					CubeNodeInstance cubeNodeInstance2 = neighbours.get_Item(num);
					InstantiatedCube instantiatedCube2 = cubeNodeInstance2.instantiatedCube;
					if (!cubeNodeInstance2.processed)
					{
						cubeNodeInstance2.processed = true;
						cubeNodeInstance2.linkToChair = cubeNodeInstance;
						_rootPerCube[instantiatedCube2] = root.instantiatedCube;
						_chunks[instantiatedCube].Add(instantiatedCube2);
						_cubesToProcess.Enqueue(cubeNodeInstance2);
						_allNodes.Add(cubeNodeInstance2);
						instantiatedCube2.isConnected = areCubesConnected;
						if (areCubesConnected)
						{
							_newConnectedCubes.Add(instantiatedCube2);
						}
					}
				}
			}
			for (int num2 = _allNodes.get_Count() - 1; num2 >= 0; num2--)
			{
				CubeNodeInstance cubeNodeInstance3 = _allNodes.get_Item(num2);
				cubeNodeInstance3.isDestroyed = false;
				cubeNodeInstance3.processed = false;
			}
			_allNodes.FastClear();
		}

		private InstantiatedCube GetMaxCpuChunkRoot()
		{
			uint totalCpu = _chunks[_maxChunkRoot].TotalCpu;
			InstantiatedCube result = _maxChunkRoot;
			Dictionary<InstantiatedCube, ChunkCpu>.Enumerator enumerator = _chunks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<InstantiatedCube, ChunkCpu> current = enumerator.Current;
				if (current.Value.TotalCpu > totalCpu || (_chunksToProcess.Contains(current.Key) && current.Value.TotalCpu >= totalCpu))
				{
					totalCpu = current.Value.TotalCpu;
					result = current.Key;
				}
			}
			return result;
		}

		private void SetCubesConnected(FasterList<InstantiatedCube> cubes, bool isConnected)
		{
			for (int i = 0; i < cubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = cubes.get_Item(i);
				instantiatedCube.isConnected = isConnected;
				instantiatedCube.cubeNodeInstance.isDestroyed = false;
			}
		}

		private void AddInstantiatedCube(InstantiatedCube cube)
		{
			_newDisconnectedCubes.FastClear();
			_newConnectedCubes.FastClear();
			ProcessCube(cube);
		}

		private void ProcessCube(InstantiatedCube currentCube)
		{
			_chunksToProcess.Clear();
			CubeNodeInstance cubeNodeInstance = currentCube.cubeNodeInstance;
			CubeNodeInstance connectedNode = GetConnectedNode(cubeNodeInstance);
			CubeNodeInstance[] array = _cubesToProcess.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				InstantiatedCube instantiatedCube = array[i].instantiatedCube;
				InstantiatedCube item = _rootPerCube[instantiatedCube];
				_chunksToProcess.Add(item);
			}
			if (connectedNode == null)
			{
				ProcessDisconnectedNodes(currentCube, cubeNodeInstance);
				return;
			}
			_newConnectedCubes.Add(currentCube);
			currentCube.isConnected = true;
			cubeNodeInstance.linkToChair = connectedNode;
			InstantiatedCube instantiatedCube2 = _rootPerCube[connectedNode.instantiatedCube];
			_rootPerCube[cubeNodeInstance.instantiatedCube] = instantiatedCube2;
			_chunks[instantiatedCube2].Add(currentCube);
			LinkNodesToRoot(instantiatedCube2, _cubesToProcess);
			HashSet<InstantiatedCube>.Enumerator enumerator = _chunksToProcess.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				_chunks[current].Cubes.Clear();
				_chunks.Remove(current);
			}
			this.OnCubesConnected(_newConnectedCubes);
		}

		private void ProcessDisconnectedNodes(InstantiatedCube currentCube, CubeNodeInstance currentNode)
		{
			ChunkCpu chunkInstance = GetChunkInstance(currentCube);
			currentNode.linkToChair = null;
			uint num = chunkInstance.TotalCpu;
			HashSet<InstantiatedCube>.Enumerator enumerator = _chunksToProcess.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				num += _chunks[current].TotalCpu;
				_chunks[current].Cubes.Clear();
				_chunks.Remove(current);
			}
			if (_maxChunkRoot == null || !_chunks.ContainsKey(_maxChunkRoot))
			{
				_maxChunkRoot = currentCube;
				currentCube.isConnected = true;
				_newConnectedCubes.Add(currentCube);
				this.OnCubesConnected(_newConnectedCubes);
			}
			else if (num > _chunks[_maxChunkRoot].TotalCpu)
			{
				HashSet<InstantiatedCube> cubes = _chunks[_maxChunkRoot].Cubes;
				HashSet<InstantiatedCube>.Enumerator enumerator2 = cubes.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					InstantiatedCube current2 = enumerator2.Current;
					current2.isConnected = false;
					_newDisconnectedCubes.Add(current2);
				}
				this.OnCubesDisconnected(_newDisconnectedCubes);
				_maxChunkRoot = currentCube;
				UpdateLinksToRoot(currentNode, areCubesConnected: true);
				this.OnCubesConnected(_newConnectedCubes);
			}
			else
			{
				UpdateLinksToRoot(currentNode, areCubesConnected: false);
				_newDisconnectedCubes.Add(currentCube);
				this.OnCubesDisconnected(_newDisconnectedCubes);
			}
		}

		private CubeNodeInstance GetConnectedNode(CubeNodeInstance currentNode)
		{
			_cubesToProcess.Clear();
			CubeNodeInstance result = null;
			List<ConnectionPoint> adjacentCubeLocations = currentNode.GetAdjacentCubeLocations();
			for (int i = 0; i < adjacentCubeLocations.Count; i++)
			{
				ConnectionPoint connectionPoint = adjacentCubeLocations[i];
				ConnectionPoint adjacentConnection;
				InstantiatedCube adjacentInstantiatedCube = GetAdjacentInstantiatedCube(currentNode, connectionPoint, out adjacentConnection);
				if (adjacentInstantiatedCube != null)
				{
					CubeNodeInstance cubeNodeInstance = adjacentInstantiatedCube.cubeNodeInstance;
					MachineGraph.MakeLink(currentNode, cubeNodeInstance, connectionPoint);
					MachineGraph.MakeLink(cubeNodeInstance, currentNode, adjacentConnection);
					if (cubeNodeInstance.instantiatedCube.isConnected)
					{
						result = cubeNodeInstance;
					}
					else
					{
						_cubesToProcess.Enqueue(cubeNodeInstance);
					}
				}
			}
			return result;
		}

		private void LinkNodesToRoot(InstantiatedCube root, Queue<CubeNodeInstance> cubesToProcess)
		{
			visitedNodes.Clear();
			while (cubesToProcess.Count > 0)
			{
				CubeNodeInstance cubeNodeInstance = cubesToProcess.Dequeue();
				List<ConnectionPoint> adjacentCubeLocations = cubeNodeInstance.GetAdjacentCubeLocations();
				bool flag = false;
				for (int i = 0; i < adjacentCubeLocations.Count; i++)
				{
					ConnectionPoint connectionPoint = adjacentCubeLocations[i];
					ConnectionPoint adjacentConnection;
					InstantiatedCube adjacentInstantiatedCube = GetAdjacentInstantiatedCube(cubeNodeInstance, connectionPoint, out adjacentConnection);
					if (!(adjacentInstantiatedCube != null))
					{
						continue;
					}
					CubeNodeInstance cubeNodeInstance2 = adjacentInstantiatedCube.cubeNodeInstance;
					if (cubeNodeInstance2.instantiatedCube.isConnected)
					{
						if (!flag)
						{
							cubeNodeInstance.instantiatedCube.isConnected = true;
							cubeNodeInstance.isDestroyed = false;
							_newConnectedCubes.Add(cubeNodeInstance.instantiatedCube);
							_rootPerCube[cubeNodeInstance.instantiatedCube] = root;
							_chunks[root].Add(cubeNodeInstance.instantiatedCube);
							cubeNodeInstance.linkToChair = cubeNodeInstance2;
							flag = true;
						}
						if (!cubeNodeInstance.GetNeighbours().Contains(cubeNodeInstance2))
						{
							MachineGraph.MakeLink(cubeNodeInstance, cubeNodeInstance2, connectionPoint);
							MachineGraph.MakeLink(cubeNodeInstance2, cubeNodeInstance, adjacentConnection);
						}
					}
					else if (!visitedNodes.Contains(cubeNodeInstance2))
					{
						visitedNodes.Add(cubeNodeInstance2);
						cubesToProcess.Enqueue(cubeNodeInstance2);
					}
				}
			}
		}

		private InstantiatedCube GetAdjacentInstantiatedCube(CubeNodeInstance cubeNode, ConnectionPoint connection, out ConnectionPoint adjacentConnection)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			InstantiatedCube instantiatedCube = cubeNode.instantiatedCube;
			if (instantiatedCube.IsDirectionSelectable(Quaternion.get_identity(), connection.direction, connection.offset))
			{
				Quaternion val = CubeData.IndexToQuat(instantiatedCube.rotationIndex);
				Vector3 direction = connection.direction;
				Vector3 val2 = val * direction;
				Vector3 val3 = val * connection.offset;
				Vector3 val4 = instantiatedCube.gridPos.ToVector3() + val2 + val3;
				MachineCell cellAt = machineMap.GetCellAt(new Byte3(val4));
				if (cellAt != null)
				{
					Transform transform = cellAt.gameObject.get_transform();
					Vector3 val5 = GridScaleUtility.WorldToGrid(transform.get_localPosition(), TargetType.Player).ToVector3();
					Vector3 localOffset = Quaternion.Inverse(transform.get_rotation()) * (val4 - val5);
					InstantiatedCube info = cellAt.info;
					if (info.persistentCubeData.IsDirectionSelectable(CubeData.IndexToQuat(info.rotationIndex), -val2, localOffset, out adjacentConnection))
					{
						return info;
					}
				}
			}
			adjacentConnection = null;
			return null;
		}
	}
}
