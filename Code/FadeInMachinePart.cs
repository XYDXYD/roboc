using System.Collections.Generic;
using UnityEngine;

internal sealed class FadeInMachinePart : MonoBehaviour
{
	private Dictionary<Material, Shader> _originalShaders;

	private Renderer[] _renderers;

	private List<Renderer> _rootRenderers;

	private List<Vector3> _rootScaling;

	private float scale;

	private float _timeAlive;

	public Shader FadingShader
	{
		private get;
		set;
	}

	public float FadeFromWhiteDuration
	{
		private get;
		set;
	}

	public float ScaleUpDuration
	{
		private get;
		set;
	}

	public FadeInMachinePart()
		: this()
	{
	}

	private void Awake()
	{
		FadeFromWhiteDuration = 0f;
		ScaleUpDuration = 0f;
	}

	private void Start()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (!(FadingShader != null))
		{
			return;
		}
		_rootRenderers = new List<Renderer>();
		_rootScaling = new List<Vector3>();
		_renderers = this.GetComponentsInChildren<Renderer>();
		_originalShaders = new Dictionary<Material, Shader>();
		Renderer[] renderers = _renderers;
		foreach (Renderer val in renderers)
		{
			Material[] materials = val.get_materials();
			foreach (Material val2 in materials)
			{
				_originalShaders.Add(val2, val2.get_shader());
				val2.set_shader(FadingShader);
				val2.SetFloat("_WhiteOut", 1f);
			}
			if (!CheckIfHasParentMeshRenderers(val))
			{
				_rootRenderers.Add(val);
				_rootScaling.Add(val.get_transform().get_localScale());
				val.get_transform().set_localScale(Vector3.get_zero());
			}
		}
	}

	private bool CheckIfHasParentMeshRenderers(Renderer r)
	{
		Transform parent = r.get_transform().get_parent();
		while (parent != this.get_transform() && parent.GetComponent<MeshRenderer>() == null)
		{
			parent = parent.get_transform().get_parent();
		}
		return !(parent == this.get_transform());
	}

	private void Update()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		_timeAlive += Time.get_deltaTime();
		if (_timeAlive < ScaleUpDuration)
		{
			scale = _timeAlive / ScaleUpDuration;
			for (int i = 0; i < _rootRenderers.Count; i++)
			{
				_rootRenderers[i].get_transform().set_localScale(_rootScaling[i] * scale);
			}
		}
		else if (_timeAlive < ScaleUpDuration + FadeFromWhiteDuration)
		{
			scale = (_timeAlive - ScaleUpDuration) / FadeFromWhiteDuration;
			Renderer[] renderers = _renderers;
			foreach (Renderer val in renderers)
			{
				if (val != null)
				{
					Material[] materials = val.get_materials();
					foreach (Material val2 in materials)
					{
						val2.SetFloat("_WhiteOut", 1f - scale);
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < _rootRenderers.Count; l++)
			{
				_rootRenderers[l].get_transform().set_localScale(_rootScaling[l]);
			}
			foreach (KeyValuePair<Material, Shader> originalShader in _originalShaders)
			{
				originalShader.Key.set_shader(originalShader.Value);
			}
			Object.Destroy(this);
		}
	}
}
