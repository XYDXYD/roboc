namespace EnginesGUI
{
	public class AnchorsSpecificationComponentImplementor : BaseSpecificationComponentImplementor<AnchorConfiguration>, IAnchorsSpecificationComponent
	{
		public bool AnchorConfigurationForRule(string ruleName, out AnchorConfiguration anchorConfig)
		{
			AnchorConfiguration configResult = null;
			bool result = ConfigurationForRule(ruleName, out configResult);
			anchorConfig = configResult;
			return result;
		}

		int IAnchorsSpecificationComponent.get_TargetPanelInstanceId()
		{
			return base.TargetPanelInstanceId;
		}
	}
}
