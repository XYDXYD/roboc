using System.Collections.Generic;
using UnityEngine;

internal sealed class RobotCullDistanceModifier
{
	private Dictionary<int, float> _originalCullDistances = new Dictionary<int, float>();

	private bool _isInitialised;

	public void SetRobotCullDistanceToFarClipDistance()
	{
		Camera main = Camera.get_main();
		float[] layerCullDistances = main.get_layerCullDistances();
		if (!_isInitialised)
		{
			_originalCullDistances[GameLayers.MCUBES] = layerCullDistances[GameLayers.MCUBES];
			_originalCullDistances[GameLayers.ECUBES] = layerCullDistances[GameLayers.ECUBES];
			_isInitialised = true;
		}
		float farClipPlane = main.get_farClipPlane();
		layerCullDistances[GameLayers.MCUBES] = farClipPlane;
		layerCullDistances[GameLayers.ECUBES] = farClipPlane;
		main.set_layerCullDistances(layerCullDistances);
	}

	public void ResetRobotCullDistance()
	{
		if (_isInitialised)
		{
			Camera main = Camera.get_main();
			float[] layerCullDistances = main.get_layerCullDistances();
			layerCullDistances[GameLayers.MCUBES] = _originalCullDistances[GameLayers.MCUBES];
			layerCullDistances[GameLayers.ECUBES] = _originalCullDistances[GameLayers.ECUBES];
			main.set_layerCullDistances(layerCullDistances);
		}
	}
}
