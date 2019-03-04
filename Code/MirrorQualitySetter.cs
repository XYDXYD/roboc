using UnityEngine;

internal sealed class MirrorQualitySetter : MonoBehaviour
{
	public bool[] qualityLevelSettings;

	public Material on;

	public Material off;

	private int _currentQuality;

	public MirrorQualitySetter()
		: this()
	{
	}

	private void Awake()
	{
		UpdateSettings();
	}

	private void Update()
	{
		if (_currentQuality != QualitySettings.GetQualityLevel())
		{
			UpdateSettings();
		}
	}

	private void UpdateSettings()
	{
		_currentQuality = QualitySettings.GetQualityLevel();
		MirrorReflection3 component = this.GetComponent<MirrorReflection3>();
		if (_currentQuality < 3)
		{
			component.set_enabled(false);
		}
		else
		{
			component.set_enabled(true);
		}
		MeshRenderer component2 = this.GetComponent<MeshRenderer>();
		if (_currentQuality < 3)
		{
			component2.set_material(off);
		}
		else
		{
			component2.set_material(on);
		}
	}
}
