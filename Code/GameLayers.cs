using UnityEngine;

internal sealed class GameLayers
{
	public static int LOCAL_PLAYER_CUBES = LayerMask.NameToLayer("LocalPlayerCubes");

	public static int LOCAL_PLAYER_COLLIDERS = LayerMask.NameToLayer("LocalPlayerColliders");

	public static int MCUBES = LayerMask.NameToLayer("MultiplayerCubes");

	public static int MCOLLIDERS = LayerMask.NameToLayer("MultiplayerColliders");

	public static int ECUBES = LayerMask.NameToLayer("EnemyCubes");

	public static int AICOLLIDERS = LayerMask.NameToLayer("AIColliders");

	public static int DEFAULT = LayerMask.NameToLayer("Default");

	public static int TRANSPARENT = LayerMask.NameToLayer("TransparentFX");

	public static int BUILD_COLLISION = LayerMask.NameToLayer("BuildCollision");

	public static int CUBE_FLOOR_LAYER = LayerMask.NameToLayer("CubeGridFloor");

	public static int GHOST_CUBE = LayerMask.NameToLayer("GhostCube");

	public static int CUBE_EXTENTS = LayerMask.NameToLayer("CubeExtents");

	public static int PROPS = LayerMask.NameToLayer("Props");

	public static int BASEBEAM = LayerMask.NameToLayer("Bases");

	public static int TERRAIN = LayerMask.NameToLayer("Terrain");

	public static int LEVEL_BARRIER = LayerMask.NameToLayer("LevelBarrier");

	public static int AI_COLLISION = LayerMask.NameToLayer("EnvCollision4AI");

	public static int IGNORE_CUBE_COLLISION = LayerMask.NameToLayer("IgnoreCubeCollision");

	public static int IGNORE_RAYCAST = LayerMask.NameToLayer("Ignore Raycast");

	public static int TEAM_BASE = LayerMask.NameToLayer("TeamBaseTrigger");

	public static int FUSION_SHIELD = LayerMask.NameToLayer("Shield");

	public static int EQUALIZER = LayerMask.NameToLayer("CubeGridFloor");

	public static int BUILDCOLLISION_UNVERIFIED = LayerMask.NameToLayer("BuildCollisionUnverified");

	public static int BUILD_COLLISION_VERIFICATION_MASK = (1 << CUBE_EXTENTS) | (1 << BUILD_COLLISION) | (1 << BUILDCOLLISION_UNVERIFIED);

	public static int ALL_PLAYERS_LAYER_MASK = (1 << AICOLLIDERS) | (1 << MCOLLIDERS) | (1 << LOCAL_PLAYER_COLLIDERS);

	public static int ENVIRONMENT_LAYER_MASK = (1 << TERRAIN) | (1 << PROPS) | (1 << LEVEL_BARRIER);

	public static int TELEPORT_LAYER_MASK = (1 << TERRAIN) | (1 << PROPS) | (1 << LEVEL_BARRIER) | (1 << FUSION_SHIELD) | (1 << EQUALIZER);

	public static int INTERACTIVE_ENVIRONMENT_LAYER_MASK = (1 << PROPS) | (1 << TERRAIN);

	public static int INTERACTIVE_LAYER_MASK = (1 << AICOLLIDERS) | (1 << MCOLLIDERS) | (1 << PROPS) | (1 << TERRAIN) | (1 << TEAM_BASE) | (1 << EQUALIZER);

	public static int INTERACTIVE_AI_LAYER_MASK = (1 << LOCAL_PLAYER_COLLIDERS) | (1 << PROPS) | (1 << TERRAIN) | (1 << TEAM_BASE) | (1 << EQUALIZER);

	public static int MYSELF_LAYER_MASK = 1 << LOCAL_PLAYER_COLLIDERS;

	public static int AI_LAYER_MASK = 1 << AICOLLIDERS;

	public static int ENEMY_PLAYERS_LAYER_MASK = (1 << MCOLLIDERS) | (1 << AICOLLIDERS);

	public static int SHOOTING_OTHERS_LAYER_MASK = (1 << AICOLLIDERS) | (1 << MCOLLIDERS) | (1 << PROPS) | (1 << TERRAIN) | (1 << LEVEL_BARRIER) | (1 << TEAM_BASE) | (1 << EQUALIZER);

	public static int AI_SHOOTING_OTHERS_LAYER_MASK = (1 << AICOLLIDERS) | (1 << MCOLLIDERS) | (1 << PROPS) | (1 << TERRAIN) | (1 << LEVEL_BARRIER) | (1 << TEAM_BASE) | (1 << LOCAL_PLAYER_COLLIDERS) | (1 << EQUALIZER);

	public static int SHOOTING_LAYER_MASK = (1 << AICOLLIDERS) | (1 << MCOLLIDERS) | (1 << LOCAL_PLAYER_COLLIDERS) | (1 << PROPS) | (1 << TERRAIN) | (1 << LEVEL_BARRIER) | (1 << TEAM_BASE) | (1 << EQUALIZER);

	public static int PROJECTOR_MUST_IGNORE_LAYER_MASK = (1 << AICOLLIDERS) | (1 << ECUBES) | (1 << IGNORE_CUBE_COLLISION) | (1 << MCUBES) | (1 << LOCAL_PLAYER_CUBES) | (1 << AI_COLLISION) | (1 << AICOLLIDERS) | (1 << TEAM_BASE) | (1 << FUSION_SHIELD) | (1 << EQUALIZER);

	public static int SPOT_LAYER_MASK = (1 << AICOLLIDERS) | (1 << MCOLLIDERS) | (1 << MCUBES) | (1 << PROPS) | (1 << TERRAIN);

	public static int MAP_PING_LAYER_MASK = (1 << TERRAIN) | (1 << PROPS) | (1 << LEVEL_BARRIER);
}
