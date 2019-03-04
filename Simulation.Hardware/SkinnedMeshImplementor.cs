using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware
{
	internal class SkinnedMeshImplementor : MonoBehaviour, IRendererComponent, IBonesComponent
	{
		private List<Transform> _bones = new List<Transform>();

		private MeshRenderer _renderer;

		Renderer IRendererComponent.renderer
		{
			get
			{
				return _renderer;
			}
		}

		public List<Transform> bones => _bones;

		public int textureId
		{
			get;
			set;
		}

		public SkinnedMeshImplementor()
			: this()
		{
		}

		private void Awake()
		{
			_renderer = this.GetComponent<MeshRenderer>();
		}

		public void AddBone(Transform bone)
		{
			_bones.Add(bone);
		}
	}
}
