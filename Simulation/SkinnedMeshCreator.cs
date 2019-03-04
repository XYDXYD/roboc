using Simulation.Hardware;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class SkinnedMeshCreator
	{
		private Dictionary<string, int> SupportedShaders = new Dictionary<string, int>
		{
			{
				"Robocraft/Robocraft standard Component Skinned",
				0
			},
			{
				"Robocraft/Component/Electroplate Skinned",
				1
			}
		};

		private List<Vector3> _verts = new List<Vector3>();

		private List<Vector3> _norms = new List<Vector3>();

		private List<Vector4> _tangents = new List<Vector4>();

		private List<Vector3> _uvs = new List<Vector3>();

		private List<int>[] _meshesReminder = new List<int>[2]
		{
			new List<int>(),
			new List<int>()
		};

		private List<MeshFilter> _allFilters = new List<MeshFilter>(15);

		private List<MeshFilter> _filtersToMerge = new List<MeshFilter>(15);

		private List<Renderer> _renderersToMerge = new List<Renderer>(15);

		private Dictionary<uint, Mesh> _meshPerCubeType = new Dictionary<uint, Mesh>();

		[Inject]
		internal IEntityFactory enginesRoot
		{
			private get;
			set;
		}

		public bool TryApplySkinnedMesh(uint cubeId, GameObject g, out float meshScale)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			_filtersToMerge.Clear();
			_allFilters.Clear();
			_renderersToMerge.Clear();
			g.GetComponentsInChildren<MeshFilter>(true, _allFilters);
			float num;
			if (_allFilters.Count > 0)
			{
				Vector3 localScale = _allFilters[0].get_transform().get_localScale();
				num = localScale.x;
			}
			else
			{
				num = 1f;
			}
			meshScale = num;
			Material[] array = null;
			for (int i = 0; i < _allFilters.Count; i++)
			{
				MeshFilter val = _allFilters[i];
				MeshRenderer component = val.GetComponent<MeshRenderer>();
				if (component == null)
				{
					continue;
				}
				int num2 = 0;
				for (int j = 0; j < component.get_sharedMaterials().Length; j++)
				{
					Material val2 = component.get_sharedMaterials()[j];
					if (SupportedShaders.ContainsKey(val2.get_shader().get_name()))
					{
						num2++;
					}
				}
				if (num2 == component.get_sharedMaterials().Length)
				{
					if (array == null || component.get_sharedMaterials().Length > array.Length)
					{
						array = component.get_sharedMaterials();
					}
					_filtersToMerge.Add(val);
					_renderersToMerge.Add(component);
				}
			}
			if (_filtersToMerge.Count > 1)
			{
				if (!_meshPerCubeType.TryGetValue(cubeId, out Mesh value))
				{
					value = StartMerge(g, _filtersToMerge, _renderersToMerge);
					_meshPerCubeType.Add(cubeId, value);
				}
				MeshRenderer val3 = g.AddComponent<MeshRenderer>();
				val3.set_sharedMaterials(array);
				MeshFilter val4 = g.AddComponent<MeshFilter>();
				val4.set_sharedMesh(value);
				SkinnedMeshImplementor skinnedMeshImplementor = g.AddComponent<SkinnedMeshImplementor>();
				ComponentTransformImplementor componentTransformImplementor = g.GetComponent<ComponentTransformImplementor>();
				if (componentTransformImplementor == null)
				{
					componentTransformImplementor = g.AddComponent<ComponentTransformImplementor>();
				}
				for (int k = 0; k < _filtersToMerge.Count; k++)
				{
					MeshFilter val5 = _filtersToMerge[k];
					skinnedMeshImplementor.AddBone(val5.get_transform());
					Object.Destroy(val5);
				}
				for (int l = 0; l < _renderersToMerge.Count; l++)
				{
					Renderer val6 = _renderersToMerge[l];
					Object.DestroyImmediate(val6);
				}
				BuildSkinnedMeshEntity(g, skinnedMeshImplementor, componentTransformImplementor);
				return true;
			}
			return false;
		}

		private void BuildSkinnedMeshEntity(GameObject gameObject, SkinnedMeshImplementor implementor, ComponentTransformImplementor componentRigidbodyTransformImplementor)
		{
			enginesRoot.BuildEntity<SkinnedMeshEntityDescriptor>(gameObject.GetInstanceID(), new object[2]
			{
				implementor,
				componentRigidbodyTransformImplementor
			});
		}

		private Mesh StartMerge(GameObject gameObject, List<MeshFilter> meshFilters, List<Renderer> renderers)
		{
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Expected O, but got Unknown
			ClearResources();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < meshFilters.Count; i++)
			{
				Mesh sharedMesh = meshFilters[i].get_sharedMesh();
				Renderer val = renderers[i];
				int num4 = val.get_sharedMaterials().Length;
				num = Mathf.Max(num, num4);
				for (int j = 0; j < num4; j++)
				{
					int num5 = SupportedShaders[val.get_sharedMaterials()[j].get_shader().get_name()];
					List<int> list = _meshesReminder[num5];
					int[] triangles = sharedMesh.GetTriangles(j);
					for (int k = 0; k < triangles.Length; k++)
					{
						list.Add(triangles[k] + num2);
					}
				}
				Vector3[] vertices = sharedMesh.get_vertices();
				_verts.AddRange(vertices);
				num2 += vertices.Length;
				vertices = sharedMesh.get_normals();
				_norms.AddRange(vertices);
				Vector4[] tangents = sharedMesh.get_tangents();
				_tangents.AddRange(tangents);
				Vector2[] uv = sharedMesh.get_uv();
				for (int l = 0; l < uv.Length; l++)
				{
					Vector2 val2 = uv[l];
					_uvs.Add(new Vector3(val2.x, val2.y, (float)num3));
				}
				num3++;
			}
			Mesh val3 = new Mesh();
			val3.set_name(gameObject.get_name());
			val3.SetVertices(_verts);
			val3.SetNormals(_norms);
			val3.SetTangents(_tangents);
			val3.SetUVs(0, _uvs);
			val3.set_subMeshCount(num);
			for (int m = 0; m < num; m++)
			{
				val3.SetTriangles(_meshesReminder[m], m);
			}
			return val3;
		}

		private void ClearResources()
		{
			for (int i = 0; i < _meshesReminder.Length; i++)
			{
				_meshesReminder[i].Clear();
			}
			_verts.Clear();
			_norms.Clear();
			_tangents.Clear();
			_uvs.Clear();
		}
	}
}
