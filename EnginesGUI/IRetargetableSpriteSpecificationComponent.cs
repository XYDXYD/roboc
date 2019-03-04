namespace EnginesGUI
{
	internal interface IRetargetableSpriteSpecificationComponent
	{
		int TargetPanelInstanceId
		{
			get;
		}

		bool SpriteTargetForRule(string ruleName, out string spriteName);
	}
}
