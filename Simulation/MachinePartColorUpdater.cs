using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class MachinePartColorUpdater : MonoBehaviour
	{
		public const float MAX_HEALTH_PERCENT = 0.8f;

		private const string HEALTH_PROPERTY = "_Health";

		private const string COLORABLE_PROPERTY = "_Colorable";

		private const string COLOR_PROPERTY = "_Color";

		private const string SPEC_COLOR_PROPERTY = "_Specular";

		private List<Renderer> _renderers = new List<Renderer>();

		private MaterialPropertyBlock _propertyBlock;

		private PaletteColor _currentColor;

		public MachinePartColorUpdater()
			: this()
		{
		}

		public void Initialize(PaletteColor c)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			Renderer[] componentsInChildren = this.GetComponentsInChildren<Renderer>();
			_propertyBlock = new MaterialPropertyBlock();
			_currentColor = c;
			_propertyBlock.Clear();
			_propertyBlock.SetColor("_Color", Color32.op_Implicit(c.diffuse));
			_propertyBlock.SetColor("_Specular", Color32.op_Implicit(c.specular));
			_propertyBlock.SetFloat("_Health", 1f);
			foreach (Renderer val in componentsInChildren)
			{
				if (!(val is TrailRenderer) && !(val is ParticleRenderer) && !(val is ParticleSystemRenderer))
				{
					val.SetPropertyBlock(_propertyBlock);
					_renderers.Add(val);
				}
			}
		}

		public void SetHealth(float healthPercent)
		{
			for (int i = 0; i < _renderers.Count; i++)
			{
				Renderer val = _renderers[i];
				val.GetPropertyBlock(_propertyBlock);
				_propertyBlock.SetFloat("_Health", healthPercent);
				val.SetPropertyBlock(_propertyBlock);
			}
		}

		private void OnDisable()
		{
			SetHealth(1f);
		}
	}
}
