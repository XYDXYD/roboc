using System;
using UnityEngine;

[Serializable]
internal sealed class TerrainQSetting
{
	public int pixelError = 10;

	public int baseMapDist = 200;

	public bool castShadows = true;

	public float detailDensity = 1f;

	public int detailDistance = 80;

	public Material alternativeMaterial;

	[Tooltip("VertexLit = 100\nDecal, Reflective VertexLit = 150\nDiffuse = 200\nDiffuse Detail, Reflective Bumped Unlit, Reflective Bumped VertexLit = 250\nBumped, Specular = 300\nBumped Specular = 400\nParallax = 500\nParallax Specular = 600")]
	public int shaderLOD = 400;
}
