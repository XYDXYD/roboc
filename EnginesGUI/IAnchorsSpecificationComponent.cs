namespace EnginesGUI
{
	internal interface IAnchorsSpecificationComponent
	{
		int TargetPanelInstanceId
		{
			get;
		}

		bool AnchorConfigurationForRule(string ruleName, out AnchorConfiguration anchorConfig);
	}
}
