using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal class CubeColorUpdater : MonoBehaviour
	{
		private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

		private Dictionary<Renderer, Material[]> colouredMaterials = new Dictionary<Renderer, Material[]>();

		private Dictionary<Renderer, Material[]> redMaterials = new Dictionary<Renderer, Material[]>();

		private Renderer[] renderers;

		private const string COLOR_PROPERTY = "_Color";

		private const string SPEC_PROPERTY = "_SpecColor";

		private bool _isRed;

		private bool _isValid = true;

		public bool isValid
		{
			set
			{
				_isValid = value;
				UpdateInvalidColor();
			}
		}

		public CubeColorUpdater()
			: this()
		{
		}

		private void Awake()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			renderers = this.GetComponentsInChildren<Renderer>();
			List<Material> list = new List<Material>();
			List<Material> list2 = new List<Material>();
			Renderer[] array = renderers;
			foreach (Renderer val in array)
			{
				Material[] sharedMaterials = val.get_sharedMaterials();
				originalMaterials[val] = sharedMaterials;
				Material[] sharedMaterials2 = val.get_sharedMaterials();
				foreach (Material val2 in sharedMaterials2)
				{
					if (val2.HasProperty("_Colorable"))
					{
						Material val3 = Object.Instantiate<Material>(val2);
						val3.SetColor("_Color", Color.get_red());
						if (val3.HasProperty("_SpecColor"))
						{
							val3.SetColor("_SpecColor", Color.get_red());
						}
						if (val3.HasProperty("_EmStr") && !val3.HasProperty("_OverrideGlowDisable"))
						{
							val3.SetFloat("_EmStr", 0f);
						}
						val3.SetTexture("_MaskTex", null);
						list.Add(val3);
						Material item = Object.Instantiate<Material>(val2);
						list2.Add(item);
					}
					else
					{
						list.Add(val2);
						list2.Add(val2);
					}
				}
				redMaterials[val] = list.ToArray();
				list.Clear();
				colouredMaterials[val] = list2.ToArray();
				list2.Clear();
			}
		}

		private bool ShouldBeRed()
		{
			return !_isValid;
		}

		private void UpdateInvalidColor()
		{
			if (_isRed != ShouldBeRed())
			{
				_isRed = !_isRed;
				Renderer[] array = renderers;
				foreach (Renderer val in array)
				{
					val.set_materials((!_isRed) ? colouredMaterials[val] : redMaterials[val]);
				}
			}
		}

		public void Reset()
		{
			_isValid = true;
			_isRed = false;
			Renderer[] array = renderers;
			foreach (Renderer val in array)
			{
				val.set_enabled(true);
				for (int j = 0; j < val.get_sharedMaterials().Length; j++)
				{
					val.set_materials(originalMaterials[val]);
				}
			}
		}

		public void SetColor(PaletteColor c)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Renderer[] array = renderers;
			foreach (Renderer val in array)
			{
				Material[] array2 = colouredMaterials[val];
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j].HasProperty("_Colorable"))
					{
						array2[j].SetColor("_Color", Color32.op_Implicit(c.diffuse));
						if (array2[j].HasProperty("_SpecColor"))
						{
							array2[j].SetColor("_SpecColor", Color32.op_Implicit(c.specular));
						}
					}
				}
				val.set_materials((!_isRed) ? array2 : redMaterials[val]);
			}
		}

		private void OnEnable()
		{
			Reset();
		}
	}
}
