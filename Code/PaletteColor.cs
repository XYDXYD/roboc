using UnityEngine;

internal sealed class PaletteColor
{
	public byte paletteIndex
	{
		get;
		set;
	}

	public Color32 diffuse
	{
		get;
		set;
	}

	public Color32 specular
	{
		get;
		set;
	}

	public Color32 overlayColor
	{
		get;
		set;
	}

	public bool isPremium
	{
		get;
		set;
	}

	public PaletteColor(byte index, Color diffuseCol, Color specularCol, Color overlayCol, bool premium)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		diffuse = Color32.op_Implicit(diffuseCol);
		specular = Color32.op_Implicit(specularCol);
		overlayColor = Color32.op_Implicit(overlayCol);
		isPremium = premium;
		paletteIndex = index;
	}
}
