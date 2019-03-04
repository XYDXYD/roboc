using System;

namespace EnginesGUI
{
	[Serializable]
	public class RuleInformation
	{
		public RuleTarget RuleTarget;

		public RuleCondition RuleCondition = RuleCondition.Any;

		public double RuleValue;
	}
}
