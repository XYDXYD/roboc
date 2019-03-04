namespace EnginesGUI
{
	public class RetargetableParticleSpecificationComponentImplementor : BaseSpecificationComponentImplementor<ParticleConfiguration>, IRetargetableParticleSpecificationComponent
	{
		public bool ParticleTargetForRule(string ruleName, out ParticleConfiguration particleConfig)
		{
			ParticleConfiguration configResult = null;
			bool result = ConfigurationForRule(ruleName, out configResult);
			particleConfig = configResult;
			return result;
		}

		int IRetargetableParticleSpecificationComponent.get_TargetPanelInstanceId()
		{
			return base.TargetPanelInstanceId;
		}
	}
}
