using Svelto.ECS;

namespace EnginesGUI
{
	internal sealed class RetargetableSpriteNode : EntityView
	{
		public IRetargetableSpriteComponent retargetableSpriteComponent;

		public readonly IRetargetableSpriteSpecificationComponent retargetableSpriteSpecificationComponent;

		public RetargetableSpriteNode()
			: this()
		{
		}
	}
}
