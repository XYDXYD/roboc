using BehaviorDesigner.Runtime;
using Simulation.BattleArena.CapturePoint;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.SinglePlayer.CapturePoints
{
	internal class AICapturePointsEngine : SingleEntityViewEngine<AIAgentDataComponentsNode>, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private CapturePointNotificationObserver _capturePointNotificationObserver;

		private FasterList<AIAgentDataComponentsNode> _agents;

		private Dictionary<int, SharedVariable> _sharedCapturePointData;

		private AICapturePointData[] _capturePointDataArray;

		private float _capturePointRadius;

		private const int MAX_CAPTURE_POINTS = 3;

		private const string CAPTURE_POINTS_DATA_KEY = "CapturePointsData";

		private const string CAPTURE_INFO_KEY = "CaptureInfo";

		public AICapturePointsEngine(CapturePointNotificationObserver capturePointNotificationObserver)
		{
			_capturePointNotificationObserver = capturePointNotificationObserver;
		}

		protected override void Add(AIAgentDataComponentsNode agent)
		{
			_agents.Add(agent);
			_sharedCapturePointData.Add(agent.get_ID(), agent.agentBehaviorTreeComponent.aiAgentBehaviorTree.GetVariable("CapturePointsData"));
			agent.agentBehaviorTreeComponent.aiAgentBehaviorTree.GetVariable("CaptureInfo").SetValue((object)new AICaptureInfo());
			UpdateCapturePointsData();
		}

		protected override void Remove(AIAgentDataComponentsNode agent)
		{
			_agents.UnorderedRemove(agent);
			_sharedCapturePointData.Remove(agent.get_ID());
		}

		unsafe void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			_capturePointNotificationObserver.AddAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_agents = new FasterList<AIAgentDataComponentsNode>();
			_sharedCapturePointData = new Dictionary<int, SharedVariable>();
			_capturePointDataArray = new AICapturePointData[3];
			_capturePointRadius = 0f;
			CapturePointsData capturePointsData = Object.FindObjectOfType<CapturePointsData>();
			if (capturePointsData != null)
			{
				_capturePointRadius = capturePointsData.captureRadius;
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_capturePointNotificationObserver.RemoveAction(new ObserverAction<CapturePointNotificationDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleCapturePointNotification(ref CapturePointNotificationDependency dependency)
		{
			switch (dependency.notification)
			{
			case CapturePointNotification.Spawn:
				HandleCapturePointSpawned(dependency);
				break;
			case CapturePointNotification.CaptureStarted:
			case CapturePointNotification.CaptureStoppedByDefenders:
			case CapturePointNotification.CaptureLocked:
				HandleCaptureStarted(dependency);
				break;
			case CapturePointNotification.CaptureStoppedNoAttackers:
			case CapturePointNotification.CaptureUnlocked:
				HandleCaptureStopped(dependency);
				break;
			case CapturePointNotification.CaptureCompleted:
				HandleCaptureCompleted(dependency);
				break;
			}
			UpdateCapturePointsData();
		}

		private void HandleCapturePointSpawned(CapturePointNotificationDependency dependency)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			_capturePointDataArray[dependency.id] = new AICapturePointData(dependency.position, _capturePointRadius, AICapturePointStatus.Neutral, -1);
		}

		private void HandleCaptureStarted(CapturePointNotificationDependency dependency)
		{
			AICapturePointData aICapturePointData = _capturePointDataArray[dependency.id];
			if (aICapturePointData.Status == AICapturePointStatus.Captured && aICapturePointData.OwnedByTeamId != dependency.attackingTeam)
			{
				aICapturePointData.Status = AICapturePointStatus.UnderAttack;
			}
		}

		private void HandleCaptureStopped(CapturePointNotificationDependency dependency)
		{
			_capturePointDataArray[dependency.id].Status = AICapturePointStatus.Captured;
		}

		private void HandleCaptureCompleted(CapturePointNotificationDependency dependency)
		{
			AICapturePointData aICapturePointData = _capturePointDataArray[dependency.id];
			aICapturePointData.Status = AICapturePointStatus.Captured;
			aICapturePointData.OwnedByTeamId = dependency.attackingTeam;
		}

		private void UpdateCapturePointsData()
		{
			foreach (KeyValuePair<int, SharedVariable> sharedCapturePointDatum in _sharedCapturePointData)
			{
				sharedCapturePointDatum.Value.SetValue((object)_capturePointDataArray);
			}
		}
	}
}
