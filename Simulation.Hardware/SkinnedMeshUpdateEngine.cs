using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Simulation.Hardware
{
	internal class SkinnedMeshUpdateEngine : SingleEntityViewEngine<SkinnedMeshNode>, IQueryingEntityViewEngine, IEngine
	{
		private const int TEXTURE_WIDTH = 256;

		private const int TEXTURE_HEIGHT = 256;

		private const int MAX_BONES = 16;

		private const int TEXTURE_BYTES = 1048576;

		private int _PropertyId;

		private Matrix4x4[] matrices = (Matrix4x4[])new Matrix4x4[15];

		private MaterialPropertyBlock _materialProperty;

		private int _id;

		private Texture2D _globalTexture;

		private float[] _textureData;

		private bool _useTexture;

		private IntPtr _address;

		private GCHandle _handle;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public SkinnedMeshUpdateEngine()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			_useTexture = (Shader.IsKeywordEnabled("VTF_ON") && SystemInfo.SupportsTextureFormat(5));
			_materialProperty = new MaterialPropertyBlock();
			if (_useTexture)
			{
				_id = 0;
				_PropertyId = Shader.PropertyToID("_TextureOffset");
				_globalTexture = GenerateNewBlankTexture(256, 256, out _textureData);
				_handle = GCHandle.Alloc(_textureData, GCHandleType.Pinned);
				_address = _handle.AddrOfPinnedObject();
				Shader.SetGlobalTexture("_BoneMatrices", _globalTexture);
				Shader.DisableKeyword("SKINNED_TEXTURE_OFF");
				Shader.EnableKeyword("SKINNED_TEXTURE_ON");
			}
			else
			{
				_PropertyId = Shader.PropertyToID("_BoneTransforms");
				Shader.EnableKeyword("SKINNED_TEXTURE_OFF");
				Shader.DisableKeyword("SKINNED_TEXTURE_ON");
			}
		}

		~SkinnedMeshUpdateEngine()
		{
			_handle.Free();
		}

		protected override void Add(SkinnedMeshNode node)
		{
			if (_useTexture)
			{
				Renderer renderer = node.rendererComponent.renderer;
				renderer.GetPropertyBlock(_materialProperty);
				_materialProperty.SetFloat(_PropertyId, (float)_id);
				renderer.SetPropertyBlock(_materialProperty);
				node.bonesComponent.textureId = _id;
				_id += 64;
			}
		}

		protected override void Remove(SkinnedMeshNode node)
		{
		}

		public void Ready()
		{
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_updateScheduler(), Update());
		}

		private IEnumerator Update()
		{
			while (true)
			{
				UpdateNodes();
				yield return null;
			}
		}

		private void UpdateNodes()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<SkinnedMeshNode> val = entityViewsDB.QueryEntityViews<SkinnedMeshNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				SkinnedMeshNode node = val.get_Item(i);
				UpdateNode(node);
			}
			if (_useTexture)
			{
				_globalTexture.LoadRawTextureData(_address, 1048576);
				_globalTexture.Apply(false);
			}
		}

		private void UpdateNode(SkinnedMeshNode node)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			Renderer renderer = node.rendererComponent.renderer;
			if (!(renderer != null) || !renderer.get_gameObject().get_activeInHierarchy() || (!renderer.get_isVisible() && renderer.get_enabled()))
			{
				return;
			}
			int textureId = node.bonesComponent.textureId;
			List<Transform> bones = node.bonesComponent.bones;
			int num = 0;
			int num2 = 0;
			while (num < bones.Count)
			{
				Transform val = bones[num];
				if (val.get_hasChanged())
				{
					if (_useTexture)
					{
						Matrix4x4 localToWorldMatrix = val.get_localToWorldMatrix();
						int num3 = (textureId + num2) * 4;
						_textureData[num3] = localToWorldMatrix.m00;
						_textureData[num3 + 1] = localToWorldMatrix.m01;
						_textureData[num3 + 2] = localToWorldMatrix.m02;
						_textureData[num3 + 3] = localToWorldMatrix.m03;
						_textureData[num3 + 4] = localToWorldMatrix.m10;
						_textureData[num3 + 5] = localToWorldMatrix.m11;
						_textureData[num3 + 6] = localToWorldMatrix.m12;
						_textureData[num3 + 7] = localToWorldMatrix.m13;
						_textureData[num3 + 8] = localToWorldMatrix.m20;
						_textureData[num3 + 9] = localToWorldMatrix.m21;
						_textureData[num3 + 10] = localToWorldMatrix.m22;
						_textureData[num3 + 11] = localToWorldMatrix.m23;
					}
					else
					{
						matrices[num] = val.get_localToWorldMatrix();
					}
				}
				num++;
				num2 += 4;
			}
			if (!_useTexture)
			{
				renderer.GetPropertyBlock(_materialProperty);
				_materialProperty.SetMatrixArray(_PropertyId, matrices);
				renderer.SetPropertyBlock(_materialProperty);
			}
		}

		private Texture2D GenerateNewBlankTexture(int width, int height, out float[] data)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Texture2D val = new Texture2D(width, height, 20, false);
			val.set_filterMode(0);
			val.set_wrapMode(1);
			Texture2D result = val;
			data = new float[width * height * 4];
			return result;
		}
	}
}
