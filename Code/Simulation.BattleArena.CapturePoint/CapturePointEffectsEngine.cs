using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;
using Utility;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class CapturePointEffectsEngine : SingleEntityViewEngine<EffectsNode>, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		[Inject]
		internal PlayerTeamsContainer teamsContainer
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public unsafe CapturePointEffectsEngine(CapturePointNotificationObserver observer, CapturePointProgressObserver progressObserver)
		{
			observer.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			progressObserver.AddAction(new ObserverAction<TeamBaseStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		protected override void Add(EffectsNode node)
		{
			SetTeam(node, node.visualTeamComponent.visualTeam);
			SetProgress(node, node.progressComponent.progressPercent, node.progressComponent.maxProgress);
		}

		protected override void Remove(EffectsNode node)
		{
		}

		private void HandleOnProgressChanged(ref TeamBaseStateDependency parameter)
		{
			EffectsNode node = default(EffectsNode);
			if (entityViewsDB.TryQueryEntityView<EffectsNode>(parameter.team, ref node))
			{
				SetProgress(node, parameter.currentProgress, parameter.maxProgress);
			}
		}

		private static void SetProgress(EffectsNode node, float current, float max)
		{
			node.progressComponent.progressPercent = current;
			node.progressComponent.maxProgress = max;
			node.propComponent.propRenderer.get_materials()[5].SetFloat("_node_3750", 1f - current / max);
		}

		public void OnDependenciesInjected()
		{
		}

		private void HandleOnNotification(ref CapturePointNotificationDependency parameter)
		{
			switch (parameter.notification)
			{
			case CapturePointNotification.CaptureStarted:
				break;
			case CapturePointNotification.CaptureStoppedNoAttackers:
				break;
			case CapturePointNotification.CaptureStoppedByDefenders:
			case CapturePointNotification.CaptureLocked:
			case CapturePointNotification.CaptureUnlocked:
			case CapturePointNotification.Dominating:
				break;
			case CapturePointNotification.CaptureCompleted:
				HandleOnCaptureCompleted(parameter);
				break;
			case CapturePointNotification.SegmentCompleted:
				HandleOnSegmentCompleted(parameter);
				break;
			case CapturePointNotification.Spawn:
				HandleOnSpawn(parameter);
				break;
			}
		}

		private void SetTeam(EffectsNode node, VisualTeam team)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			_003F val;
			switch (team)
			{
			case VisualTeam.None:
				return;
			case VisualTeam.MyTeam:
				val = node.colorComponent.teamColor;
				break;
			default:
				val = node.colorComponent.enemyColor;
				break;
			}
			Color val2 = val;
			node.captureZoneComponent.sphereRenderer.get_material().SetColor("_TintColor", val2);
			node.captureZoneComponent.plateFxRenderer.get_material().SetColor("_Colour", val2);
			for (int i = 0; i < node.ringsComponent.captureParticleSystem.Length; i++)
			{
				node.ringsComponent.captureParticleSystem[i].set_startColor(val2);
			}
			node.propComponent.propRenderer.set_materials((team != 0) ? node.propComponent.redMaterials : node.propComponent.bluMaterials);
		}

		private void HandleOnSpawn(CapturePointNotificationDependency parameter)
		{
			EffectsNode node = default(EffectsNode);
			if (entityViewsDB.TryQueryEntityView<EffectsNode>(parameter.id, ref node))
			{
				SetTeam(node, GetTeamSide(parameter.defendingTeam));
			}
			else
			{
				Console.LogError("HandleOnSpawn received too soon");
			}
		}

		private void HandleOnSegmentCompleted(CapturePointNotificationDependency parameter)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			EffectsNode effectsNode = default(EffectsNode);
			if (entityViewsDB.TryQueryEntityView<EffectsNode>(parameter.id, ref effectsNode))
			{
				Color val = (!teamsContainer.IsMyTeam(parameter.attackingTeam)) ? effectsNode.colorComponent.enemyColor : effectsNode.colorComponent.teamColor;
				effectsNode.ringsComponent.segmentParticleSystem.set_startColor(val);
				effectsNode.captureZoneComponent.plateFxRenderer.get_material().SetColor("_Colour", val);
				effectsNode.animationComponent.animator.SetTrigger(effectsNode.animationComponent.segmentCapturedTrigger);
			}
			else
			{
				Console.LogError("HandleOnSegmentCompleted received too soon");
			}
		}

		private void HandleOnCaptureCompleted(CapturePointNotificationDependency parameter)
		{
			EffectsNode effectsNode = default(EffectsNode);
			if (entityViewsDB.TryQueryEntityView<EffectsNode>(parameter.id, ref effectsNode))
			{
				SetTeam(effectsNode, GetTeamSide(parameter.attackingTeam));
				effectsNode.animationComponent.animator.SetTrigger(effectsNode.animationComponent.captureCompletedTrigger);
			}
			else
			{
				Console.LogError("HandleOnCaptureCompleted received too soon");
			}
		}

		private VisualTeam GetTeamSide(int team)
		{
			if (team == -1)
			{
				return VisualTeam.None;
			}
			if (teamsContainer.IsMyTeam(team))
			{
				return VisualTeam.MyTeam;
			}
			return VisualTeam.EnemyTeam;
		}
	}
}
