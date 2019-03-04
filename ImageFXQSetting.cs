using System;

[Serializable]
internal sealed class ImageFXQSetting
{
	public bool AA = true;

	public bool SSAOOn = true;

	public bool DeferredShading = true;

	public BloomAndFlaresModes BloomAndFlares;

	public float CullingDistance = 100f;

	public float TerrainCullingDistance = 650f;

	public bool DamageOverlay = true;
}
