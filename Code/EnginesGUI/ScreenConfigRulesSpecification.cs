namespace EnginesGUI
{
	public struct ScreenConfigRulesSpecification
	{
		public RuleTarget ruleTarget1;

		public RuleCondition ruleCondition1;

		public double ruleValue1;

		public RuleTarget ruleTarget2;

		public RuleCondition ruleCondition2;

		public double ruleValue2;

		public ScreenConfigRulesSpecification(RuleTarget ruleTarget_, RuleCondition ruleCondition_, double ruleValue1_)
		{
			ruleTarget1 = ruleTarget_;
			ruleCondition1 = ruleCondition_;
			ruleValue1 = ruleValue1_;
			ruleTarget2 = RuleTarget.NoCondition;
			ruleCondition2 = RuleCondition.Any;
			ruleValue2 = 0.0;
		}

		public ScreenConfigRulesSpecification(RuleTarget ruleTarget1_, RuleCondition ruleCondition1_, double ruleValue1_, RuleTarget ruleTarget2_, RuleCondition ruleCondition2_, double ruleValue2_)
		{
			ruleTarget1 = ruleTarget1_;
			ruleCondition1 = ruleCondition1_;
			ruleValue1 = ruleValue1_;
			ruleTarget2 = ruleTarget2_;
			ruleCondition2 = ruleCondition2_;
			ruleValue2 = ruleValue2_;
		}
	}
}
