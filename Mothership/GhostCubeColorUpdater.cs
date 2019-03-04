using Svelto.DataStructures;
using UnityEngine;

namespace Mothership
{
	internal class GhostCubeColorUpdater : MonoBehaviour
	{
		private FasterList<Material> _materials = new FasterList<Material>();

		internal Shader fadeShader
		{
			get;
			set;
		}

		internal PaletteColor initialColor
		{
			get;
			set;
		}

		public GhostCubeColorUpdater()
			: this()
		{
		}

		private void Start()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Renderer[] componentsInChildren = this.GetComponentsInChildren<Renderer>();
			foreach (Renderer val in componentsInChildren)
			{
				for (int j = 0; j < val.get_materials().Length; j++)
				{
					Material val2 = val.get_materials()[j];
					val2.set_shader(fadeShader);
					val2.SetColor("_Color", Color32.op_Implicit(initialColor.diffuse));
					_materials.Add(val2);
				}
			}
		}

		internal void ApplyColor(PaletteColor color)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _materials.get_Count(); i++)
			{
				_materials.get_Item(i).SetColor("_Color", Color32.op_Implicit(color.diffuse));
			}
		}

		internal void Redify(bool redit)
		{
			for (int i = 0; i < _materials.get_Count(); i++)
			{
				_materials.get_Item(i).SetFloat("_Redify", (!redit) ? 0f : 1f);
			}
		}
	}
}
