using BehaviorDesigner.Runtime;
using Simulation.SinglePlayer;
using Simulation.SinglePlayer.BehaviorTree;
using System;
using UnityEngine;

[Serializable]
internal class AIEnemyBehaviorTreeImplementor : MonoBehaviour, IBehaviorTreeComponent
{
	private const string TDM_BEHAVIOR_TREE_FILE_NAME = "BehaviorTreeTDM";

	private const string BA_BEHAVIOR_TREE_FILE_NAME = "BehaviorTreeBA";

	public BehaviorTree aiAgentBehaviorTree
	{
		get;
		private set;
	}

	public AIEnemyBehaviorTreeImplementor()
		: this()
	{
	}

	public void LoadData(GameModeType gameMode)
	{
		aiAgentBehaviorTree = this.get_gameObject().AddComponent<BehaviorTree>();
		aiAgentBehaviorTree.set_ExternalBehavior(LoadBehaviorTree(gameMode));
		aiAgentBehaviorTree.set_StartWhenEnabled(true);
	}

	internal void SetBehaviorGlobalVariables(AIGameObjectMovementData aiGameObjectMovementData, AIInputWrapper aiInputWrapper, AIWeaponRaycast weaponRaycast, Transform viewTransform)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		SharedVariable variable = aiAgentBehaviorTree.GetVariable("AiInputWrapper");
		variable.SetValue((object)aiInputWrapper);
		SharedVariable variable2 = aiAgentBehaviorTree.GetVariable("WeaponAimingData");
		variable2.SetValue((object)weaponRaycast);
		SharedVariable variable3 = aiAgentBehaviorTree.GetVariable("ViewTransform");
		variable3.SetValue((object)viewTransform);
		SharedVariable variable4 = aiAgentBehaviorTree.GetVariable("MoveCommandValue");
		variable4.SetValue((object)0f);
		SharedVariable variable5 = aiAgentBehaviorTree.GetVariable("CurrentSteeringGoal");
		variable5.SetValue((object)Vector3.get_zero());
		SharedVariable variable6 = aiAgentBehaviorTree.GetVariable("GetUnstuckState");
		variable6.SetValue((object)GetUnstuckState.Finished);
		SharedVariable variable7 = aiAgentBehaviorTree.GetVariable("WillingToMove");
		variable7.SetValue((object)false);
	}

	private ExternalBehaviorTree LoadBehaviorTree(GameModeType gameMode)
	{
		string empty = string.Empty;
		switch (gameMode)
		{
		case GameModeType.SuddenDeath:
		case GameModeType.Pit:
		case GameModeType.PraticeMode:
		case GameModeType.TeamDeathmatch:
		case GameModeType.Campaign:
			empty = "BehaviorTreeTDM";
			break;
		case GameModeType.Normal:
			empty = "BehaviorTreeBA";
			break;
		default:
			throw new Exception("No behavior resource file name specified for the following game mode: " + gameMode);
		}
		return Resources.Load(empty) as ExternalBehaviorTree;
	}
}
