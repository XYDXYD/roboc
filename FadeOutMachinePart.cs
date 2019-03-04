using System.Collections.Generic;
using UnityEngine;

internal sealed class FadeOutMachinePart : MonoBehaviour
{
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

	public float FadeToWhiteDuration
	{
		private get;
		set;
	}

	public float ScaleDownDuration
	{
		private get;
		set;
	}

	public float initialDelay
	{
		private get;
		set;
	}

	public FadeOutMachinePart()
		: this()
	{
	}

	private void Awake()
	{
		FadeToWhiteDuration = 0f;
		ScaleDownDuration = 0f;
	}

	private void Start()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (FadingShader != null)
		{
			_rootRenderers = new List<Renderer>();
			_rootScaling = new List<Vector3>();
			_renderers = this.GetComponentsInChildren<Renderer>();
			Renderer[] renderers = _renderers;
			foreach (Renderer val in renderers)
			{
				val.set_enabled(true);
				Material[] materials = val.get_materials();
				foreach (Material val2 in materials)
				{
					val2.set_shader(FadingShader);
				}
				if (!CheckIfHasParentMeshRenderers(val))
				{
					_rootRenderers.Add(val);
					_rootScaling.Add(val.get_transform().get_localScale());
				}
			}
		}
		ParticleSystem[] componentsInChildren = this.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem val3 in array)
		{
			Object.Destroy(val3);
		}
		FadeToWhiteDuration += initialDelay;
	}

	private bool CheckIfHasParentMeshRenderers(Renderer r)
	{
		Transform parent = r.get_transform().get_parent();
		while (parent != null && parent != this.get_transform() && parent.GetComponent<MeshRenderer>() == null)
		{
			parent = parent.get_transform().get_parent();
		}
		return !(parent == this.get_transform());
	}

	private void Update()
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		_timeAlive += Time.get_deltaTime();
		if (_timeAlive < initialDelay)
		{
			return;
		}
		if (_timeAlive < FadeToWhiteDuration)
		{
			scale = _timeAlive / FadeToWhiteDuration;
			Renderer[] renderers = _renderers;
			foreach (Renderer val in renderers)
			{
				if (val != null)
				{
					Material[] materials = val.get_materials();
					foreach (Material val2 in materials)
					{
						val2.SetFloat("_WhiteOut", scale);
					}
				}
			}
		}
		else if (_timeAlive < ScaleDownDuration + FadeToWhiteDuration)
		{
			scale = (_timeAlive - FadeToWhiteDuration) / ScaleDownDuration;
			for (int k = 0; k < _rootRenderers.Count; k++)
			{
				_rootRenderers[k].get_transform().set_localScale(_rootScaling[k] * (1f - scale));
			}
		}
		else
		{
			Object.Destroy(this.get_gameObject());
		}
	}
}
