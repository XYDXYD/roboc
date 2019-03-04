using System.Collections.Generic;
using UnityEngine;

internal sealed class MachineTeamColourUtility
{
	private static readonly Color allyTeamColorAlt = new Color(0f, 0.29f, 1f);

	private static readonly Color allyTeamColor = new Color(0f, 0.431f, 1f);

	private static readonly Color enemyTeamColor = new Color(0.7f, 0f, 0f);

	private static readonly Color allyTransparentColor = new Color(0f, 0.431f, 1f, 1f);

	private static readonly Color enemyTransparentColor = new Color(1f, 0f, 0f, 1f);

	private static readonly Color allyDamagedGlassColor = new Color(0f, 0.376f, 1f, 0.49f);

	private static readonly Color enemyDamagedGlassColor = new Color(0.506f, 0f, 0f, 0.435f);

	private static readonly Color allyTeslaColor = new Color(0.45f, 0.75f, 0.905f, 1f);

	private static readonly Color enemyTeslaColor = new Color(0.875f, 0.27f, 0.27f, 1f);

	private static readonly Color allyProtoniumColor = new Color(0f, 0.39f, 1f, 1f);

	private static readonly Color enemyProtoniumColor = new Color(0.86f, 0.057f, 0.057f, 1f);

	private static readonly Color allyBaseColor = new Color(0f, 0.38f, 0.99f, 1f);

	private static readonly Color enemyBaseColor = new Color(1f, 0f, 0f, 1f);

	private static readonly Color enemyParticles = new Color(1f, 0.01f, 0.05f);

	private static readonly Color allyParticles = new Color(0f, 0.431f, 1f);

	private static readonly Dictionary<string, TeamColorInfo> _glowMaterials = new Dictionary<string, TeamColorInfo>
	{
		{
			"Robocraft/Reflective/Bumped Specular Glow Colored",
			new TeamColorInfo("_GlowCol", allyTeamColor, enemyTeamColor, _altColorIsBlack: true)
		},
		{
			"Robocraft/Robocraft standard Component",
			new TeamColorInfo("_TeamColor", allyTeamColor, enemyTeamColor)
		},
		{
			"Robocraft/Robocraft standard Component Skinned",
			new TeamColorInfo("_TeamColor", allyTeamColor, enemyTeamColor)
		},
		{
			"Robocraft/LOD1 Specular Glow",
			new TeamColorInfo("_GlowCol", allyTeamColor, enemyTeamColor, _altColorIsBlack: true)
		},
		{
			"Robocraft/Reflective/Specular Colored v2",
			new TeamColorInfo("_GlowCol", allyBaseColor, enemyBaseColor, _altColorIsBlack: true)
		},
		{
			"Robocraft/Component/Tesla Blade",
			new TeamColorInfo("_TeslaColor", allyTeslaColor, enemyTeslaColor)
		},
		{
			"Legacy Shaders/Transparent/Specular",
			new TeamColorInfo("_Color", allyTransparentColor, enemyTransparentColor)
		},
		{
			"Legacy Shaders/Self-Illumin/Diffuse",
			new TeamColorInfo("_Color", allyTransparentColor, enemyTransparentColor)
		},
		{
			"Legacy Shaders/Transparent/Bumped Specular",
			new TeamColorInfo("_Color", allyDamagedGlassColor, enemyDamagedGlassColor)
		},
		{
			"Robocraft/BattleArena/Protonium Crystal",
			new TeamColorInfo("_Color", allyProtoniumColor, enemyProtoniumColor)
		},
		{
			"Robocraft/BattleArena/FusionTower",
			new TeamColorInfo("_GlowCol", allyBaseColor, enemyBaseColor)
		},
		{
			"Robocraft/Component/Electroplate Build TeamColor",
			new TeamColorInfo("_GlowCol", allyTeamColor, enemyTeamColor)
		},
		{
			"Robocraft/Component/League Badge Hologram",
			new TeamColorInfo("_Color", allyTeamColor, enemyTeamColor)
		},
		{
			"Robocraft/Reflective/Bumped Specular Glow Colored Chaingun",
			new TeamColorInfo("_GlowCol", allyTeamColor, enemyTeamColor)
		},
		{
			"Effects/Cubes/Propeller Blur",
			new TeamColorInfo("_TeamColor", allyTeamColor, enemyTeamColor)
		},
		{
			"Robocraft/Component/Shield_Glow",
			new TeamColorInfo("_TeamColor", allyTeamColor, enemyTeamColor)
		}
	};

	private MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();

	private Dictionary<int, Material> _allySharedMaterials = new Dictionary<int, Material>();

	private Dictionary<int, Material> _enemySharedMaterials = new Dictionary<int, Material>();

	public void SetRobotTeamColors(bool isEnemy, GameObject gameObject)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>(true);
		SkinnedMeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		AssignRobotTeamColors(isEnemy, (Renderer[])componentsInChildren);
		AssignRobotTeamColors(isEnemy, (Renderer[])componentsInChildren2);
		ParticleSystem[] componentsInChildren3 = gameObject.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < componentsInChildren3.Length; i++)
		{
			if (!componentsInChildren3[i].CompareTag("DontColorParticles"))
			{
				componentsInChildren3[i].set_startColor((!isEnemy) ? allyParticles : enemyParticles);
			}
		}
	}

	private void AssignRobotTeamColors(bool isEnemy, Renderer[] renderers)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < renderers.Length; i++)
		{
			Material[] sharedMaterials = renderers[i].get_sharedMaterials();
			renderers[i].GetPropertyBlock(_propertyBlock);
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (sharedMaterials[j] == null)
				{
					continue;
				}
				string name = sharedMaterials[j].get_shader().get_name();
				if (_glowMaterials.TryGetValue(name, out TeamColorInfo value))
				{
					Color val = (!isEnemy) ? value.allyColor : value.enemyColor;
					_propertyBlock.SetColor(value.shaderProperty, val);
					if (value.altColorIsBlack)
					{
						_propertyBlock.SetColor("_AltColor", value.blackColor);
					}
					renderers[i].SetPropertyBlock(_propertyBlock);
					break;
				}
			}
		}
	}
}
