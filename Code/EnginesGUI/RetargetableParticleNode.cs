using Svelto.ECS;

namespace EnginesGUI
{
	internal sealed class RetargetableParticleNode : EntityView
	{
		public IRetargetableParticleComponent retargetableParticleComponent;

		public readonly IRetargetableParticleSpecificationComponent retargetableParticleSpecificationComponent;

		public RetargetableParticleNode()
			: this()
		{
		}
	}
}
