using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class MachineCluster
	{
		public const float PLAYER_TOLERANCE = 1.5f;

		public const float ENEMY_TOLERANCE = 4f;

		public const float MENEMY_TOLERANCE = 4f;

		public const float TEAMBASE_TOLERANCE = 4f;

		public const string CLUSTER_COLLIDER_NAME = "ClusterCollider";

		public const int PREALLOCATED_COLLIDERS = 1024;

		private static PhysicMaterial _boxMaterial = Resources.Load("BasicCubeSurface") as PhysicMaterial;

		private Dictionary<IClusteredColliderNode, BoxCollider> _colliders;

		private FasterList<NodeBoxCollider> _collidersList;

		private Dictionary<ColliderNode, bool> _colliderNodesToRecompute;

		private int _layer = -1;

		private float _tolerance = 1.5f;

		private Dictionary<int, ColliderLeaf> _leaves;

		private IClusteredColliderNode _root;

		private ObjectPool<BoxCollider> _pool;

		private Func<BoxCollider> _generateNewCollider;

		private Transform _parent;

		private int _computationID;

		[CompilerGenerated]
		private static Func<BoxCollider> _003C_003Ef__mg_0024cache0;

		public IClusteredColliderNode root => _root;

		public MachineCluster(IClusteredColliderNode rootNode, Dictionary<int, ColliderLeaf> leaves, MachineClusterPool pool)
		{
			_leaves = leaves;
			_root = rootNode;
			_pool = pool;
			_generateNewCollider = GenerateNewCollider;
			_colliders = new Dictionary<IClusteredColliderNode, BoxCollider>(leaves.Count);
			_collidersList = new FasterList<NodeBoxCollider>(leaves.Count);
			_colliderNodesToRecompute = new Dictionary<ColliderNode, bool>(leaves.Count);
		}

		public void RemoveLeaves(FasterList<InstantiatedCube> deadCubes)
		{
			_colliderNodesToRecompute.Clear();
			for (int num = deadCubes.get_Count() - 1; num >= 0; num--)
			{
				int hashCode = deadCubes.get_Item(num).gridPos.GetHashCode();
				ColliderLeaf value = null;
				if (_leaves.TryGetValue(hashCode, out value))
				{
					ColliderNode colliderNode = value.parent.DisableChild(value);
					if (colliderNode != null)
					{
						_colliderNodesToRecompute[colliderNode] = true;
					}
				}
				else
				{
					Console.LogError("Leave not found");
				}
			}
			Dictionary<ColliderNode, bool>.KeyCollection keys = _colliderNodesToRecompute.Keys;
			Dictionary<ColliderNode, bool>.KeyCollection.Enumerator enumerator = keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ColliderNode current = enumerator.Current;
				current.RecomputeError(_computationID);
			}
			_computationID++;
		}

		public void AddLeaves(FasterList<InstantiatedCube> healedCubes)
		{
			for (int num = healedCubes.get_Count() - 1; num >= 0; num--)
			{
				int hashCode = healedCubes.get_Item(num).gridPos.GetHashCode();
				ColliderLeaf value = null;
				if (_leaves.TryGetValue(hashCode, out value))
				{
					value.parent.EnableChild(value);
				}
				else
				{
					Console.LogError("Leave not found");
				}
			}
			for (int num2 = healedCubes.get_Count() - 1; num2 >= 0; num2--)
			{
				int hashCode2 = healedCubes.get_Item(num2).gridPos.GetHashCode();
				_leaves[hashCode2].parent.RecomputeError(_computationID);
			}
			_computationID++;
		}

		public void Configure(int layer, float tolerance)
		{
			_layer = layer;
			_tolerance = tolerance;
		}

		public void CreateGameObjectStructure(Transform parent, FasterList<Collider> newColliders, FasterList<Collider> removedColliders)
		{
			_parent = parent;
			for (int num = _collidersList.get_Count() - 1; num >= 0; num--)
			{
				NodeBoxCollider nodeBoxCollider = _collidersList.get_Item(num);
				nodeBoxCollider.gameobject.set_layer(GameLayers.IGNORE_RAYCAST);
			}
			Walk(_root, _tolerance);
			for (int num2 = _collidersList.get_Count() - 1; num2 >= 0; num2--)
			{
				NodeBoxCollider nodeBoxCollider2 = _collidersList.get_Item(num2);
				if (nodeBoxCollider2.gameobject.get_layer() == GameLayers.IGNORE_RAYCAST)
				{
					nodeBoxCollider2.gameobject.get_transform().set_parent(null);
					_pool.Recycle(nodeBoxCollider2.collider, 0);
					_collidersList.UnorderedRemoveAt(num2);
					_colliders.Remove(nodeBoxCollider2.node);
					removedColliders.Add(nodeBoxCollider2.collider);
					nodeBoxCollider2.node = null;
				}
			}
			for (int i = 0; i < _collidersList.get_Count(); i++)
			{
				NodeBoxCollider nodeBoxCollider3 = _collidersList.get_Item(i);
				newColliders.Add(nodeBoxCollider3.collider);
			}
		}

		internal FasterList<NodeBoxCollider> GetColliders()
		{
			return _collidersList;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private void OnNode(IClusteredColliderNode node)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			Bounds bounds = node.bounds;
			Vector3 size = bounds.get_size();
			float num = size.x * size.y * size.z;
			if (num > 0f)
			{
				if (!_colliders.TryGetValue(node, out BoxCollider value))
				{
					BoxCollider val = GenerateCollider(node.bounds);
					_collidersList.Add(new NodeBoxCollider(node, val));
					_colliders[node] = val;
				}
				else
				{
					value.get_gameObject().set_layer(_layer);
				}
			}
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private BoxCollider GenerateCollider(Bounds b)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			BoxCollider val = _pool.Use(0, _generateNewCollider);
			GameObject gameObject = val.get_gameObject();
			Transform transform = gameObject.get_transform();
			if (_parent != transform.get_parent())
			{
				transform.set_parent(_parent);
				transform.set_localRotation(Quaternion.get_identity());
			}
			gameObject.set_layer(_layer);
			transform.set_localPosition(b.get_center());
			if (val.get_size() != b.get_size())
			{
				val.set_size(b.get_size());
			}
			return val;
		}

		public static BoxCollider GenerateNewCollider()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			BoxCollider val = new GameObject("ClusteredCollider").AddComponent<BoxCollider>();
			val.get_gameObject().set_layer(GameLayers.IGNORE_RAYCAST);
			val.set_material(_boxMaterial);
			return val;
		}

		private int Walk(IClusteredColliderNode n, float tolerance)
		{
			IClusteredColliderNode clusteredColliderNode = null;
			bool flag = true;
			int num = 0;
			while (NodeIsEnabled(n))
			{
				if (flag && NodeIsEnabled(n.left) && n.error > (double)tolerance && !n.isLeaf)
				{
					n = n.left;
					continue;
				}
				if (NodeIsEnabled(n.right) && n.right != clusteredColliderNode && n.error > (double)tolerance && !n.isLeaf)
				{
					n = n.right;
					flag = true;
					continue;
				}
				if (n.error <= (double)tolerance || n.isLeaf)
				{
					num++;
					OnNode(n);
				}
				clusteredColliderNode = n;
				n = n.parent;
				flag = false;
			}
			return num;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private bool NodeIsEnabled(IClusteredColliderNode node)
		{
			return node != null && node.isEnabled;
		}
	}
}
