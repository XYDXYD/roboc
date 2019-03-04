using UnityEngine;
using UnityEngine.PostProcessing;
using Utility;

internal sealed class ImageEffectsQualitySetter : MonoBehaviour
{
	public ImageFXQSetting[] qualityLevelSettings;

	private int _currentQuality;

	private PostProcessingProfile _postProcessProfile;

	public ImageEffectsQualitySetter()
		: this()
	{
	}

	private void Awake()
	{
		_postProcessProfile = this.GetComponent<PostProcessingBehaviour>().profile;
	}

	private void OnEnable()
	{
		UpdateSettings();
	}

	private void OnPostRender()
	{
		if (_currentQuality != QualitySettings.GetQualityLevel())
		{
			UpdateSettings();
		}
	}

	private void UpdateSettings()
	{
		_currentQuality = QualitySettings.GetQualityLevel();
		for (int i = 0; i < Camera.get_allCamerasCount(); i++)
		{
			Camera.get_allCameras()[i].set_allowHDR(false);
			Camera.get_allCameras()[i].set_depthTextureMode(0);
		}
		Camera component = this.GetComponent<Camera>();
		float[] array = new float[32];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = 216f;
		}
		array[GameLayers.BASEBEAM] = 650f;
		array[GameLayers.FUSION_SHIELD] = 650f;
		array[GameLayers.TERRAIN] = qualityLevelSettings[_currentQuality].TerrainCullingDistance;
		array[GameLayers.PROPS] = qualityLevelSettings[_currentQuality].TerrainCullingDistance;
		array[GameLayers.MCUBES] = qualityLevelSettings[_currentQuality].CullingDistance;
		array[GameLayers.LOCAL_PLAYER_CUBES] = qualityLevelSettings[_currentQuality].CullingDistance;
		array[GameLayers.ECUBES] = qualityLevelSettings[_currentQuality].CullingDistance;
		component.set_layerCullDistances(array);
		component.set_layerCullSpherical(true);
		if (!qualityLevelSettings[_currentQuality].SSAOOn)
		{
			Console.Log("SSAO DISABLED");
			_postProcessProfile.ambientOcclusion.set_enabled(false);
		}
		else
		{
			Console.Log("SSAO ENABLED");
			_postProcessProfile.ambientOcclusion.set_enabled(true);
		}
		if (!qualityLevelSettings[_currentQuality].AA || !qualityLevelSettings[_currentQuality].DeferredShading)
		{
			Console.Log("AntialiasingAsPostEffect DISABLED");
			_postProcessProfile.antialiasing.set_enabled(false);
		}
		else
		{
			Console.LogWarning("AntialiasingAsPostEffect ENABLED");
			_postProcessProfile.antialiasing.set_enabled(true);
		}
		if (!qualityLevelSettings[_currentQuality].DeferredShading)
		{
			Console.Log("Forward Rendering");
			_postProcessProfile.fog.set_enabled(false);
		}
		else
		{
			for (int k = 0; k < Camera.get_allCamerasCount(); k++)
			{
				if (Camera.get_allCameras()[k].GetComponent<UICamera>() == null)
				{
					Camera.get_allCameras()[k].set_renderingPath(3);
				}
				else
				{
					Camera.get_allCameras()[k].set_renderingPath(1);
				}
			}
			_postProcessProfile.fog.set_enabled(RenderSettings.get_fog());
		}
		if (qualityLevelSettings[_currentQuality].BloomAndFlares == BloomAndFlaresModes.Off)
		{
			Console.Log("BloomAndFlares DISABLED");
			_postProcessProfile.bloom.set_enabled(false);
			_postProcessProfile.colorGrading.set_enabled(false);
			for (int l = 0; l < Camera.get_allCamerasCount(); l++)
			{
				Camera.get_allCameras()[l].set_allowHDR(false);
			}
		}
		else if (qualityLevelSettings[_currentQuality].BloomAndFlares == BloomAndFlaresModes.HDROn)
		{
			Console.Log("BloomAndFlares Enabled w HDR");
			if (qualityLevelSettings[_currentQuality].DeferredShading)
			{
				_postProcessProfile.colorGrading.set_enabled(true);
				_postProcessProfile.bloom.set_enabled(true);
			}
			else
			{
				_postProcessProfile.colorGrading.set_enabled(false);
				_postProcessProfile.bloom.set_enabled(false);
			}
			for (int m = 0; m < Camera.get_allCamerasCount(); m++)
			{
				Camera.get_allCameras()[m].set_allowHDR(_postProcessProfile.colorGrading.get_enabled());
			}
		}
		OverlayEffectController component2 = this.GetComponent<OverlayEffectController>();
		if (component2 != null)
		{
			component2.set_enabled(qualityLevelSettings[_currentQuality].DamageOverlay);
		}
	}
}
