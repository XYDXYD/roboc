using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class MachineMesh
{
	internal sealed class MeshInstance
	{
		public Mesh mesh;

		public MeshFilter combinedMesh;

		public int combinedMeshHash;

		public Quaternion rotation;

		public int subMeshIndex;

		public Matrix4x4 transform;

		public int textureOffset;

		public int cubeID;

		public int vertexOffset;

		public int colorIndex;
	}

	internal struct FilterData
	{
		public Color32[] originalColors;

		public Color32[] damageColors;

		public Color32[] mainColors;

		public Texture2D mainTexture;

		public Texture2D destroyIndicesTexture;

		public Texture2D damageTexture;
	}

	internal struct CubeBatchData
	{
		public Transform root;

		public Quaternion rotation;

		public Vector3 position;

		public byte paletteIndex;

		public MeshFilter[] filters;

		public int gridHash;
	}

	internal sealed class MeshCombineUtility
	{
		private struct MeshAttribute
		{
			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector4[] tangents;

			public Vector2[] uv;

			public Color[] colors;
		}

		private static Dictionary<int, List<int>> _meshesReminder = new Dictionary<int, List<int>>();

		private static Dictionary<int, MeshAttribute> _meshesAttributes = new Dictionary<int, MeshAttribute>();

		private static Vector3 CalculateFaceDirection(ref Color color, ref Quaternion rotation)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			if (color.r != 0f)
			{
				if (color.r == 1f)
				{
					return rotation * Vector3.get_up();
				}
				return rotation * Vector3.get_down();
			}
			if (color.g != 0f)
			{
				if (color.g == 1f)
				{
					return rotation * Vector3.get_forward();
				}
				return rotation * Vector3.get_back();
			}
			if (color.b != 0f)
			{
				if (color.b == 1f)
				{
					return rotation * Vector3.get_right();
				}
				return rotation * Vector3.get_left();
			}
			return Vector3.get_one();
		}

		private static CubeFace GenerateSideIndexFromVector(Vector3 dir)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (dir == Vector3.get_up())
			{
				return CubeFace.Up;
			}
			if (dir == Vector3.get_down())
			{
				return CubeFace.Down;
			}
			if (dir == Vector3.get_left())
			{
				return CubeFace.Left;
			}
			if (dir == Vector3.get_right())
			{
				return CubeFace.Right;
			}
			if (dir == Vector3.get_forward())
			{
				return CubeFace.Front;
			}
			if (dir == Vector3.get_back())
			{
				return CubeFace.Back;
			}
			return CubeFace.Other;
		}

		public static Mesh Combine(Dictionary<Material, List<MeshInstance>> meshInstancesPerMaterial, Dictionary<Material, int> colorIndicies, int height)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			_meshesAttributes.Clear();
			foreach (KeyValuePair<Material, List<MeshInstance>> item in meshInstancesPerMaterial)
			{
				List<MeshInstance> value = item.Value;
				for (int i = 0; i < value.Count; i++)
				{
					num += value[i].mesh.get_vertexCount();
				}
			}
			Vector3[] array = (Vector3[])new Vector3[num];
			Vector3[] array2 = (Vector3[])new Vector3[num];
			Vector4[] array3 = (Vector4[])new Vector4[num];
			Vector2[] array4 = (Vector2[])new Vector2[num];
			Vector2[] array5 = (Vector2[])new Vector2[num];
			Color32[] array6 = (Color32[])new Color32[num];
			Mesh val = new Mesh();
			val.set_name("Combined Mesh");
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			_meshesReminder.Clear();
			foreach (KeyValuePair<Material, List<MeshInstance>> item2 in meshInstancesPerMaterial)
			{
				Material key = item2.Key;
				List<MeshInstance> value2 = item2.Value;
				int num6 = -1;
				if (colorIndicies.ContainsKey(key))
				{
					num6 = colorIndicies[key];
				}
				num3 = num4++;
				int num8 = 0;
				for (int j = 0; j < value2.Count; j++)
				{
					MeshInstance meshInstance = value2[j];
					if (Object.op_Implicit(meshInstance.mesh))
					{
						num8 += meshInstance.mesh.GetTriangles(meshInstance.subMeshIndex).Length;
					}
				}
				int[] array7 = new int[num8];
				int num9 = 0;
				for (int k = 0; k < value2.Count; k++)
				{
					MeshInstance meshInstance2 = value2[k];
					Mesh mesh = meshInstance2.mesh;
					if (Object.op_Implicit(mesh))
					{
						if (!_meshesAttributes.TryGetValue(mesh.GetInstanceID(), out MeshAttribute value3))
						{
							value3 = default(MeshAttribute);
							value3.vertices = mesh.get_vertices();
							value3.normals = mesh.get_normals();
							value3.tangents = mesh.get_tangents();
							value3.uv = mesh.get_uv();
							value3.colors = mesh.get_colors();
							_meshesAttributes.Add(mesh.GetInstanceID(), value3);
						}
						Matrix4x4 transform = meshInstance2.transform;
						Matrix4x4 inverse = transform.get_inverse();
						transform = inverse.get_transpose();
						Copy(value3.vertices, array, num5, ref meshInstance2.transform);
						CopyNormal(value3.normals, array2, num5, ref transform);
						CopyTangents(value3.tangents, array3, num5, ref transform);
						Copy(value3.uv, array4, num5);
						meshInstance2.textureOffset = num2 * CubeFaceExtensions.NumberOfDirections();
						Color[] colors = value3.colors;
						Color black = Color.get_black();
						if (num6 >= 0)
						{
							int num10 = meshInstance2.colorIndex * 2;
							black._002Ector(((float)num10 + 0.5f) / CombineMeshes.ColourTextureWidth, ((float)num10 + 1.5f) / CombineMeshes.ColourTextureWidth, ((float)num6 + 0.5f) / CombineMeshes.ColourTextureWidth, 0f);
						}
						int num11 = num5 + mesh.get_vertexCount();
						int num12 = 6;
						for (int l = num5; l < num11; l++)
						{
							array6[l] = Color32.op_Implicit(black);
							if (colors.Length > l - num5)
							{
								num12 = (int)GenerateSideIndexFromVector(CalculateFaceDirection(ref colors[l - num5], ref meshInstance2.rotation));
							}
							int num13 = meshInstance2.textureOffset + num12;
							int num14 = num13 % 224;
							int num15 = Mathf.FloorToInt((float)num13 / 224f);
							array5[l] = new Vector2(((float)num14 + 0.5f) / 224f, ((float)num15 + 0.5f) / (float)height);
						}
						int[] triangles = mesh.GetTriangles(meshInstance2.subMeshIndex);
						meshInstance2.subMeshIndex = num3;
						meshInstance2.vertexOffset = num5;
						meshInstance2.colorIndex = num6;
						for (int m = 0; m < triangles.Length; m++)
						{
							array7[m + num9] = triangles[m] + num5;
						}
						num9 += triangles.Length;
						num5 += mesh.get_vertexCount();
						num2++;
					}
				}
				if (_meshesReminder.ContainsKey(num3))
				{
					_meshesReminder[num3].AddRange(array7);
				}
				else
				{
					_meshesReminder.Add(num3, new List<int>(array7));
				}
			}
			val.set_vertices(array);
			val.set_normals(array2);
			val.set_colors32(array6);
			val.set_tangents(array3);
			val.set_uv(array4);
			val.set_uv2(array5);
			val.set_subMeshCount(num4);
			Dictionary<int, List<int>>.KeyCollection.Enumerator enumerator3 = _meshesReminder.Keys.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				int current2 = enumerator3.Current;
				val.SetTriangles(_meshesReminder[current2], current2);
			}
			return val;
		}

		private static void Copy(Vector3[] src, Vector3[] dst, int offset, ref Matrix4x4 transform)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < src.Length; i++)
			{
				dst[i + offset] = transform.MultiplyPoint3x4(src[i]);
			}
		}

		private static void CopyNormal(Vector3[] src, Vector3[] dst, int offset, ref Matrix4x4 transform)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < src.Length; i++)
			{
				ref Vector3 reference = ref dst[i + offset];
				Vector3 val = transform.MultiplyVector(src[i]);
				reference = val.get_normalized();
			}
		}

		private static void Copy(Vector2[] src, Vector2[] dst, int offset)
		{
			Array.Copy(src, 0, dst, offset, src.Length);
		}

		private static void CopyColors(Color[] src, Color[] dst, int offset)
		{
			Array.Copy(src, 0, dst, offset, src.Length);
		}

		private static void CopyTangents(Vector4[] src, Vector4[] dst, int offset, ref Matrix4x4 transform)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < src.Length; i++)
			{
				Vector4 val = src[i];
				Vector3 normalized = default(Vector3);
				normalized._002Ector(val.x, val.y, val.z);
				Vector3 val2 = transform.MultiplyVector(normalized);
				normalized = val2.get_normalized();
				dst[i + offset] = new Vector4(normalized.x, normalized.y, normalized.z, val.w);
			}
		}
	}

	internal sealed class CombineMeshes
	{
		private const uint MAX_VERTEX_COUNT_PER_MESH = 65000u;

		public const int DAMAGE_TEXTURE_WIDTH = 32;

		public const int DEFORM_TEXTURE_WIDTH = 224;

		private const string CUBE_SHADER = "Robocraft/Reflective/Bumped Specular";

		private const string CUBE_PROTONIUM_SHADER = "Robocraft/Protonium Crystal";

		private const string CUBE_SHADER_TEXTURED = "Robocraft/Reflective/Bumped Specular Texture - Colored";

		private const string CUBE_SHADER_TEXTURED_C6 = "Robocraft/Reflective/Bumped Specular Texture - Colored C6";

		private const string CUBE_SHADER_GLASS = "Robocraft/Transparent/Glass Cubes";

		private const string CUBE_SHADER_GLOW_PULSE = "Robocraft/Component/Electroplate Build";

		private const string CUBE_NEON = "Robocraft/Cube - Neon";

		private static float _colourTextureWidth = 0f;

		private static Dictionary<string, Shader> ReplacementShaders = new Dictionary<string, Shader>
		{
			{
				"Robocraft/Reflective/Bumped Specular",
				COMBINED_SHADER
			},
			{
				"Robocraft/Reflective/Bumped Specular Texture - Colored",
				COMBINED_TEXTURED_SHADER
			},
			{
				"Robocraft/Reflective/Bumped Specular Texture - Colored C6",
				COMBINED_TEXTURED_SHADER_C6
			},
			{
				"Robocraft/Transparent/Glass Cubes",
				COMBINED_GLASS_SHADER
			},
			{
				"Robocraft/Component/Electroplate Build",
				COMBINED_SHADER_GLOW_PULSE
			},
			{
				"Robocraft/Cube - Neon",
				COMBINED_NEON
			}
		};

		public static float ColourTextureWidth
		{
			get
			{
				return _colourTextureWidth;
			}
			set
			{
				_colourTextureWidth = value;
			}
		}

		public static Texture2D GenerateNewBlankTexture(int width, int height, out Color32[] initialColorArray, bool is8Bit)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			Texture2D val = new Texture2D(width, height, is8Bit ? 1 : 5, false);
			val.set_filterMode(0);
			val.set_wrapMode(1);
			Texture2D result = val;
			initialColorArray = (Color32[])new Color32[width * height];
			return result;
		}

		public static void Combine(GameObject newMeshGO, FasterList<CubeBatchData> cubesToMerge, Dictionary<int, List<MeshInstance>> meshInstancesPerCube, ColorPaletteData palette, Dictionary<int, FilterData> combinedMeshesData, List<Object> resourcesToRelease)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<Material, List<MeshInstance>> dictionary = new Dictionary<Material, List<MeshInstance>>();
			List<Material> list = new List<Material>();
			int num = 0;
			for (int i = 0; i < cubesToMerge.get_Count(); i++)
			{
				CubeBatchData cubeBatchData = cubesToMerge.get_Item(i);
				Transform root = cubeBatchData.root;
				root.set_position(cubeBatchData.position);
				root.set_rotation(cubeBatchData.rotation);
				for (int j = 0; j < cubeBatchData.filters.Length; j++)
				{
					MeshFilter val = cubeBatchData.filters[j];
					MeshRenderer component = val.GetComponent<MeshRenderer>();
					if (!(component != null) || !(val.get_sharedMesh() != null))
					{
						continue;
					}
					Material[] sharedMaterials = component.get_sharedMaterials();
					for (int k = 0; k < sharedMaterials.Length; k++)
					{
						MeshInstance meshInstance = new MeshInstance();
						meshInstance.transform.SetTRS(component.get_transform().get_position(), component.get_transform().get_rotation(), component.get_transform().get_lossyScale());
						meshInstance.rotation = component.get_transform().get_rotation();
						meshInstance.mesh = val.get_sharedMesh();
						meshInstance.cubeID = cubeBatchData.gridHash;
						meshInstance.subMeshIndex = Math.Min(k, meshInstance.mesh.get_subMeshCount() - 1);
						meshInstance.colorIndex = cubeBatchData.paletteIndex;
						Material val2 = sharedMaterials[k];
						if ((long)(num + meshInstance.mesh.get_vertexCount()) < 65000L)
						{
							if (!dictionary.TryGetValue(val2, out List<MeshInstance> value))
							{
								value = new List<MeshInstance>();
								value.Add(meshInstance);
								dictionary.Add(val2, value);
								list.Add(val2);
							}
							else
							{
								value.Add(meshInstance);
							}
							num += meshInstance.mesh.get_vertexCount();
						}
						else
						{
							CreateCombinedObject(meshInstancesPerCube, newMeshGO, dictionary, list, palette, combinedMeshesData, resourcesToRelease);
							dictionary.Clear();
							list.Clear();
							num = 0;
							k--;
						}
					}
					component.set_enabled(false);
				}
				root.set_localPosition(cubeBatchData.position);
			}
			CreateCombinedObject(meshInstancesPerCube, newMeshGO, dictionary, list, palette, combinedMeshesData, resourcesToRelease);
		}

		private static void ParseMaterials(List<Material> singleMaterialList, Color32[] indexColors, ref Dictionary<Material, int> colorIndexes, out List<Material> optimizedMaterials, ColorPaletteData palette)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			int num = 0;
			for (byte b = 0; b < palette.Count; b = (byte)(b + 1))
			{
				Color32 diffuse = palette[b].diffuse;
				indexColors[num++] = diffuse;
				Color val = Color32.op_Implicit(palette[b].specular);
				indexColors[num++] = Color32.op_Implicit(val);
			}
			optimizedMaterials = new List<Material>();
			for (int i = 0; i < singleMaterialList.Count; i++)
			{
				Material val2 = singleMaterialList[i];
				string name = val2.get_shader().get_name();
				if (ReplacementShaders.TryGetValue(name, out Shader value))
				{
					colorIndexes.Add(val2, num);
					Material val3 = new Material(val2);
					val3.set_shader(value);
					optimizedMaterials.Add(val3);
					Color val4 = Color.get_white();
					if (val2.HasProperty("_ReflectColor"))
					{
						val4 = val2.GetColor("_ReflectColor");
						val4.a = val2.GetFloat("_Shininess");
					}
					indexColors[num++] = Color32.op_Implicit(val4);
				}
				else
				{
					Material item = new Material(val2);
					optimizedMaterials.Add(item);
				}
			}
		}

		private static int SetDeformTexture(Dictionary<Material, List<MeshInstance>> meshInstancesPerMaterial, out Color32[] originalColors, out int height, out Texture2D deformTex)
		{
			int num = 0;
			foreach (KeyValuePair<Material, List<MeshInstance>> item in meshInstancesPerMaterial)
			{
				num += item.Value.Count;
			}
			height = Mathf.CeilToInt((float)(num * CubeFaceExtensions.NumberOfDirections()) / 224f);
			height += (height & 3);
			deformTex = GenerateNewBlankTexture(224, height, out originalColors, is8Bit: true);
			deformTex.SetPixels32(originalColors);
			deformTex.Apply();
			return num;
		}

		private static int SetDamageTexture(out Color32[] originalColors, int totalCubes, out Texture2D deformTex)
		{
			int num = Mathf.CeilToInt((float)totalCubes / 32f);
			num += (num & 3);
			deformTex = GenerateNewBlankTexture(32, num, out originalColors, is8Bit: true);
			deformTex.SetPixels32(originalColors);
			deformTex.Apply();
			return totalCubes;
		}

		private static void CreateCombinedObject(Dictionary<int, List<MeshInstance>> meshInstancesPerCube, GameObject newMeshGO, Dictionary<Material, List<MeshInstance>> meshInstancesPerMaterial, List<Material> singleMaterialList, ColorPaletteData palette, Dictionary<int, FilterData> combinedMeshesData, List<Object> resourcesToRelease)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			Dictionary<Material, int> colorIndexes = new Dictionary<Material, int>();
			GameObject val = new GameObject("CombinedMesh");
			val.get_transform().SetParent(newMeshGO.get_transform(), false);
			Color32[] originalColors;
			int height;
			Texture2D deformTex;
			int totalCubes = SetDeformTexture(meshInstancesPerMaterial, out originalColors, out height, out deformTex);
			SetDamageTexture(out Color32[] originalColors2, totalCubes, out Texture2D deformTex2);
			Color32[] initialColorArray;
			Texture2D val2 = GenerateNewBlankTexture((int)ColourTextureWidth, 1, out initialColorArray, is8Bit: false);
			ParseMaterials(singleMaterialList, initialColorArray, ref colorIndexes, out List<Material> optimizedMaterials, palette);
			val2.SetPixels32(initialColorArray);
			val2.Apply();
			for (int i = 0; i < optimizedMaterials.Count; i++)
			{
				Material val3 = optimizedMaterials[i];
				resourcesToRelease.Add(val3);
				if (val3.HasProperty("_ColorMap"))
				{
					val3.SetTexture("_ColorMap", val2);
				}
				val3.SetTexture("_DestroyTex", deformTex);
				val3.SetTexture("_DamageTex", deformTex2);
			}
			resourcesToRelease.Add(val2);
			resourcesToRelease.Add(deformTex2);
			resourcesToRelease.Add(deformTex);
			MeshFilter val4 = val.AddComponent<MeshFilter>();
			MeshRenderer val5 = val.AddComponent<MeshRenderer>();
			val5.set_shadowCastingMode(1);
			val5.set_receiveShadows(true);
			val5.get_gameObject().set_layer(GameLayers.MCUBES);
			val5.set_sharedMaterials(optimizedMaterials.ToArray());
			val4.set_sharedMesh(MeshCombineUtility.Combine(meshInstancesPerMaterial, colorIndexes, height));
			resourcesToRelease.Add(val4.get_sharedMesh());
			foreach (KeyValuePair<Material, List<MeshInstance>> item in meshInstancesPerMaterial)
			{
				List<MeshInstance> value = item.Value;
				for (int j = 0; j < value.Count; j++)
				{
					int instanceID = val4.GetInstanceID();
					MeshInstance meshInstance = value[j];
					meshInstance.combinedMesh = val4;
					meshInstance.combinedMeshHash = instanceID;
					meshInstancesPerCube[value[j].cubeID].Add(meshInstance);
					if (!combinedMeshesData.ContainsKey(instanceID))
					{
						FilterData value2 = default(FilterData);
						value2.mainTexture = val2;
						value2.damageTexture = deformTex2;
						value2.destroyIndicesTexture = deformTex;
						value2.originalColors = originalColors;
						value2.damageColors = originalColors2;
						value2.mainColors = initialColorArray;
						combinedMeshesData.Add(instanceID, value2);
					}
				}
			}
		}
	}

	private static Shader COMBINED_TEXTURED_SHADER = Shader.Find("Robocraft/Combined Textured Cubes Special");

	private static Shader COMBINED_TEXTURED_SHADER_C6 = Shader.Find("Robocraft/Combined Textured Cubes Special C6");

	private static Shader COMBINED_GLASS_SHADER = Shader.Find("Robocraft/Transparent/Combined Cubes");

	private static Shader COMBINED_SHADER = Shader.Find("Robocraft/Combined Cubes");

	private static Shader COMBINED_SHADER_GLOW_PULSE = Shader.Find("Robocraft/Combined Cubes Glow Pulse");

	private static Shader COMBINED_NEON = Shader.Find("Robocraft/Combined Neon Cubes");

	private static Dictionary<int, List<MeshInstance>> meshInstancesPerCube = new Dictionary<int, List<MeshInstance>>();

	private static Dictionary<int, FilterData> combinedMeshesData = new Dictionary<int, FilterData>();

	private const float OPERATION_TIME = 0.001f;

	internal static GameObject BatchCubes(ColorPaletteData palette, Transform machineRoot, FasterList<SettingUpCube> cubes, TargetType targetType)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		Dictionary<int, List<MeshInstance>> dictionary = new Dictionary<int, List<MeshInstance>>();
		FasterList<CubeBatchData> cubesToMerge = new FasterList<CubeBatchData>(cubes.get_Count());
		Dictionary<int, FilterData> dictionary2 = new Dictionary<int, FilterData>();
		ParseCubesBeforeCombine(cubes, cubesToMerge, dictionary, targetType);
		GameObject val = new GameObject("OptimizedMesh");
		val.get_transform().SetParent(MachineBoard.Instance.board, false);
		CreateBatchGameObject(val, machineRoot, cubesToMerge, dictionary, palette, dictionary2, targetType);
		ComputeDestroyTextureIndices(cubes, dictionary, dictionary2, isEditor: false);
		return val;
	}

	internal static void ClearDictionary()
	{
		meshInstancesPerCube.Clear();
		combinedMeshesData.Clear();
	}

	internal static void StartBatchCubes(ColorPaletteData palette, GameObject parent, ChunkMeshUpdater cmu, FasterList<SettingUpCube> cubes, TargetType targetType)
	{
		FasterList<CubeBatchData> cubesToMerge = new FasterList<CubeBatchData>();
		List<Object> resourcesToRelease = new List<Object>();
		ParseCubesBeforeCombine(cubes, cubesToMerge, meshInstancesPerCube, targetType);
		CombineMeshes.Combine(parent, cubesToMerge, meshInstancesPerCube, palette, combinedMeshesData, resourcesToRelease);
		cmu.CubeVertices = meshInstancesPerCube;
		cmu.CombinedMeshesData = combinedMeshesData;
		ComputeDestroyTextureIndices(cubes, meshInstancesPerCube, combinedMeshesData, isEditor: true);
	}

	private static void CreateBatchGameObject(GameObject go, Transform machineBoard, FasterList<CubeBatchData> cubesToMerge, Dictionary<int, List<MeshInstance>> meshInstancesPerCube, ColorPaletteData palette, Dictionary<int, FilterData> combinedMeshesData, TargetType targetType)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		List<Object> resourcesToRelease = new List<Object>();
		if (cubesToMerge.get_Count() > 0)
		{
			CombineMeshes.Combine(go, cubesToMerge, meshInstancesPerCube, palette, combinedMeshesData, resourcesToRelease);
		}
		go.get_transform().set_parent(machineBoard);
		go.get_transform().set_localPosition(Vector3.get_zero());
		go.get_transform().set_localRotation(Quaternion.get_identity());
		ChunkMeshUpdater chunkMeshUpdater = machineBoard.get_gameObject().AddComponent<ChunkMeshUpdater>();
		chunkMeshUpdater.rendererGO = go;
		chunkMeshUpdater.CubeVertices = meshInstancesPerCube;
		chunkMeshUpdater.CombinedMeshesData = combinedMeshesData;
		chunkMeshUpdater.ResourcesToRelease = resourcesToRelease;
	}

	private static void ParseCubesBeforeCombine(FasterList<SettingUpCube> cubes, FasterList<CubeBatchData> cubesToMerge, Dictionary<int, List<MeshInstance>> meshInstancesPerCube, TargetType targetType)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		for (int num = cubes.get_Count() - 1; num >= 0; num--)
		{
			Transform transform = cubes.get_Item(num).transform;
			InstantiatedCube instance = cubes.get_Item(num).instance;
			int hashCode = instance.gridPos.GetHashCode();
			MeshFilter[] componentsInChildren = transform.GetComponentsInChildren<MeshFilter>(true);
			meshInstancesPerCube.Add(hashCode, new List<MeshInstance>());
			CubeBatchData cubeBatchData = default(CubeBatchData);
			cubeBatchData.root = transform;
			cubeBatchData.gridHash = hashCode;
			cubeBatchData.rotation = CubeData.IndexToQuat(instance.rotationIndex);
			cubeBatchData.position = GridScaleUtility.GridToWorld(instance.gridPos, targetType);
			cubeBatchData.filters = componentsInChildren;
			cubeBatchData.paletteIndex = instance.paletteIndex;
			cubesToMerge.Add(cubeBatchData);
		}
	}

	private static void ComputeDestroyTextureIndices(FasterList<SettingUpCube> instantiatedCubes, Dictionary<int, List<MeshInstance>> meshInstancesPerCube, Dictionary<int, FilterData> combinedMeshesData, bool isEditor)
	{
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < instantiatedCubes.get_Count(); i++)
		{
			InstantiatedCube instance = instantiatedCubes.get_Item(i).instance;
			int hashCode = instance.gridPos.GetHashCode();
			byte a = byte.MaxValue;
			if (isEditor && instance.isRed == 0)
			{
				a = 0;
			}
			if (!meshInstancesPerCube.TryGetValue(hashCode, out List<MeshInstance> value))
			{
				continue;
			}
			for (int num = value.Count - 1; num >= 0; num--)
			{
				MeshInstance meshInstance = value[num];
				FilterData filterData = combinedMeshesData[meshInstance.combinedMeshHash];
				Color32[] damageColors = filterData.damageColors;
				int num2 = meshInstance.textureOffset / CubeFaceExtensions.NumberOfDirections();
				damageColors[num2].a = a;
				if (instance.cubeNodeInstance.isDestroyed)
				{
					damageColors = filterData.originalColors;
					int textureOffset = meshInstance.textureOffset;
					byte a2 = byte.MaxValue;
					damageColors[textureOffset].a = a2;
					damageColors[textureOffset + 1].a = a2;
					damageColors[textureOffset + 2].a = a2;
					damageColors[textureOffset + 3].a = a2;
					damageColors[textureOffset + 4].a = a2;
					damageColors[textureOffset + 5].a = a2;
					damageColors[textureOffset + 6].a = a2;
				}
			}
		}
		for (int j = 0; j < instantiatedCubes.get_Count(); j++)
		{
			InstantiatedCube instance2 = instantiatedCubes.get_Item(j).instance;
			CubeNodeInstance cubeNodeInstance = instance2.cubeNodeInstance;
			int hashCode2 = instance2.gridPos.GetHashCode();
			if (!meshInstancesPerCube.TryGetValue(hashCode2, out List<MeshInstance> value2))
			{
				continue;
			}
			FasterList<CubeNodeInstance> neighbours = instance2.cubeNodeInstance.GetNeighbours();
			for (int num3 = neighbours.get_Count() - 1; num3 >= 0; num3--)
			{
				CubeNodeInstance cubeNodeInstance2 = neighbours.get_Item(num3);
				if (!cubeNodeInstance2.isDestroyed && meshInstancesPerCube.ContainsKey(cubeNodeInstance2.instantiatedCube.gridPos.GetHashCode()))
				{
					for (int num4 = value2.Count - 1; num4 >= 0; num4--)
					{
						MeshInstance meshInstance2 = value2[num4];
						FilterData filterData2 = combinedMeshesData[meshInstance2.combinedMeshHash];
						Color32[] originalColors = filterData2.originalColors;
						Quaternion val = CubeData.IndexToQuat(instance2.rotationIndex);
						ConnectionPoint connectionPointOfNeighbour = cubeNodeInstance.GetConnectionPointOfNeighbour(cubeNodeInstance2);
						Vector3 direction = val * connectionPointOfNeighbour.direction;
						int offsetFromDirection = (int)MachineUtility.GetOffsetFromDirection(direction);
						if (cubeNodeInstance2.instantiatedCube.persistentCubeData.IsDirectionOccluding(CubeData.IndexToQuat(cubeNodeInstance2.instantiatedCube.rotationIndex), ((Int3)instance2.gridPos - (Int3)cubeNodeInstance2.instantiatedCube.gridPos).ToVector3()))
						{
							originalColors[meshInstance2.textureOffset + offsetFromDirection].a = byte.MaxValue;
						}
					}
				}
			}
		}
		foreach (FilterData value3 in combinedMeshesData.Values)
		{
			FilterData current = value3;
			current.damageTexture.SetPixels32(current.damageColors);
			current.damageTexture.Apply(false, false);
			current.destroyIndicesTexture.SetPixels32(current.originalColors);
			current.destroyIndicesTexture.Apply(false, false);
		}
	}
}
