using Svelto.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace EnginesGUI
{
	internal class LayoutAdjustmentToScreenConfigEngine : MultiEntityViewsEngine<ScreenConfigurationNode, AnchorsManipulationNode>
	{
		private Dictionary<int, List<AnchorsManipulationNode>> _anchorManipulationNodes = new Dictionary<int, List<AnchorsManipulationNode>>();

		private Dictionary<int, Dictionary<string, ScreenConfigRulesSpecification>> _screenConfigSpecificationForNodes = new Dictionary<int, Dictionary<string, ScreenConfigRulesSpecification>>();

		protected override void Add(ScreenConfigurationNode node)
		{
			node.panelSizeComponent.PanelSizeChanged.NotifyOnValueSet((Action<int, Vector2>)HandlePanelSizeChanged);
			Console.Log("added notify on value set for panel size change");
			int panelID = node.panelSizeComponent.PanelID;
			RegisterScreenConfigurations(panelID, node.screenConfigComponent.ruleNames, node.screenConfigComponent.screenConfigurationRulesSpecifications);
		}

		protected override void Remove(ScreenConfigurationNode node)
		{
			node.panelSizeComponent.PanelSizeChanged.StopNotify((Action<int, Vector2>)HandlePanelSizeChanged);
			UnRegisterScreenConfigurations(node.panelSizeComponent.PanelID);
		}

		protected override void Add(AnchorsManipulationNode node)
		{
			int targetPanelInstanceId = node.anchorsSpecificationComponent.TargetPanelInstanceId;
			if (!_anchorManipulationNodes.ContainsKey(targetPanelInstanceId))
			{
				_anchorManipulationNodes[targetPanelInstanceId] = new List<AnchorsManipulationNode>();
			}
			_anchorManipulationNodes[targetPanelInstanceId].Add(node);
		}

		protected override void Remove(AnchorsManipulationNode node)
		{
			if (_anchorManipulationNodes.ContainsKey(node.anchorsSpecificationComponent.TargetPanelInstanceId))
			{
				List<AnchorsManipulationNode> list = _anchorManipulationNodes[node.anchorsSpecificationComponent.TargetPanelInstanceId];
				list.Remove(node);
			}
		}

		private void HandlePanelSizeChanged(int panelID, Vector2 newSize)
		{
			Dictionary<string, ScreenConfigRulesSpecification> dictionary = _screenConfigSpecificationForNodes[panelID];
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, ScreenConfigRulesSpecification> item in dictionary)
			{
				string key = item.Key;
				ScreenConfigRulesSpecification value = item.Value;
				if (CheckIfRulesPassed(value, newSize.x, newSize.y))
				{
					list.Add(key);
				}
			}
			HandleRulesPassedOnPanelSizeChange(panelID, list);
		}

		private bool CheckIfRulesPassed(ScreenConfigRulesSpecification ruleSpecification, double newSizeX, double newSizeY)
		{
			RuleTarget ruleTarget = ruleSpecification.ruleTarget1;
			double ruleValue = ruleSpecification.ruleValue1;
			RuleCondition ruleCondition = ruleSpecification.ruleCondition1;
			bool flag = CheckRule(ruleTarget, ruleValue, ruleCondition, newSizeX, newSizeY);
			if (ruleSpecification.ruleTarget2 == RuleTarget.NoCondition)
			{
				return flag;
			}
			ruleTarget = ruleSpecification.ruleTarget2;
			ruleValue = ruleSpecification.ruleValue2;
			ruleCondition = ruleSpecification.ruleCondition2;
			bool flag2 = CheckRule(ruleTarget, ruleValue, ruleCondition, newSizeX, newSizeY);
			return flag && flag2;
		}

		private bool CheckRule(RuleTarget targetType, double value, RuleCondition condition, double sizeX, double sizeY)
		{
			double num = 0.0;
			switch (targetType)
			{
			case RuleTarget.NoCondition:
				return true;
			case RuleTarget.AspectRatio:
				num = sizeX / sizeY;
				num = Math.Round(num, 2);
				value = Math.Round(value, 2);
				break;
			case RuleTarget.ScreenWidth:
				num = sizeX;
				break;
			case RuleTarget.ScreenHeight:
				num = sizeY;
				break;
			}
			switch (condition)
			{
			case RuleCondition.ExactlyEqualTo:
				return value == num;
			case RuleCondition.LessThanOrEqual:
				return value > num;
			case RuleCondition.LessThan:
				return value >= num;
			case RuleCondition.GreaterThan:
				return value <= num;
			case RuleCondition.GreaterThanOrEqual:
				return value < num;
			default:
				return false;
			}
		}

		private void RegisterScreenConfigurations(int panelID, string[] ruleNames, ScreenConfigRulesSpecification[] screenConfigurations)
		{
			if (!_screenConfigSpecificationForNodes.ContainsKey(panelID))
			{
				_screenConfigSpecificationForNodes[panelID] = new Dictionary<string, ScreenConfigRulesSpecification>();
			}
			for (int i = 0; i < ruleNames.Length; i++)
			{
				_screenConfigSpecificationForNodes[panelID].Add(ruleNames[i], screenConfigurations[i]);
			}
		}

		private void UnRegisterScreenConfigurations(int panelID)
		{
			_screenConfigSpecificationForNodes.Remove(panelID);
		}

		private void HandleRulesPassedOnPanelSizeChange(int panelID, List<string> rulesPassed)
		{
			if (_anchorManipulationNodes.ContainsKey(panelID))
			{
				foreach (AnchorsManipulationNode item in _anchorManipulationNodes[panelID])
				{
					if (!ApplyAnchorsToNodeIfRulePassed(item, rulesPassed) && item.anchorsSpecificationComponent.AnchorConfigurationForRule(BaseSpecificationComponentImplementor<AnchorConfiguration>.DEFAULT_CONFIG_NAME, out AnchorConfiguration anchorConfig))
					{
						item.anchorsComponent.ApplyAnchors(anchorConfig);
					}
				}
			}
		}

		private bool ApplyAnchorsToNodeIfRulePassed(AnchorsManipulationNode target, List<string> rulesPassed)
		{
			foreach (string item in rulesPassed)
			{
				if (target.anchorsSpecificationComponent.AnchorConfigurationForRule(item, out AnchorConfiguration anchorConfig))
				{
					target.anchorsComponent.ApplyAnchors(anchorConfig);
					return true;
				}
			}
			return false;
		}
	}
}
