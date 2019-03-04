using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal static class WeaponRaycastUtility
	{
		internal struct Ray
		{
			public Vector3 startPosition;

			public Vector3 direction;

			public float range;
		}

		internal struct Parameters
		{
			public MachineRootContainer machineRootContainer;

			public PlayerTeamsContainer playerTeamsContainer;

			public PlayerMachinesContainer playerMachinesContainer;

			public NetworkMachineManager machineManager;

			public string fusionShieldTag;

			public int shooterId;

			public bool isShooterAi;

			public void Inject(int id, bool ai, string tag)
			{
				shooterId = id;
				isShooterAi = ai;
				fusionShieldTag = tag;
			}
		}

		public static readonly string ENEMY_FUSION_SHIELD_TAG = "EnemyFusionShield";

		public static readonly string ALLY_FUSION_SHIELD_TAG = "AllyFusionShield";

		public static readonly string MICROBOT_SPHERE_COLLIDER_TAG = "MicrobotSphereCollider";

		public static int shootingLayer = GameLayers.SHOOTING_OTHERS_LAYER_MASK;

		public static int myselfLayer = GameLayers.MYSELF_LAYER_MASK;

		public static int environmentLayer = GameLayers.ENVIRONMENT_LAYER_MASK;

		public static int teamBaseLayer = GameLayers.TEAM_BASE;

		private static GridAllignedLineCheck.GridAlignedCheckDependency _dependency = new GridAllignedLineCheck.GridAlignedCheckDependency();

		private static HitResult[] _singleHitResult = new HitResult[1];

		public static bool RaycastWeaponHit(ref Ray ray, ref Parameters parameters, ref HitResult hitResult)
		{
			int numHits;
			bool result = RaycastWeaponHit(ref ray, ref ray, ref parameters, _singleHitResult, out numHits, 1);
			hitResult = _singleHitResult[0];
			return result;
		}

		public static bool RaycastWeaponHit(ref Ray rayMe, ref Ray ray, ref Parameters parameters, ref HitResult hitResult)
		{
			int numHits;
			bool result = RaycastWeaponHit(ref rayMe, ref ray, ref parameters, _singleHitResult, out numHits, 1);
			hitResult = _singleHitResult[0];
			return result;
		}

		public static bool RaycastWeaponHit(ref Ray ray, ref Parameters parameters, HitResult[] hitResults, out int numHits, int maxHits)
		{
			return RaycastWeaponHit(ref ray, ref ray, ref parameters, hitResults, out numHits, maxHits);
		}

		public static bool RaycastWeaponHit(ref Ray rayMe, ref Ray ray, ref Parameters parameters, HitResult[] hitResults, out int numHits, int maxHits)
		{
			numHits = 0;
			for (int i = 0; i < hitResults.Length; i++)
			{
				hitResults[i].Initialise(ref rayMe);
			}
			if (parameters.isShooterAi)
			{
				if (RaycastPlayerSelf(ref rayMe, ref parameters, hitResults, GameLayers.AI_LAYER_MASK))
				{
					return true;
				}
				shootingLayer = GameLayers.AI_SHOOTING_OTHERS_LAYER_MASK;
			}
			else if (parameters.playerTeamsContainer.IsMe(TargetType.Player, parameters.shooterId))
			{
				if (RaycastPlayerSelf(ref rayMe, ref parameters, hitResults, GameLayers.MYSELF_LAYER_MASK))
				{
					return true;
				}
				shootingLayer = GameLayers.SHOOTING_OTHERS_LAYER_MASK;
			}
			else
			{
				if (RaycastPlayerSelf(ref rayMe, ref parameters, hitResults, GameLayers.ENEMY_PLAYERS_LAYER_MASK))
				{
					return true;
				}
				shootingLayer = GameLayers.SHOOTING_LAYER_MASK;
			}
			return RaycastWeaponHitDoCast(ref ray, ref parameters, doesFusionShieldBlock: true, hitResults, out numHits);
		}

		public static bool RaycastWeaponAim(ref Ray ray, ref Parameters parameters, ref HitResult hitResult)
		{
			bool result = RaycastWeaponAim(ref ray, ref parameters, _singleHitResult, ignoreTeamMates: true);
			hitResult = _singleHitResult[0];
			return result;
		}

		public static bool RaycastWeaponAim(ref Ray ray, ref Parameters parameters, HitResult[] hitResults, bool ignoreTeamMates)
		{
			for (int i = 0; i < hitResults.Length; i++)
			{
				hitResults[i].Initialise(ref ray);
			}
			if (parameters.shooterId == parameters.playerTeamsContainer.localPlayerId)
			{
				shootingLayer = GameLayers.SHOOTING_OTHERS_LAYER_MASK;
			}
			else
			{
				shootingLayer = GameLayers.AI_SHOOTING_OTHERS_LAYER_MASK;
			}
			int numHits;
			return RaycastWeaponHitDoCast(ref ray, ref parameters, doesFusionShieldBlock: false, hitResults, out numHits, ignoreTeamMates);
		}

		private static bool RaycastWeaponHitDoCast(ref Ray ray, ref Parameters parameters, bool doesFusionShieldBlock, HitResult[] hitResults, out int numHits, bool ignoreTeamMates = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			numHits = 0;
			RaycastHit rcHit = default(RaycastHit);
			bool flag2 = Physics.Raycast(ray.startPosition, ray.direction, ref rcHit, ray.range, shootingLayer | (1 << GameLayers.FUSION_SHIELD));
			bool flag3 = false;
			if (doesFusionShieldBlock)
			{
				if (flag2)
				{
					RaycastHit val = default(RaycastHit);
					flag3 = Physics.Raycast(rcHit.get_point(), -ray.direction, ref val, Vector3.Distance(ray.startPosition, rcHit.get_point()), 1 << GameLayers.FUSION_SHIELD);
					if (flag3)
					{
						TargetType type = LayerToTargetType.GetType(val.get_collider().get_gameObject().get_layer());
						if (type == TargetType.FusionShield)
						{
							rcHit = val;
						}
					}
				}
				else
				{
					flag3 = Physics.Raycast(ray.startPosition + ray.direction * ray.range, -ray.direction, ref rcHit, ray.range, shootingLayer | (1 << GameLayers.FUSION_SHIELD));
					if (flag3 && Object.op_Implicit(rcHit.get_collider()) && rcHit.get_collider().get_tag() != parameters.fusionShieldTag)
					{
						flag3 = false;
					}
				}
			}
			if (flag2 || flag3)
			{
				TargetType type2 = LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer());
				if (!doesFusionShieldBlock || (doesFusionShieldBlock && type2 == TargetType.FusionShield && rcHit.get_collider().get_tag() != parameters.fusionShieldTag))
				{
					if (doesFusionShieldBlock)
					{
						int layer = rcHit.get_collider().get_gameObject().get_layer();
						GameObject gameObject = rcHit.get_collider().get_gameObject();
						gameObject.set_layer(GameLayers.IGNORE_RAYCAST);
						float num = 0.01f;
						Vector3 startPosition = ray.startPosition;
						int num2 = shootingLayer | (1 << GameLayers.FUSION_SHIELD);
						while (flag2 = Physics.Raycast(ray.startPosition, ray.direction, ref rcHit, ray.range, num2))
						{
							type2 = LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer());
							if (type2 != TargetType.FusionShield || rcHit.get_collider().get_tag() == parameters.fusionShieldTag)
							{
								ray.startPosition = startPosition;
								break;
							}
							ray.startPosition = rcHit.get_point() + num * ray.direction;
						}
						gameObject.set_layer(layer);
					}
					else
					{
						flag2 = Physics.Raycast(ray.startPosition, ray.direction, ref rcHit, ray.range, shootingLayer);
						if (flag2)
						{
							type2 = LayerToTargetType.GetType(rcHit.get_collider().get_gameObject().get_layer());
						}
					}
				}
				if (flag2 || flag3)
				{
					if (LayerToTargetType.IsTargetDestructible(type2))
					{
						GameObject machineBoard = GameUtility.GetMachineBoard(rcHit.get_transform());
						int machineIdFromRoot = parameters.machineRootContainer.GetMachineIdFromRoot(type2, machineBoard);
						int playerFromMachineId = parameters.playerMachinesContainer.GetPlayerFromMachineId(type2, machineIdFromRoot);
						int playerTeam = parameters.playerTeamsContainer.GetPlayerTeam(type2, playerFromMachineId);
						int playerTeam2 = parameters.playerTeamsContainer.GetPlayerTeam(TargetType.Player, parameters.shooterId);
						bool flag4 = playerTeam == playerTeam2;
						if (!ignoreTeamMates || !flag4)
						{
							numHits = DoGridCheck(ref ray, type2, rcHit, ref parameters, machineIdFromRoot, hitResults);
							for (int i = 0; i < numHits; i++)
							{
								switch (type2)
								{
								case TargetType.Player:
									hitResults[i].hitAlly = flag4;
									break;
								case TargetType.TeamBase:
								case TargetType.EqualizerCrystal:
									hitResults[i].hitOwnBase = flag4;
									break;
								}
							}
							flag = (numHits > 0);
						}
					}
					else if (doesFusionShieldBlock && !flag && type2 == TargetType.FusionShield && rcHit.get_collider() != null && rcHit.get_collider().get_tag() == parameters.fusionShieldTag)
					{
						HitResult hitResult = hitResults[0];
						hitResult.hitPoint = rcHit.get_point();
						hitResult.normal = rcHit.get_normal();
						hitResult.targetType = TargetType.FusionShield;
						hitResults[0] = hitResult;
						flag = true;
					}
					if (!flag)
					{
						flag2 = Physics.Raycast(ray.startPosition, ray.direction, ref rcHit, ray.range, environmentLayer);
						if (flag2)
						{
							HitResult hitResult2 = hitResults[0];
							hitResult2.hitPoint = rcHit.get_point();
							hitResult2.normal = rcHit.get_normal();
							hitResult2.targetType = TargetType.Environment;
							hitResults[0] = hitResult2;
							flag = true;
						}
					}
				}
			}
			return flag2 || flag3;
		}

		private static bool RaycastPlayerSelf(ref Ray ray, ref Parameters parameters, HitResult[] hitResults, int layerMask)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			RaycastHit rcHit = default(RaycastHit);
			bool flag2 = Physics.Raycast(ray.startPosition, ray.direction, ref rcHit, ray.range, layerMask);
			if (layerMask == GameLayers.MYSELF_LAYER_MASK && flag2 && rcHit.get_collider().get_tag() == MICROBOT_SPHERE_COLLIDER_TAG)
			{
				ray.startPosition = rcHit.get_point() + ray.direction * 0.01f;
				flag2 = Physics.Raycast(ray.startPosition, ray.direction, ref rcHit, ray.range, layerMask);
			}
			if (flag2)
			{
				GameObject machineBoard = GameUtility.GetMachineBoard(rcHit.get_transform());
				int machineIdFromRoot = parameters.machineRootContainer.GetMachineIdFromRoot(TargetType.Player, machineBoard);
				int num = DoGridCheck(ref ray, TargetType.Player, rcHit, ref parameters, machineIdFromRoot, hitResults);
				for (int i = 0; i < num; i++)
				{
					int playerFromMachineId = parameters.playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, hitResults[i].hitTargetMachineId);
					flag |= (hitResults[i].hitSelf = (playerFromMachineId == parameters.shooterId));
				}
			}
			return flag;
		}

		private static int DoGridCheck(ref Ray ray, TargetType type, RaycastHit rcHit, ref Parameters parameters, int targetMachineId, HitResult[] hitResults)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			IMachineMap machineMap = parameters.machineManager.GetMachineMap(type, targetMachineId);
			Rigidbody rigidbody = rcHit.get_rigidbody();
			_dependency.Populate(rcHit.get_point(), rigidbody, ray.startPosition, ray.direction, ray.range, machineMap, type, null);
			int cubeInGridStepLine = GridAllignedLineCheck.GetCubeInGridStepLine(_dependency, hitResults);
			if (cubeInGridStepLine <= 0 && rcHit.get_collider().get_tag() == MICROBOT_SPHERE_COLLIDER_TAG)
			{
				_dependency.Populate(rcHit.get_point(), rigidbody, rcHit.get_point(), rigidbody.get_worldCenterOfMass() - rcHit.get_point(), ray.range, machineMap, type, null);
				cubeInGridStepLine = GridAllignedLineCheck.GetCubeInGridStepLine(_dependency, hitResults);
			}
			if (cubeInGridStepLine > 0)
			{
				Vector3 position = rigidbody.get_position();
				Quaternion rotation = rigidbody.get_rotation();
				for (int i = 0; i < cubeInGridStepLine; i++)
				{
					HitResult hitResult = hitResults[i];
					hitResult.hitTargetMachineId = targetMachineId;
					hitResult.hitPoint = rcHit.get_point();
					hitResult.normal = rcHit.get_normal();
					hitResult.targetType = type;
					hitResults[i] = hitResult;
				}
			}
			return cubeInGridStepLine;
		}
	}
}
