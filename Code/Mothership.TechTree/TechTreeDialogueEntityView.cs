using Svelto.ECS;

namespace Mothership.TechTree
{
	internal class TechTreeDialogueEntityView : EntityView
	{
		public ITechTreeDialogueButtonsComponent dialogueButtonsComponent;

		public ITechTreeDialogueLabelsComponent dialogueLabelsComponent;

		public ITechTreeDialogueTypeComponent dialogueTypeComponent;

		public ITechTreeDialogueNodeComponent dialogueNodeComponent;

		public IGameObjectComponent gameObjectComponent;

		public TechTreeDialogueEntityView()
			: this()
		{
		}
	}
}
