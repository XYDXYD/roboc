using UnityEngine;

internal sealed class TeamColorInfo
{
	public Color allyColor;

	public Color allyGlassColor = new Color(0f, 0.376f, 1f, 0.49f);

	public bool altColorIsBlack;

	public Color blackColor = new Color(0f, 0f, 0f);

	public Color enemyColor;

	public Color enemyGlassColor = new Color(0.506f, 0f, 0f, 0.435f);

	public string shaderProperty;

	public TeamColorInfo(string _shaderProperty, Color _allyColor, Color _enemyColor)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		shaderProperty = _shaderProperty;
		allyColor = _allyColor;
		enemyColor = _enemyColor;
	}

	public TeamColorInfo(string _shaderProperty, Color _allyColor, Color _enemyColor, bool _altColorIsBlack)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		shaderProperty = _shaderProperty;
		allyColor = _allyColor;
		enemyColor = _enemyColor;
		altColorIsBlack = _altColorIsBlack;
	}
}
