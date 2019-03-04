namespace EnginesGUI
{
	public class RetargetableSpriteSpecificationComponentImplementor : BaseSpecificationComponentImplementor<string>, IRetargetableSpriteSpecificationComponent
	{
		public bool SpriteTargetForRule(string ruleName, out string spriteName)
		{
			string configResult = null;
			bool result = ConfigurationForRule(ruleName, out configResult);
			spriteName = configResult;
			return result;
		}

		int IRetargetableSpriteSpecificationComponent.get_TargetPanelInstanceId()
		{
			return base.TargetPanelInstanceId;
		}
	}
}
