using Svelto.ECS;

namespace EnginesGUI
{
	internal sealed class AnchorsManipulationNode : EntityView
	{
		public IAnchorsComponent anchorsComponent;

		public readonly IAnchorsSpecificationComponent anchorsSpecificationComponent;

		public AnchorsManipulationNode()
			: this()
		{
		}
	}
}
