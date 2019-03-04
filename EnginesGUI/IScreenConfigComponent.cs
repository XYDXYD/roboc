namespace EnginesGUI
{
	internal interface IScreenConfigComponent
	{
		string[] ruleNames
		{
			get;
		}

		ScreenConfigRulesSpecification[] screenConfigurationRulesSpecifications
		{
			get;
		}
	}
}
