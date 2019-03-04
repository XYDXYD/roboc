using UnityEngine;

namespace EnginesGUI
{
	[RequireComponent(typeof(PanelSizeComponentImplementor))]
	public class ScreenConfigPresetsComponentImplementor : MonoBehaviour, IScreenConfigComponent
	{
		public RuleSet[] Rules;

		private string[] _ruleNames;

		private ScreenConfigRulesSpecification[] _screenConfigs;

		public string[] ruleNames => _ruleNames;

		public ScreenConfigRulesSpecification[] screenConfigurationRulesSpecifications => _screenConfigs;

		public ScreenConfigPresetsComponentImplementor()
			: this()
		{
		}

		public void Awake()
		{
			_ruleNames = new string[Rules.Length];
			_screenConfigs = new ScreenConfigRulesSpecification[Rules.Length];
			int num = 0;
			RuleSet[] rules = Rules;
			for (int i = 0; i < rules.Length; i++)
			{
				RuleSet ruleSet = rules[i];
				_ruleNames[num] = ruleSet.RuleSetName;
				_screenConfigs[num] = new ScreenConfigRulesSpecification(Rules[num].Rule1.RuleTarget, Rules[num].Rule1.RuleCondition, Rules[num].Rule1.RuleValue, Rules[num].Rule2.RuleTarget, Rules[num].Rule2.RuleCondition, Rules[num].Rule2.RuleValue);
				num++;
			}
		}
	}
}
