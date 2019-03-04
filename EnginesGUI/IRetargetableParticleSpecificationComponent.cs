namespace EnginesGUI
{
	internal interface IRetargetableParticleSpecificationComponent
	{
		int TargetPanelInstanceId
		{
			get;
		}

		bool ParticleTargetForRule(string ruleName, out ParticleConfiguration particleConfig);
	}
}
