using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersistentCubeData
{
	[SerializeField]
	internal CubeFaces selectableFaces;

	[SerializeField]
	internal CubeOccludingFaces occludingFaces;

	public bool canRotate = true;

	public int rotationWhenFixed;

	public int initialMirrorRotation;

	public int initialMirrorZOffset;

	public string mirrorCubeId = string.Empty;

	public CubeCategory category;

	[SerializeField]
	internal CubeTypeID cubeType;

	public bool isGoReused = true;

	public bool isBatchable = true;

	[SerializeField]
	internal List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

	[SerializeField]
	internal List<ConnectionPoint> attachablePoints = new List<ConnectionPoint>();

	public bool localiseSprite;

	public SpecialCubesKind specialCubeKind;

	public float groundPlaceHitNormalZ_Rotation;

	public Vector3 groundPlacePositionOffset;

	[NonSerialized]
	public string descriptionStrKey = string.Empty;

	[NonSerialized]
	public int health;

	[NonSerialized]
	public uint cpuRating;

	[NonSerialized]
	public float healthBoost;

	[NonSerialized]
	public int cubeRanking;

	[NonSerialized]
	public bool isIndestructible;

	public IDictionary<string, object> displayStats;

	[NonSerialized]
	internal CubePlacementFaces placementFaces;

	[NonSerialized]
	internal ItemDescriptor itemDescriptor;

	[NonSerialized]
	internal ItemType itemType;

	[NonSerialized]
	public bool protoniumCube;

	[NonSerialized]
	public BuildVisibility buildModeVisibility;

	[NonSerialized]
	public bool greyOutInTutorial;

	[NonSerialized]
	internal CubeColliderInfo[] colliderInfo;

	[NonSerialized]
	public int extentCubesCount;

	[NonSerialized]
	internal bool nonClusteredColliders;

	[NonSerialized]
	internal CubeTypeID variantOf;

	public void InitConnections()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		extentCubesCount = connectionPoints.Count;
		for (int i = 0; i < CubeFaceExtensions.NumberOfFaces(); i++)
		{
			CubeFace cubeFace = (CubeFace)i;
			if (this.IsFaceSelectable(cubeFace))
			{
				connectionPoints.Add(new ConnectionPoint(Vector3.get_zero(), MachineUtility.GetDirection(cubeFace).ToVector3()));
			}
		}
	}

	public void InitAttachablePoints()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		attachablePoints.Add(new ConnectionPoint(Vector3.get_zero(), -Vector3.get_up()));
	}
}
