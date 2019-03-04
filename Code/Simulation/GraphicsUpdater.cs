using Svelto.DataStructures;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class GraphicsUpdater : ILateTickable, ITickableBase
	{
		private class DestructionInfo
		{
			public bool pendingUpdate;

			public ChunkMeshUpdater meshUpdater;

			public FasterList<InstantiatedCube> destroyedCubes;

			public FasterList<InstantiatedCube> spawnedCubes;

			public FasterList<InstantiatedCube> damagedCubes;

			public DestructionInfo(GameObject g)
			{
				destroyedCubes = new FasterList<InstantiatedCube>();
				damagedCubes = new FasterList<InstantiatedCube>();
				spawnedCubes = new FasterList<InstantiatedCube>();
				meshUpdater = g.GetComponent<ChunkMeshUpdater>();
				pendingUpdate = false;
			}
		}

		private Dictionary<int, DestructionInfo> _lateUpdateInfo = new Dictionary<int, DestructionInfo>();

		public void LateTick(float deltaTime)
		{
			Dictionary<int, DestructionInfo>.Enumerator enumerator = _lateUpdateInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DestructionInfo value = enumerator.Current.Value;
				if (value.pendingUpdate)
				{
					UpdateGraphics(value);
					value.pendingUpdate = false;
				}
			}
		}

		public void RunGraphicsUpdaterOnDestroy(DestructionData data)
		{
			DestructionInfo destructionInfo = GetDestructionInfo(data.hitRigidbody.get_gameObject());
			destructionInfo.destroyedCubes.AddRange(data.destroyedCubes);
			destructionInfo.damagedCubes.AddRange(data.damagedCubes);
			destructionInfo.pendingUpdate = true;
		}

		public void RunGraphicsUpdaterOnHeal(GameObject g, FasterList<InstantiatedCube> respawnedCubes, FasterList<InstantiatedCube> healedCubes)
		{
			DestructionInfo destructionInfo = GetDestructionInfo(g);
			destructionInfo.spawnedCubes.AddRange(respawnedCubes);
			destructionInfo.damagedCubes.AddRange(healedCubes);
			destructionInfo.pendingUpdate = true;
		}

		private DestructionInfo GetDestructionInfo(GameObject g)
		{
			if (!_lateUpdateInfo.TryGetValue(g.GetInstanceID(), out DestructionInfo value))
			{
				DestructionInfo destructionInfo = new DestructionInfo(g);
				_lateUpdateInfo[g.GetInstanceID()] = destructionInfo;
				return destructionInfo;
			}
			return value;
		}

		private void UpdateGraphics(DestructionInfo destructionInfo)
		{
			FasterList<InstantiatedCube> destroyedCubes = destructionInfo.destroyedCubes;
			FasterList<InstantiatedCube> damagedCubes = destructionInfo.damagedCubes;
			FasterList<InstantiatedCube> spawnedCubes = destructionInfo.spawnedCubes;
			ChunkMeshUpdater meshUpdater = destructionInfo.meshUpdater;
			if ((damagedCubes.get_Count() > 0 || spawnedCubes.get_Count() > 0) && meshUpdater != null)
			{
				meshUpdater.UpdateDamageTexture(damagedCubes, spawnedCubes);
			}
			if ((destroyedCubes.get_Count() > 0 || spawnedCubes.get_Count() > 0) && meshUpdater != null)
			{
				meshUpdater.UpdateDestroyTexture(destroyedCubes, spawnedCubes);
			}
			destroyedCubes.FastClear();
			spawnedCubes.FastClear();
			damagedCubes.FastClear();
		}
	}
}
