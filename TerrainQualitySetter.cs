using UnityEngine;
using Utility;

internal sealed class TerrainQualitySetter : MonoBehaviour
{
	public TerrainQSetting[] qualityLevelSettings;

	private int _currentQuality;

	public TerrainQualitySetter()
		: this()
	{
	}

	private void Start()
	{
		UpdateSettings();
	}

	private void UpdateSettings()
	{
		Terrain val = Object.FindObjectOfType<Terrain>();
		_currentQuality = QualitySettings.GetQualityLevel();
		val.set_heightmapPixelError((float)qualityLevelSettings[_currentQuality].pixelError);
		val.set_basemapDistance((float)qualityLevelSettings[_currentQuality].baseMapDist);
		val.set_castShadows(qualityLevelSettings[_currentQuality].castShadows);
		val.set_detailObjectDensity(qualityLevelSettings[_currentQuality].detailDensity);
		val.set_detailObjectDistance((float)qualityLevelSettings[_currentQuality].detailDistance);
		val.set_materialType(3);
		val.set_materialTemplate(qualityLevelSettings[_currentQuality].alternativeMaterial);
		Shader.set_globalMaximumLOD(qualityLevelSettings[_currentQuality].shaderLOD);
		Console.LogWarning("Terrain Cast Shadows " + val.get_castShadows());
		Console.LogWarning("Terrain Material " + val.get_materialTemplate().get_name());
	}

	private void OnPostRender()
	{
		if (_currentQuality != QualitySettings.GetQualityLevel())
		{
			UpdateSettings();
		}
	}
}
