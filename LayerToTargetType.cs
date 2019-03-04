using Simulation.Hardware.Weapons;
using System.Collections.Generic;

internal static class LayerToTargetType
{
	private static readonly Dictionary<int, TargetType> Types = new Dictionary<int, TargetType>
	{
		{
			GameLayers.AICOLLIDERS,
			TargetType.Player
		},
		{
			GameLayers.MCOLLIDERS,
			TargetType.Player
		},
		{
			GameLayers.LOCAL_PLAYER_COLLIDERS,
			TargetType.Player
		},
		{
			GameLayers.FUSION_SHIELD,
			TargetType.FusionShield
		},
		{
			GameLayers.TEAM_BASE,
			TargetType.TeamBase
		},
		{
			GameLayers.EQUALIZER,
			TargetType.EqualizerCrystal
		},
		{
			GameLayers.TERRAIN,
			TargetType.Environment
		},
		{
			GameLayers.PROPS,
			TargetType.Environment
		},
		{
			GameLayers.LEVEL_BARRIER,
			TargetType.Environment
		}
	};

	public static bool IsTargetDestructible(TargetType type)
	{
		return type == TargetType.Player || type == TargetType.TeamBase || type == TargetType.EqualizerCrystal;
	}

	public static TargetType GetType(int layer)
	{
		if (Types.ContainsKey(layer))
		{
			return Types[layer];
		}
		return TargetType.Environment;
	}
}
