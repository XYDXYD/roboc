using System.ComponentModel;
using UnityEngine;

public class RobocraftSROptions
{
	private bool _GUIValue;

	private bool _fogValue;

	private bool _allObjectsValue;

	private bool _meshRendering;

	public static bool isMachineRenderingDisabled
	{
		get;
		private set;
	}

	public static bool isParticleRenderingDisabled
	{
		get;
		private set;
	}

	[Category("Profiling")]
	public bool DisableGUIRendering
	{
		get
		{
			return _GUIValue;
		}
		set
		{
			_GUIValue = value;
			SwitchGUI(value);
		}
	}

	[Category("Profiling")]
	public bool DisableAllGameObjects
	{
		get
		{
			return _allObjectsValue;
		}
		set
		{
			_allObjectsValue = value;
			SwitchGameObjects(value);
		}
	}

	[Category("Profiling")]
	public bool DisableFog
	{
		get
		{
			return _fogValue;
		}
		set
		{
			_fogValue = value;
			SwitchFog(value);
		}
	}

	[Category("Profiling")]
	public bool DisableMachineRendering
	{
		get
		{
			return isMachineRenderingDisabled;
		}
		set
		{
			isMachineRenderingDisabled = value;
			SwitchMachineRendering(value);
		}
	}

	[Category("Profiling")]
	public bool DisableMeshRendering
	{
		get
		{
			return _meshRendering;
		}
		set
		{
			_meshRendering = value;
			SwitchMeshRenders(value);
		}
	}

	[Category("Profiling")]
	public bool DisablePaArticleRendering
	{
		get
		{
			return isParticleRenderingDisabled;
		}
		set
		{
			isParticleRenderingDisabled = value;
			SwitchParticleRenders(value);
		}
	}

	private void SwitchParticleRenders(bool value)
	{
		ParticleSystemRenderer[] array = Object.FindObjectsOfType<ParticleSystemRenderer>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].set_enabled(!value);
		}
		TrailRenderer[] array2 = Object.FindObjectsOfType<TrailRenderer>();
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].set_enabled(!value);
		}
	}

	private void SwitchMachineRendering(bool value)
	{
	}

	private void SwitchMeshRenders(bool value)
	{
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].set_enabled(!value);
		}
	}

	private void SwitchGUI(bool disable)
	{
		UICamera[] array = Object.FindObjectsOfType<UICamera>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].get_transform().get_gameObject().SetActive(!disable);
		}
	}

	private void SwitchGameObjects(bool disable)
	{
		if (disable)
		{
			Transform[] array = Object.FindObjectsOfType<Transform>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].get_gameObject().SetActive(false);
			}
		}
	}

	private void SwitchFog(bool disable)
	{
		RenderSettings.set_fog(!disable);
	}
}
