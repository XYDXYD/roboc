using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ChunkMeshUpdater : MonoBehaviour
{
	private struct IndexRange
	{
		public int indicesStart;

		public int indicesSize;

		public IndexRange(int start, int size)
		{
			indicesStart = start;
			indicesSize = size;
		}
	}

	internal Dictionary<int, List<MachineMesh.MeshInstance>> CubeVertices;

	internal Dictionary<int, MachineMesh.FilterData> CombinedMeshesData;

	internal List<Object> ResourcesToRelease;

	internal GameObject rendererGO;

	private static HashSet<int> toUpdate = new HashSet<int>();

	private Dictionary<Mesh, Color32[]> colorsPerMesh = new Dictionary<Mesh, Color32[]>();

	public ChunkMeshUpdater()
		: this()
	{
	}

	private void OnDestroy()
	{
		if (ResourcesToRelease != null)
		{
			for (int i = 0; i < ResourcesToRelease.Count; i++)
			{
				Object.DestroyImmediate(ResourcesToRelease[i], true);
			}
		}
	}

	internal void UpdateDestroyTextureInEditor(FasterList<InstantiatedCube> cubesToRemove)
	{
		toUpdate.Clear();
		for (int i = 0; i < cubesToRemove.get_Count(); i++)
		{
			EnableCube(cubesToRemove.get_Item(i), enabled: false);
			CubeVertices.Remove(cubesToRemove.get_Item(i).gridPos.GetHashCode());
		}
		UpdateDestroyTexture();
	}

	internal void UpdateDestroyTexture(FasterList<InstantiatedCube> cubesToRemove, FasterList<InstantiatedCube> cubesToRespawn)
	{
		toUpdate.Clear();
		if (cubesToRespawn != null)
		{
			for (int num = cubesToRespawn.get_Count() - 1; num >= 0; num--)
			{
				EnableCube(cubesToRespawn.get_Item(num), enabled: true);
			}
		}
		if (cubesToRemove != null)
		{
			for (int num2 = cubesToRemove.get_Count() - 1; num2 >= 0; num2--)
			{
				EnableCube(cubesToRemove.get_Item(num2), enabled: false);
			}
		}
		UpdateDestroyTexture();
	}

	internal void UpdateColorsInEditor(FasterList<InstantiatedCube> invalidCubesToUpdate)
	{
		toUpdate.Clear();
		if (invalidCubesToUpdate != null)
		{
			UpdateRedColor(invalidCubesToUpdate);
		}
		UpdateDamageTexture();
	}

	internal void UpdateDamageTexture(FasterList<InstantiatedCube> damagedCubes, FasterList<InstantiatedCube> spawnedCubes)
	{
		toUpdate.Clear();
		if (damagedCubes != null)
		{
			UpdateDamageTexture(damagedCubes);
		}
		if (spawnedCubes != null)
		{
			UpdateDamageTexture(spawnedCubes);
		}
		UpdateDamageTexture();
	}

	private void UpdateRedColor(FasterList<InstantiatedCube> cubes)
	{
		for (int num = cubes.get_Count() - 1; num >= 0; num--)
		{
			InstantiatedCube instantiatedCube = cubes.get_Item(num);
			int hashCode = instantiatedCube.gridPos.GetHashCode();
			byte a = 0;
			if (instantiatedCube.isRed > 0)
			{
				a = byte.MaxValue;
			}
			if (CubeVertices.TryGetValue(hashCode, out List<MachineMesh.MeshInstance> value))
			{
				for (int num2 = value.Count - 1; num2 >= 0; num2--)
				{
					MachineMesh.MeshInstance meshInstance = value[num2];
					MachineMesh.FilterData filterData = CombinedMeshesData[meshInstance.combinedMeshHash];
					Color32[] damageColors = filterData.damageColors;
					toUpdate.Add(meshInstance.combinedMeshHash);
					int num3 = meshInstance.textureOffset / CubeFaceExtensions.NumberOfDirections();
					damageColors[num3].a = a;
				}
			}
		}
	}

	private void UpdateDestroyTexture()
	{
		HashSet<int>.Enumerator enumerator = toUpdate.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MachineMesh.FilterData filterData = CombinedMeshesData[enumerator.Current];
			filterData.destroyIndicesTexture.SetPixels32(filterData.originalColors);
			filterData.destroyIndicesTexture.Apply(false, false);
		}
	}

	private void UpdateDamageTexture()
	{
		HashSet<int>.Enumerator enumerator = toUpdate.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MachineMesh.FilterData filterData = CombinedMeshesData[enumerator.Current];
			filterData.damageTexture.SetPixels32(filterData.damageColors);
			filterData.damageTexture.Apply(false, false);
		}
	}

	private void EnableCube(InstantiatedCube cubeToUpdate, bool enabled)
	{
		int hashCode = cubeToUpdate.gridPos.GetHashCode();
		if (CubeVertices.TryGetValue(hashCode, out List<MachineMesh.MeshInstance> value))
		{
			SetNeighboursEnabled(cubeToUpdate, !enabled);
			SetCubeEnabled(cubeToUpdate, value, enabled);
		}
	}

	private void SetCubeEnabled(InstantiatedCube cubeToUpdate, List<MachineMesh.MeshInstance> meshInstances, bool enabled)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		CubeNodeInstance cubeNodeInstance = cubeToUpdate.cubeNodeInstance;
		for (int num = meshInstances.Count - 1; num >= 0; num--)
		{
			MachineMesh.MeshInstance meshInstance = meshInstances[num];
			MachineMesh.FilterData filterData = CombinedMeshesData[meshInstance.combinedMeshHash];
			Color32[] originalColors = filterData.originalColors;
			toUpdate.Add(meshInstance.combinedMeshHash);
			int textureOffset = meshInstance.textureOffset;
			byte a = byte.MaxValue;
			if (enabled)
			{
				a = 0;
			}
			originalColors[textureOffset].a = a;
			originalColors[textureOffset + 1].a = a;
			originalColors[textureOffset + 2].a = a;
			originalColors[textureOffset + 3].a = a;
			originalColors[textureOffset + 4].a = a;
			originalColors[textureOffset + 5].a = a;
			originalColors[textureOffset + 6].a = a;
			if (enabled)
			{
				FasterList<CubeNodeInstance> originalNeighbours = cubeNodeInstance.GetOriginalNeighbours();
				for (int num2 = originalNeighbours.get_Count() - 1; num2 >= 0; num2--)
				{
					CubeNodeInstance cubeNodeInstance2 = originalNeighbours.get_Item(num2);
					InstantiatedCube instantiatedCube = cubeNodeInstance2.instantiatedCube;
					Quaternion val = CubeData.IndexToQuat(cubeToUpdate.rotationIndex);
					ConnectionPoint connectionPointOfNeighbour = cubeNodeInstance.GetConnectionPointOfNeighbour(cubeNodeInstance2);
					Vector3 val2 = val * connectionPointOfNeighbour.direction;
					int offsetFromDirection = (int)MachineUtility.GetOffsetFromDirection(val2);
					if (!instantiatedCube.isDestroyed)
					{
						Vector3 direction = -val2;
						Quaternion rotation = CubeData.IndexToQuat(instantiatedCube.rotationIndex);
						if (instantiatedCube.persistentCubeData.IsDirectionOccluding(rotation, direction))
						{
							originalColors[textureOffset + offsetFromDirection].a = byte.MaxValue;
						}
					}
				}
			}
		}
	}

	private void SetNeighboursEnabled(InstantiatedCube cubeToUpdate, bool enabled)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		CubeNodeInstance cubeNodeInstance = cubeToUpdate.cubeNodeInstance;
		FasterList<CubeNodeInstance> originalNeighbours = cubeToUpdate.cubeNodeInstance.GetOriginalNeighbours();
		for (int num = originalNeighbours.get_Count() - 1; num >= 0; num--)
		{
			CubeNodeInstance cubeNodeInstance2 = originalNeighbours.get_Item(num);
			InstantiatedCube instantiatedCube = cubeNodeInstance2.instantiatedCube;
			if (!instantiatedCube.isDestroyed && CubeVertices.TryGetValue(instantiatedCube.gridPos.GetHashCode(), out List<MachineMesh.MeshInstance> value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					MachineMesh.MeshInstance meshInstance = value[i];
					MachineMesh.FilterData filterData = CombinedMeshesData[meshInstance.combinedMeshHash];
					Color32[] originalColors = filterData.originalColors;
					if (originalColors[meshInstance.textureOffset + 6].a != byte.MaxValue)
					{
						toUpdate.Add(meshInstance.combinedMeshHash);
						Quaternion val = CubeData.IndexToQuat(instantiatedCube.rotationIndex);
						ConnectionPoint connectionPointOfNeighbour = cubeNodeInstance2.GetConnectionPointOfNeighbour(cubeNodeInstance);
						Vector3 direction = val * connectionPointOfNeighbour.direction;
						int offsetFromDirection = (int)MachineUtility.GetOffsetFromDirection(direction);
						if (enabled)
						{
							originalColors[meshInstance.textureOffset + offsetFromDirection].a = 0;
						}
						else if (cubeToUpdate.persistentCubeData.IsDirectionOccluding(CubeData.IndexToQuat(cubeToUpdate.rotationIndex), ((Int3)cubeNodeInstance2.instantiatedCube.gridPos - (Int3)cubeToUpdate.gridPos).ToVector3()))
						{
							originalColors[meshInstance.textureOffset + offsetFromDirection].a = byte.MaxValue;
						}
					}
				}
			}
		}
	}

	private void UpdateDamageTexture(FasterList<InstantiatedCube> damagedCubes)
	{
		for (int num = damagedCubes.get_Count() - 1; num >= 0; num--)
		{
			UpdateDamageColors(damagedCubes.get_Item(num));
		}
	}

	private void UpdateDamageColors(InstantiatedCube cubeToUpdate)
	{
		int hashCode = cubeToUpdate.gridPos.GetHashCode();
		int health = cubeToUpdate.health;
		int totalHealth = cubeToUpdate.totalHealth;
		float num = (float)health / (float)totalHealth;
		if (health != totalHealth)
		{
			num = Math.Min(num, 0.8f);
		}
		if (CubeVertices.TryGetValue(hashCode, out List<MachineMesh.MeshInstance> value))
		{
			for (int num2 = value.Count - 1; num2 >= 0; num2--)
			{
				MachineMesh.MeshInstance meshInstance = value[num2];
				MachineMesh.FilterData filterData = CombinedMeshesData[meshInstance.combinedMeshHash];
				Color32[] damageColors = filterData.damageColors;
				toUpdate.Add(meshInstance.combinedMeshHash);
				int num3 = meshInstance.textureOffset / CubeFaceExtensions.NumberOfDirections();
				byte a = (byte)(num * 255f);
				damageColors[num3].a = a;
			}
		}
	}

	internal void UpdateMainColor(InstantiatedCube cubeToUpdate)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		int hashCode = cubeToUpdate.gridPos.GetHashCode();
		if (!CubeVertices.TryGetValue(hashCode, out List<MachineMesh.MeshInstance> value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			MachineMesh.MeshInstance meshInstance = value[i];
			Color32[] colors = meshInstance.combinedMesh.get_sharedMesh().get_colors32();
			int num = cubeToUpdate.paletteIndex * 2;
			Color val = default(Color);
			val._002Ector(((float)num + 0.5f) / MachineMesh.CombineMeshes.ColourTextureWidth, ((float)num + 1.5f) / MachineMesh.CombineMeshes.ColourTextureWidth, ((float)meshInstance.colorIndex + 0.5f) / MachineMesh.CombineMeshes.ColourTextureWidth, 0f);
			for (int j = meshInstance.vertexOffset; j < meshInstance.vertexOffset + meshInstance.mesh.get_vertexCount(); j++)
			{
				colors[j] = Color32.op_Implicit(val);
			}
			meshInstance.combinedMesh.get_sharedMesh().set_colors32(colors);
		}
	}

	internal void UpdateMainColor(FasterList<InstantiatedCube> cubes)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		colorsPerMesh.Clear();
		for (int i = 0; i < cubes.get_Count(); i++)
		{
			int hashCode = cubes.get_Item(i).gridPos.GetHashCode();
			if (!CubeVertices.TryGetValue(hashCode, out List<MachineMesh.MeshInstance> value))
			{
				continue;
			}
			for (int j = 0; j < value.Count; j++)
			{
				MachineMesh.MeshInstance meshInstance = value[j];
				Mesh sharedMesh = meshInstance.combinedMesh.get_sharedMesh();
				if (!colorsPerMesh.TryGetValue(sharedMesh, out Color32[] value2))
				{
					value2 = sharedMesh.get_colors32();
					colorsPerMesh.Add(sharedMesh, value2);
				}
				int num = cubes.get_Item(i).paletteIndex * 2;
				Color val = default(Color);
				val._002Ector(((float)num + 0.5f) / MachineMesh.CombineMeshes.ColourTextureWidth, ((float)num + 1.5f) / MachineMesh.CombineMeshes.ColourTextureWidth, ((float)meshInstance.colorIndex + 0.5f) / MachineMesh.CombineMeshes.ColourTextureWidth, 0f);
				for (int k = meshInstance.vertexOffset; k < meshInstance.vertexOffset + meshInstance.mesh.get_vertexCount(); k++)
				{
					value2[k] = Color32.op_Implicit(val);
				}
			}
		}
		foreach (KeyValuePair<Mesh, Color32[]> item in colorsPerMesh)
		{
			item.Key.set_colors32(item.Value);
		}
	}
}
